using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIBZ.Common;
using DIBZ.Common.DTO;
using DIBZ.Common.Model;
using DIBZ.Data;
using DIBZ.Logic.Scorecard;

namespace DIBZ.Logic.Swap
{
    public class SwapLogic : BaseLogic
    {
        public SwapLogic(LogicContext context) : base(context)
        {
        }
        public async Task<List<MySwap>> GetAllOffersOfApplicationUser(int applicationUserId, int currentPage, int pageSize, bool isCount, bool isLatestFirst)
        {
            int skipRecords = (currentPage - 1) * pageSize;

            var swaps = await Db.Query<DIBZ.Common.Model.Swap>(o => (o.Offer.ApplicationUserId == applicationUserId || o.GameSwapPsersonId == applicationUserId) && o.IsActive && !o.IsDeleted).QueryAsync();
            var distinctSwaps = swaps.OrderByDescending(g => g.CreatedTime).GroupBy(g => g.OfferId);
            List<DIBZ.Common.Model.Swap> swapList = new List<Common.Model.Swap>();
            foreach (var item in distinctSwaps)
            {
                swapList.Add((DIBZ.Common.Model.Swap)item.FirstOrDefault());
            }

            var mySwapData = swapList.Select(t => new MySwap
            {
                Id = t.Id,
                OfferId = t.OfferId,
                SwapStatus = (t.Offer.Transactions.Count == 2 && (t.SwapStatus == SwapStatus.Payment_Done_By_Offerer || t.SwapStatus == SwapStatus.Payment_Done_By_Swapper)) ? SwapStatus.Payment_Successful : t.SwapStatus,
                OfferedGameImageId = t.Offer.GameCatalog.GameImageId,
                OfferStatus = t.Offer.OfferStatus,
                IsPaymentRequired = !t.Offer.Transactions.Any(p => p.ApplicationUserId == applicationUserId),
                GameSwapPersonId = t.GameSwapPsersonId,
                OfferPersonId = t.Offer.ApplicationUserId,
                CreatedTime = t.CreatedTime,
                UpdatedTime = t.UpdatedTime,
                OfferPersonNickName = t.Offer.ApplicationUser.NickName,
                GameSwapPersonNickName = t.GameSwapPserson.NickName,
                GameName = t.Offer.GameCatalog.Name,
                GameFormat = t.Offer.GameCatalog.Format.Name,
                ReturnGameName = t.Offer.ReturnGameCatalog.Name,
                ReturnGameFormat = t.Offer.ReturnGameCatalog.Format.Name,
                SwappedGameImageId = t.Offer.ReturnGameCatalog.GameImageId,
                OffererEmail = t.Offer.ApplicationUser.Email,
                SwapperEmail = t.GameSwapPserson.Email
            }).ToList();


            if (isLatestFirst == true)
            {
                if (isCount == true)
                    return mySwapData.OrderByDescending(t => t.CreatedTime).ToList();
                else
                    return mySwapData.OrderByDescending(t => t.CreatedTime).Skip(skipRecords).Take(pageSize).ToList();

            }
            else
            {
                return mySwapData.Skip(skipRecords).Take(pageSize).ToList();
            }
        }

        public async Task<IEnumerable<DIBZ.Common.DTO.Swap>> GetAllSwaps()
        {
            //declarations

            try
            {


                List<DIBZ.Common.Model.Swap> tempSwapList = new List<DIBZ.Common.Model.Swap>();
                List<DIBZ.Common.DTO.Swap> swapListToShow = new List<DIBZ.Common.DTO.Swap>();
                DIBZ.Common.Model.Swap swap = new DIBZ.Common.Model.Swap();

                // 5 day rule check and its impact --- starts
                // DIBZ SRS for phase 2 version 1.5
                //5 day rule starts after both parties have paid.

                var swapListExpiryRule = await Db.Query<DIBZ.Common.Model.Swap>(o => o.IsActive == true && !o.IsDeleted).QueryAsync();
                //AllSwapsRecord Group by with OfferID and get list decrease order
                var groupbyOfferIDsExpiryRule = swapListExpiryRule.OrderByDescending(d => d.Id).GroupBy(g => g.OfferId);

                List<DIBZ.Common.Model.Swap> tempSwapListExpiryRule = new List<DIBZ.Common.Model.Swap>();
                //now for loop to get latest swap status
                foreach (var item in groupbyOfferIDsExpiryRule)
                {
                    //to get latest swap status
                    var tempdata = item.First();
                    swap = (DIBZ.Common.Model.Swap)tempdata;
                    tempSwapListExpiryRule.Add(swap);
                }

                foreach (var tempSwap in tempSwapListExpiryRule)
                {
                    var scorecardLogic = LogicContext.Create<ScorecardLogic>();

                    // ************ 5 day rule starts after both parties have paid. **************

                    var getWorkingDaysCount = GetWorkingDaysCount(ConversionHelper.ConvertDateToTimeZone(tempSwap.CreatedTime));
                    LogHelper.LogInfo("getWorkingDaysCount: " + getWorkingDaysCount);

                    //Condition when both party hasn't sent game on time.
                    if (tempSwap.Offer.OfferStatus == OfferStatus.Accept
                        && tempSwap.Offer.Transactions.Count() == 2
                        && (tempSwap.SwapStatus == SwapStatus.Payment_Done_By_Offerer || tempSwap.SwapStatus == SwapStatus.Payment_Done_By_Swapper)
                        && getWorkingDaysCount > SystemSettings.DayRule)
                    {
                        await scorecardLogic.UpdateScoreCardOfApplicationUserWithNoShow(tempSwap.GameSwapPsersonId);
                        await scorecardLogic.UpdateScoreCardOfApplicationUserWithNoShow(tempSwap.Offer.ApplicationUserId);
                        await this.UpdateOfferStatusToPendingAndSetSwapToInActiveAndRemoveTransactionIfAny(tempSwap.OfferId, tempSwap.Id);

                    }

                    //Condition when offerer party hasn't sent game on time.
                    if (tempSwap.Offer.OfferStatus == OfferStatus.Accept
                        && tempSwap.Offer.Transactions.Count() == 2
                        && (tempSwap.SwapStatus != SwapStatus.Payment_Done_By_Offerer || tempSwap.SwapStatus != SwapStatus.Payment_Done_By_Swapper && tempSwap.SwapStatus == SwapStatus.Game2_Received)
                        && getWorkingDaysCount > SystemSettings.DayRule)
                    {
                        await scorecardLogic.UpdateScoreCardOfApplicationUserWithNoShow(tempSwap.Offer.ApplicationUserId);
                        await UpdateOfferStatusToPendingAndSetSwapToInActiveAndRemoveTransactionIfAny(tempSwap.OfferId, tempSwap.Id);
                    }
                    //Condition when swapper party hasn't sent game on time.
                    if (tempSwap.Offer.OfferStatus == OfferStatus.Accept
                        && tempSwap.Offer.Transactions.Count() == 2
                        && (tempSwap.SwapStatus != SwapStatus.Payment_Done_By_Offerer || tempSwap.SwapStatus != SwapStatus.Payment_Done_By_Swapper && tempSwap.SwapStatus == SwapStatus.Game1_Received)
                        && getWorkingDaysCount > SystemSettings.DayRule)
                    {
                        await scorecardLogic.UpdateScoreCardOfApplicationUserWithNoShow(tempSwap.GameSwapPsersonId);
                        await UpdateOfferStatusToPendingAndSetSwapToInActiveAndRemoveTransactionIfAny(tempSwap.OfferId, tempSwap.Id);
                    }
                }
                // 5 day rule check and its impact --- ends

                var swapList = await Db.Query<DIBZ.Common.Model.Swap>(o => !o.IsDeleted).QueryAsync();
                //AllSwapsRecord Group by with OfferID and get list decrease order
                var groupbyOfferIDs = swapList.OrderByDescending(d => d.Id).GroupBy(g => g.OfferId);

                //now for loop to get latest swap status
                foreach (var item in groupbyOfferIDs)
                {
                    //to get latest swap status
                    var tempdata = item.First();
                    swap = (DIBZ.Common.Model.Swap)tempdata;
                    tempSwapList.Add(swap);
                }
                foreach (var tempSwap in tempSwapList)
                {
                    DIBZ.Common.DTO.Swap swapModel = new DIBZ.Common.DTO.Swap();

                    if (tempSwap.IsActive)
                    {
                        if (tempSwap.SwapStatus == DIBZ.Common.Model.SwapStatus.Game2_Received)
                        {
                            var result = swapList.Where(d => d.OfferId == tempSwap.OfferId).Any(r => r.SwapStatus == DIBZ.Common.Model.SwapStatus.Game1_Received);
                            if (result)
                            {
                                //this assignment is for ,which button to show
                                swapModel.SwapButtonToShow = DIBZ.Common.Model.SwapStatus.Testing;
                            }
                            else
                            {
                                swapModel.SwapButtonToShow = DIBZ.Common.Model.SwapStatus.Game1_Received;
                            }

                        }
                        else if (tempSwap.SwapStatus == DIBZ.Common.Model.SwapStatus.Game1_Received)
                        {
                            var result = swapList.Where(d => d.OfferId == tempSwap.OfferId).Any(r => r.SwapStatus == DIBZ.Common.Model.SwapStatus.Game2_Received);
                            if (result)
                            {
                                swapModel.SwapButtonToShow = DIBZ.Common.Model.SwapStatus.Testing;
                            }
                            else
                            {
                                swapModel.SwapButtonToShow = DIBZ.Common.Model.SwapStatus.Game2_Received;
                            }
                        }
                        if (tempSwap.SwapStatus == DIBZ.Common.Model.SwapStatus.Game2_NoShow)
                        {
                            var result = swapList.Where(d => d.OfferId == tempSwap.OfferId).Any(r => r.SwapStatus == DIBZ.Common.Model.SwapStatus.Game1_NoShow);
                            if (result)
                            {
                                //this assignment is for ,which button to show
                                swapModel.SwapButtonToShow = DIBZ.Common.Model.SwapStatus.Returned;
                            }
                            else
                            {
                                swapModel.SwapButtonToShow = DIBZ.Common.Model.SwapStatus.Game1_NoShow;
                            }

                        }
                        else if (tempSwap.SwapStatus == DIBZ.Common.Model.SwapStatus.Game1_NoShow)
                        {
                            var result = swapList.Where(d => d.OfferId == tempSwap.OfferId).Any(r => r.SwapStatus == DIBZ.Common.Model.SwapStatus.Game2_NoShow);
                            if (result)
                            {
                                swapModel.SwapButtonToShow = DIBZ.Common.Model.SwapStatus.Returned;
                            }
                            else
                            {
                                swapModel.SwapButtonToShow = DIBZ.Common.Model.SwapStatus.Game2_NoShow;
                            }
                        }
                        swapModel.Id = tempSwap.Id;
                        swapModel.CreatedTime = tempSwap.CreatedTime;
                        swapModel.OfferedGameImageId = tempSwap.Offer.GameCatalog.GameImageId;
                        swapModel.SwapGameImageId = tempSwap.GameSwapWith.GameImageId;

                        swapModel.GameSwapPersonNickName = tempSwap.GameSwapPserson.NickName;
                        swapModel.OfferPersonNickName = tempSwap.Offer.ApplicationUser.NickName;
                        swapModel.GameSwapWithId = tempSwap.GameSwapWithId;
                        swapModel.GameSwapPersonId = tempSwap.GameSwapPsersonId;
                        swapModel.OfferPersonId = tempSwap.Offer.ApplicationUserId;
                        swapModel.SwapStatus = (tempSwap.Offer.Transactions.Count == 2 && (tempSwap.SwapStatus == SwapStatus.Payment_Done_By_Offerer || tempSwap.SwapStatus == SwapStatus.Payment_Done_By_Swapper)) ? SwapStatus.Payment_Successful : tempSwap.SwapStatus;
                        swapModel.OfferId = tempSwap.OfferId;
                        swapModel.GameOffererDFOM = tempSwap.Offer.GameOffererDFOM;
                        swapModel.GameSwapperDFOM = tempSwap.Offer.GameSwapperDFOM;
                        swapModel.OfferStatus = tempSwap.Offer.OfferStatus;
                        swapListToShow.Add(swapModel);
                    }


                }
                return swapListToShow;
            }
            catch (Exception ex)
            {

                LogHelper.LogError(ex.Message, ex);
            }
            return null;
        }
        public int GetWorkingDaysCount(DateTime PaymentDate)
        {
            int workingDaysCount = 0;
            int j = 1;

            DateTime startDate = PaymentDate;
            var getNonWorkingDaysList = GetAllNonWorkingDaysPublic();
            while (true)
            {
                startDate = ConversionHelper.ConvertToMinDateTime(PaymentDate.AddDays(j)).AddHours(SystemSettings.DayRuleStartTime);
                if (!getNonWorkingDaysList.Any(o => o.NonWorkingDate == PaymentDate) || PaymentDate.DayOfWeek != DayOfWeek.Saturday || PaymentDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    break;
                }
                j++;
            }

            for (int i = 1; i <= ConversionHelper.SafeConvertToInt32(Math.Round(DateTime.Now.Subtract(PaymentDate).TotalDays)); i++)
            {
                if (!getNonWorkingDaysList.Any(o => o.NonWorkingDate == startDate) || startDate.DayOfWeek == DayOfWeek.Saturday || startDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    workingDaysCount += 1;
                }
            }
            return workingDaysCount;
        }

        public async Task UpdateOfferStatusToPendingAndSetSwapToInActiveAndRemoveTransactionIfAny(int offerId, int swapId)
        {
            var OfferObj = await Db.GetObjectById<DIBZ.Common.Model.Offer>(offerId);
            OfferObj.OfferStatus = OfferStatus.Pending;

            var swapObj = await Db.GetObjectById<DIBZ.Common.Model.Swap>(swapId);
            swapObj.IsActive = false;

            var TransactObj = await Db.Query<DIBZ.Common.Model.Transaction>(o => o.OfferId == offerId).QueryAsync();

            foreach (var item in TransactObj)
            {
                item.IsDeleted = true;
            }
            await Db.SaveAsync();

        }
        public List<DIBZ.Common.Model.NonWorkingDay> GetAllNonWorkingDaysPublic()
        {
            var nonWorkingData = Db.Query<DIBZ.Common.Model.NonWorkingDay>(c => !c.IsDeleted && c.IsActive && c.NonWorkingDate >= DateTime.UtcNow);

            return nonWorkingData.OrderByDescending(o => o.CreatedTime).ToList();
        }
        public async Task<DIBZ.Common.Model.Swap> AddSwap(DIBZ.Common.Model.Swap swap)
        {
            Db.Add(swap);
            await Db.SaveAsync();
            return swap;
        }
        public async Task<DIBZ.Common.DTO.Swap> GetSwapDetailById(int id)
        {
            return (await Db.Query<DIBZ.Common.Model.Swap>(o => o.Id == id).QueryAsync()).Select(o => new DIBZ.Common.DTO.Swap
            {
                OfferedGame = o.Offer.GameCatalog.Name,
                ReturnedGame = o.Offer.ReturnGameCatalog.Name,
                OfferedBy = o.Offer.ApplicationUser.NickName,
                GameSwapPersonNickName = o.GameSwapPserson.NickName,
                SwapDate = o.CreatedTime,
                SwapStatus = (SwapStatus)o.SwapStatus,
                OfferedGameFormat = o.Offer.GameCatalog.Format.Name,
                ReturnedGameFormat = o.Offer.ReturnGameCatalog.Format.Name,

            }).FirstOrDefault();
        }
        public async Task<DIBZ.Common.DTO.Swap> GetSwapDetailByDFOMCode(string dfomCode)
        {
            return (await Db.Query<DIBZ.Common.Model.Swap>(o => o.Offer.GameOffererDFOM == dfomCode || o.Offer.GameSwapperDFOM == dfomCode).QueryAsync()).Select(o => new DIBZ.Common.DTO.Swap
            {
                OfferedGame = o.Offer.GameCatalog.Name,
                ReturnedGame = o.Offer.ReturnGameCatalog.Name,
                OfferedBy = o.Offer.ApplicationUser.NickName,
                GameSwapPersonNickName = o.GameSwapPserson.NickName,
                SwapDate = o.CreatedTime,
                SwapStatus = (SwapStatus)o.SwapStatus,
                OfferedGameFormat = o.Offer.GameCatalog.Format.Name,
                ReturnedGameFormat = o.Offer.ReturnGameCatalog.Format.Name,

            }).FirstOrDefault();

        }
        public async Task<DIBZ.Common.Model.Swap> GetSwapById(int id, string user)
        {
            if (user == "Swapper")
            {
                return (await Db.Query<DIBZ.Common.Model.Swap>(o => o.OfferId == id && o.IsActive && !o.IsDeleted && o.SwapStatus != SwapStatus.Payment_Done_By_Offerer &&
                o.SwapStatus != SwapStatus.Game1_NoShow && o.SwapStatus != SwapStatus.Game1_Received).QueryAsync()).OrderByDescending(o => o.CreatedTime).FirstOrDefault();
            }
            else 
            {
                return (await Db.Query<DIBZ.Common.Model.Swap>(o => o.OfferId == id && o.IsActive && !o.IsDeleted && o.SwapStatus != SwapStatus.Payment_Done_By_Swapper &&
                o.SwapStatus != SwapStatus.Game2_NoShow && o.SwapStatus != SwapStatus.Game2_Received).QueryAsync()).OrderByDescending(o => o.CreatedTime).FirstOrDefault();
            }
        }
    }
}

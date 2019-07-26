using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIBZ.Common.DTO;
using DIBZ.Common.Model;
using DIBZ.Data;
using DIBZ.Logic.CounterOffer;
using DIBZ.Logic.GameCatalog;
using DIBZ.Logic.Scorecard;

namespace DIBZ.Logic.Offer
{
    public class OfferData
    {
        public int Sequence { get; set; }
        public int OfferId { get; set; }
        public int AppUserId { get; set; }
        public string AppUserFullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NickName { get; set; }
        public int GameId { get; set; }
        public int? ReturnGameId { get; set; }
        public string GameName { get; set; }
        public string imgpath { get; set; }
        public string ReturnImgpath { get; set; }
        public decimal sellprice { get; set; }
        public decimal cashprice { get; set; }
        public decimal voucherprice { get; set; }
        public int GameImageId { get; set; }
        public int? ReturnGameImageId { get; set; }
        public string GameFormat { get; set; }
        public int GameFormatId { get; set; }
        public string ReturnGameName { get; set; }
        public string ReturnGameFormat { get; set; }
        public DateTime OfferedTime { get; set; }
        public string OfferedTimeValue { get; set; }
        public string GameCategory { get; set; }
        public int NoOfInterested { get; set; }
        public int? ProfileImageId { get; set; }
        public bool IsCounterOfferMade { get; set; }
        public bool IsCounterOffer { get; set; }
        public int CounterOfferId { get; set; }
        public DIBZ.Common.Model.Scorecard Scorecard { get; set; }
        public OfferStatus OfferStatus { get; set; }
        public string Status { get; set; }
        public bool IsPaymentRequired { get; set; }
    }
    public class OfferLogic : BaseLogic
    {
        public OfferLogic(LogicContext context) : base(context)
        {
        }

        public DIBZ.Common.Model.Offer AddUpdateOffer(OfferModel request)
        {
            DIBZ.Common.Model.Offer offer = new DIBZ.Common.Model.Offer
            {
                OfferStatus = OfferStatus.Pending,
                GameCatalogId = request.GameCatalogId,
                ApplicationUserId = request.ApplicationUserId,
                ReturnGameCatalogId = request.AgaintsGameCatalogId,
                Description = "Testing 02", //request.Description,
                IsActive = true,
            };
            Db.Add(offer);
            Db.Save();
            return offer;
        }

        public async Task<DIBZ.Common.Model.Offer> GetOfferById(int id)
        {
            return (await Db.Query<DIBZ.Common.Model.Offer>(o => o.Id == id && o.IsActive && !o.IsDeleted).QueryAsync()).FirstOrDefault();
        }

        public async Task Delete(int id)
        {
            var OfferObj = await Db.GetObjectById<DIBZ.Common.Model.Offer>(id);
            OfferObj.IsDeleted = true;
            await Db.SaveAsync();

        }

        public async Task<List<OfferData>> GetAllOffers(int formatId)
        {
            var offers = await Db.Query<DIBZ.Common.Model.Offer>(o => o.IsActive && !o.IsDeleted && o.OfferStatus == OfferStatus.Pending
            && o.GameCatalog.FormatId == (formatId == 0 ? o.GameCatalog.FormatId : formatId)).OrderByDescending(g => g.CreatedTime).QueryAsync();
            var offerData = offers.Select(t => new OfferData
            {
                GameName = t.GameCatalog.Name,
                GameId = t.GameCatalogId,
                AppUserId = t.ApplicationUserId,
                AppUserFullName = string.Concat(t.ApplicationUser.FirstName, " ", t.ApplicationUser.LastName),
                NickName = t.ApplicationUser.NickName,
                GameImageId = t.GameCatalog.GameImageId,
                GameFormat = t.GameCatalog.Format.Name,
                OfferedTime = DIBZ.Common.ConversionHelper.ConvertDateToTimeZone(t.CreatedTime),
                GameCategory = t.GameCatalog.Category.Name,
                OfferId = t.Id
            }).ToList();
            int i = 0;
            foreach (var item in offerData)
            {
                //DIBZ.Common.LogHelper.LogInfo("OfferedTime: " + item.OfferedTime);
                DateTime currentTime = DIBZ.Common.ConversionHelper.ConvertDateToTimeZone(DateTime.UtcNow);

                item.Sequence = i;
                var time = DateTime.Now.Subtract(item.OfferedTime).TotalDays;
                if (time < 1)
                {
                    item.OfferedTimeValue = string.Concat(currentTime.Subtract(item.OfferedTime).Hours.ToString(), " Hours");
                }
                else if (time <= 2)
                {
                    item.OfferedTimeValue = string.Concat(currentTime.Subtract(item.OfferedTime).Days.ToString(), " Day");
                }
                else if (time > 2)
                {
                    item.OfferedTimeValue = string.Concat(currentTime.Subtract(item.OfferedTime).Days.ToString(), " Days");
                }
                i++;
            }
            return offerData.OrderByDescending(o => o.OfferedTime).ToList();
        }
        public async Task<List<OfferData>> GetAllOfferForDashboard(int formatId)
        {
            var offers = await Db.Query<DIBZ.Common.Model.Offer>(o => o.IsActive && !o.IsDeleted && o.OfferStatus == OfferStatus.Pending
            && o.GameCatalog.FormatId == (formatId == 0 ? o.GameCatalog.FormatId : formatId)).OrderByDescending(g => g.CreatedTime).QueryAsync();

            var offerData = offers.Select(t => new OfferData
            {
                GameName = t.GameCatalog.Name,
                GameId = t.GameCatalogId,
                AppUserId = t.ApplicationUserId,
                AppUserFullName = string.Concat(t.ApplicationUser.FirstName, " ", t.ApplicationUser.LastName),
                NickName = t.ApplicationUser.NickName,
                GameImageId = t.GameCatalog.GameImageId,
                GameFormat = t.GameCatalog.Format.Name,
                GameFormatId = t.GameCatalog.FormatId,
                OfferedTime = DIBZ.Common.ConversionHelper.ConvertDateToTimeZone(t.CreatedTime),
                OfferId = t.Id
            }).ToList();

            foreach (var item in offerData)
            {
                //DIBZ.Common.LogHelper.LogInfo("OfferedTime: " + item.OfferedTime);
                DateTime currentTime = DIBZ.Common.ConversionHelper.ConvertDateToTimeZone(DateTime.UtcNow);
                var time = DateTime.Now.Subtract(item.OfferedTime).TotalDays;
                if (time < 1)
                {
                    item.OfferedTimeValue = string.Concat(currentTime.Subtract(item.OfferedTime).Hours.ToString(), " Hours");
                }
                else if (time <= 2)
                {
                    item.OfferedTimeValue = string.Concat(currentTime.Subtract(item.OfferedTime).Days.ToString(), " Day");
                }
                else if (time > 2)
                {
                    item.OfferedTimeValue = string.Concat(currentTime.Subtract(item.OfferedTime).Days.ToString(), " Days");
                }

            }
            return offerData.OrderByDescending(o => o.OfferedTime).ToList();
        }

        public async Task<List<OfferData>> ShowPossibleSwapToUser(SearchOffer Request, int currentPage = 1, int pageSize = 5, bool isLatestFirst = true, bool isCount = false)
        {
            int skipRecords = (currentPage - 1) * pageSize;
            var gameCatalogLogic = LogicContext.Create<GameCatalogLogic>();
            var myGames = (await gameCatalogLogic.GetAllGamesOfApplicationUser(Request.AppUserId)).Where(o => o.IsValidForOffer).Select(o => o.GameId).ToList();
            //&& myGames.Contains(o.ReturnGameCatalogId.Value)
            IEnumerable<DIBZ.Common.Model.Offer> offers = null;
            offers = await Db.Query<DIBZ.Common.Model.Offer>(o => o.ApplicationUserId != Request.AppUserId &&
            (o.GameCatalog.Name.ToLower().Contains(Request.GameName.ToLower().Trim()) || o.ReturnGameCatalog.Name.ToLower().Contains(Request.GameName.ToLower().Trim())) &&
            o.OfferStatus == OfferStatus.Pending && o.GameCatalog.FormatId == (Request.FormatId == 0 ? o.GameCatalog.FormatId : Request.FormatId) &&
            o.GameCatalog.CategoryId == (Request.CategoryId == 0 ? o.GameCatalog.CategoryId : Request.CategoryId) && o.IsActive &&
            !o.IsDeleted && o.ReturnGameCatalogId.HasValue && myGames.Contains(o.ReturnGameCatalogId.Value)).QueryAsync();

            var offerData = offers.Select(t => new OfferData
            {
                GameName = t.GameCatalog.Name,
                GameId = t.GameCatalogId,
                AppUserId = t.ApplicationUserId,
                AppUserFullName = string.Concat(t.ApplicationUser.FirstName, " ", t.ApplicationUser.LastName),
                NickName = t.ApplicationUser.NickName,
                GameImageId = t.GameCatalog.GameImageId,
                ReturnGameId = t.ReturnGameCatalogId,                
                GameFormat = t.GameCatalog.Format.Name,
                OfferedTime = DIBZ.Common.ConversionHelper.ConvertDateToTimeZone(t.CreatedTime),
                GameCategory = t.GameCatalog.Category.Name,
                OfferId = t.Id,
                ProfileImageId = t.ApplicationUser.ProfileImageId,
                ReturnGameFormat = t.ReturnGameCatalog.Format.Name,
                ReturnGameName = t.ReturnGameCatalog.Name,
                IsCounterOfferMade = t.CounterOffers.Any(o => o.CounterOfferPersonId == Request.AppUserId),
                OfferStatus = t.OfferStatus,
                IsPaymentRequired = !t.Transactions.Any(p => p.ApplicationUserId == Request.AppUserId)

            }).ToList();

            var scorecardLogic = LogicContext.Create<ScorecardLogic>();
            foreach (var item in offerData)
            {
                //DIBZ.Common.LogHelper.LogInfo("OfferedTime: " + item.OfferedTime);
                DateTime currentTime = DIBZ.Common.ConversionHelper.ConvertDateToTimeZone(DateTime.UtcNow);

                item.Scorecard = await scorecardLogic.GetScoreCardByAppUserId(item.AppUserId);

                var time = DateTime.Now.Subtract(item.OfferedTime).TotalDays;
                if (time < 1)
                {
                    item.OfferedTimeValue = string.Concat(currentTime.Subtract(item.OfferedTime).Hours.ToString(), " Hours");
                }
                else if (time <= 2)
                {
                    item.OfferedTimeValue = string.Concat(Math.Round(time, 0).ToString(), " Day");
                }
                else if (time > 2)
                {
                    item.OfferedTimeValue = string.Concat(Math.Round(time, 0).ToString(), " Days");
                }

            }
            //Getting my counter offers againts those games which are not in my game list
            var myCounterOffers = await Db.Query<DIBZ.Common.Model.CounterOffer>(o => o.CounterOfferPersonId == Request.AppUserId && (o.Offer.OfferStatus == OfferStatus.Pending || o.Offer.OfferStatus == OfferStatus.NotAvailable) &&
            !o.Offer.IsDeleted && o.IsActive && !o.IsDeleted && o.Offer.ReturnGameCatalogId.HasValue && !myGames.Contains(o.Offer.ReturnGameCatalogId.Value)).QueryAsync();
            var myCounterOffersData = myCounterOffers.Select(t => new OfferData
            {
                GameName = t.Offer.GameCatalog.Name,
                GameId = t.Offer.GameCatalogId,
                AppUserId = t.Offer.ApplicationUserId,
                AppUserFullName = string.Concat(t.Offer.ApplicationUser.FirstName, " ", t.Offer.ApplicationUser.LastName),
                NickName = t.Offer.ApplicationUser.NickName,
                GameImageId = t.Offer.GameCatalog.GameImageId,
                ReturnGameId = t.Offer.ReturnGameCatalogId,
                GameFormat = t.Offer.GameCatalog.Format.Name,
                OfferedTime = DIBZ.Common.ConversionHelper.ConvertDateToTimeZone(t.CreatedTime),
                GameCategory = t.Offer.GameCatalog.Category.Name,
                OfferId = t.Offer.Id,
                ProfileImageId = t.Offer.ApplicationUser.ProfileImageId,
                ReturnGameFormat = t.Offer.ReturnGameCatalog.Format.Name,
                ReturnGameName = t.Offer.ReturnGameCatalog.Name,
                IsCounterOfferMade = true,
                CounterOfferId = t.Id,
                OfferStatus = t.Offer.OfferStatus,
                IsPaymentRequired = !t.Offer.Transactions.Any(p => p.ApplicationUserId == Request.AppUserId)

            }).ToList();

            foreach (var item in myCounterOffersData)
            {
                //DIBZ.Common.LogHelper.LogInfo("OfferedTime: " + item.OfferedTime);
                DateTime currentTime = DIBZ.Common.ConversionHelper.ConvertDateToTimeZone(DateTime.UtcNow);

                item.Scorecard = await scorecardLogic.GetScoreCardByAppUserId(item.AppUserId);

                var time = DateTime.Now.Subtract(item.OfferedTime).TotalDays;
                if (time < 1)
                {
                    item.OfferedTimeValue = string.Concat(currentTime.Subtract(item.OfferedTime).Hours.ToString(), " Hours");
                }
                else if (time <= 2)
                {
                    item.OfferedTimeValue = string.Concat(Math.Round(time, 0).ToString(), " Day");
                }
                else if (time > 2)
                {
                    item.OfferedTimeValue = string.Concat(Math.Round(time, 0).ToString(), " Days");
                }
            }
            offerData.AddRange(myCounterOffersData);

            // Get counter offers againts my offers
            var myOfferIds = (await Db.Query<DIBZ.Common.Model.Offer>(o => o.ApplicationUserId == Request.AppUserId && (o.OfferStatus == OfferStatus.NotAvailable ||
            o.OfferStatus == OfferStatus.Pending) && o.IsActive && !o.IsDeleted).QueryAsync()).Select(o => o.Id).ToList();

            var counterOfferLogic = LogicContext.Create<CounterOfferLogic>();
            var counterOffers = await counterOfferLogic.GetAllCounterOffers(myOfferIds);
            var counterOfferData = counterOffers.Select(t => new OfferData
            {
                GameName = t.Offer.GameCatalog.Name,
                GameId = t.Offer.GameCatalogId,
                AppUserId = t.CounterOfferPersonId,
                AppUserFullName = string.Concat(t.CounterOfferPerson.FirstName, " ", t.CounterOfferPerson.LastName),
                NickName = t.CounterOfferPerson.NickName,
                GameImageId = t.Offer.GameCatalog.GameImageId,
                ReturnGameId = t.Offer.ReturnGameCatalogId,
                GameFormat = t.Offer.GameCatalog.Format.Name,
                OfferedTime = DIBZ.Common.ConversionHelper.ConvertDateToTimeZone(t.CreatedTime),
                GameCategory = t.Offer.GameCatalog.Category.Name,
                OfferId = t.Offer.Id,
                ProfileImageId = t.CounterOfferPerson.ProfileImageId,
                ReturnGameFormat = t.Offer.ReturnGameCatalog.Format.Name,
                ReturnGameName = t.Offer.ReturnGameCatalog.Name,
                IsCounterOffer = true,
                CounterOfferId = t.Id,
                OfferStatus = t.Offer.OfferStatus,
                IsPaymentRequired = !t.Offer.Transactions.Any(p => p.ApplicationUserId == Request.AppUserId)

            }).ToList();

            foreach (var item in counterOfferData)
            {
                //DIBZ.Common.LogHelper.LogInfo("OfferedTime: " + item.OfferedTime);
                DateTime currentTime = DIBZ.Common.ConversionHelper.ConvertDateToTimeZone(DateTime.UtcNow);

                item.Scorecard = await scorecardLogic.GetScoreCardByAppUserId(item.AppUserId);

                var time = DateTime.Now.Subtract(item.OfferedTime).TotalDays;
                if (time < 1)
                {
                    item.OfferedTimeValue = string.Concat(currentTime.Subtract(item.OfferedTime).Hours.ToString(), " Hours");
                }
                else if (time <= 2)
                {
                    item.OfferedTimeValue = string.Concat(Math.Round(time, 0).ToString(), " Day");
                }
                else if (time > 2)
                {
                    item.OfferedTimeValue = string.Concat(Math.Round(time, 0).ToString(), " Days");
                }

            }

            offerData.AddRange(counterOfferData);

            foreach (var item in offerData)
            {
                if (item.IsPaymentRequired && item.OfferedTime > Request.AppUserRegisteredTime)
                {
                    var offerForTimeCheck = await this.GetOfferById(item.OfferId);
                    if (offerForTimeCheck != null && offerForTimeCheck.UpdatedTime.HasValue && Math.Round(DateTime.Now.Subtract(offerForTimeCheck.UpdatedTime.Value).TotalHours) >= DIBZ.Common.SystemSettings.PaymentTimeInHours)
                    {
                        offerForTimeCheck.OfferStatus = OfferStatus.Pending;
                        item.OfferStatus = OfferStatus.Pending;
                        await scorecardLogic.UpdateScoreCardOfApplicationUserWithNoShow(Request.AppUserId);

                    }
                }
            }
            await Db.SaveAsync();

            if (isLatestFirst == true)
            {
                if (isCount == true)
                    return offerData.Where(o => o.OfferStatus == OfferStatus.Pending || o.OfferStatus == OfferStatus.PaymentNeeded).OrderByDescending(o => o.OfferedTime).ToList();
                else
                    return offerData.Where(o => o.OfferStatus == OfferStatus.Pending || o.OfferStatus == OfferStatus.PaymentNeeded).OrderByDescending(o => o.OfferedTime).Skip(skipRecords).Take(pageSize).ToList();
            }
            else
            {
                return offerData.Where(o => o.OfferStatus == OfferStatus.Pending || o.OfferStatus == OfferStatus.PaymentNeeded).Skip(skipRecords).Take(pageSize).ToList();
            }
        }

        public async Task<List<OfferData>> SearchOffers(SearchOffer Data, int currentPage)
        {
            var pageSize = Convert.ToInt16(PageSize.SearchOffer);
            int skipRecords = (currentPage - 1) * pageSize;
            IEnumerable<DIBZ.Common.Model.Offer> Offers = null;

            Offers = await Db.Query<DIBZ.Common.Model.Offer>(o => o.OfferStatus == OfferStatus.Pending &&
            o.GameCatalog.Name.ToLower().Contains(Data.GameName.ToLower().Trim()) && o.GameCatalog.FormatId == (Data.FormatId == 0 ? o.GameCatalog.FormatId : Data.FormatId) &&
            o.GameCatalog.CategoryId == (Data.CategoryId == 0 ? o.GameCatalog.CategoryId : Data.CategoryId) && o.IsActive && !o.IsDeleted).OrderByDescending(g => g.CreatedTime).QueryAsync();

            var offerData = Offers.Select(t => new OfferData
            {
                GameName = t.GameCatalog.Name,
                GameId = t.GameCatalogId,
                AppUserId = t.ApplicationUserId,
                AppUserFullName = string.Concat(t.ApplicationUser.FirstName, " ", t.ApplicationUser.LastName),
                NickName = t.ApplicationUser.NickName,
                GameImageId = t.GameCatalog.GameImageId,
                GameFormat = t.GameCatalog.Format.Name,
                OfferedTime = DIBZ.Common.ConversionHelper.ConvertDateToTimeZone(t.CreatedTime),
                OfferId = t.Id
            }).ToList();
            foreach (var item in offerData)
            {
                //DIBZ.Common.LogHelper.LogInfo("OfferedTime: " + item.OfferedTime);
                DateTime currentTime = DIBZ.Common.ConversionHelper.ConvertDateToTimeZone(DateTime.UtcNow);

                var time = DateTime.Now.Subtract(item.OfferedTime).TotalDays;
                if (time < 1)
                {
                    item.OfferedTimeValue = string.Concat(currentTime.Subtract(item.OfferedTime).Hours.ToString(), " Hours");
                    //Math.Round(time, 2, MidpointRounding.AwayFromZero).ToString();
                }
                else if (time <= 2)
                {
                    item.OfferedTimeValue = string.Concat(currentTime.Subtract(item.OfferedTime).Days.ToString(), " Day");
                }
                else if (time > 2)
                {
                    item.OfferedTimeValue = string.Concat(currentTime.Subtract(item.OfferedTime).Days.ToString(), " Days");
                }

            }
            return offerData.OrderByDescending(o => o.OfferedTime).Skip(skipRecords).Take(pageSize).ToList();
        }
        public async Task<List<OfferData>> GetAllOffersOfApplicationUser(int applicationUserId, int currentPage, int pageSize, bool isLatestFirst, bool isCount)
        {
            int skipRecords = (currentPage - 1) * pageSize;
            //&& o.OfferStatus == OfferStatus.Pending
            var offers = await Db.Query<DIBZ.Common.Model.Offer>(o => o.ApplicationUserId == applicationUserId && o.IsActive &&
                         !o.IsDeleted && o.OfferStatus == OfferStatus.Pending).Preload(o => o.GameCatalog).QueryAsync();
            var offerData = offers.Select(o => new OfferData
            {
                OfferId = o.Id,
                GameName = o.GameCatalog.Name,
                GameId = o.GameCatalogId,
                AppUserId = o.ApplicationUserId,
                AppUserFullName = string.Concat(o.ApplicationUser.FirstName, " ", o.ApplicationUser.LastName),
                NickName = o.ApplicationUser.NickName,
                FirstName = o.ApplicationUser.FirstName,
                LastName = o.ApplicationUser.LastName,
                GameImageId = o.GameCatalog.GameImageId,
                //ReturnGameId = (o.ReturnGameCatalogId.HasValue) ? o.ReturnGameCatalogId : null,
                ReturnGameId = (o.ReturnGameCatalogId.HasValue) ? o.ReturnGameCatalogId : null,
                imgpath = o.GameCatalog.imgpath,
                ReturnImgpath = o.ReturnGameCatalog.imgpath,
                GameFormat = o.GameCatalog.Format.Name,
                OfferedTime = DIBZ.Common.ConversionHelper.ConvertDateToTimeZone(o.CreatedTime),
                NoOfInterested = o.CounterOffers.Count,
                GameCategory = o.GameCatalog.Category.Name,
                ReturnGameName = (o.ReturnGameCatalogId.HasValue) ? o.ReturnGameCatalog.Name : string.Empty,
                ReturnGameFormat = (o.ReturnGameCatalogId.HasValue) ? o.ReturnGameCatalog.Format.Name : string.Empty


            }).ToList();

            if (isLatestFirst == true)
            {
                if (isCount == true)
                    return offerData.OrderByDescending(o => o.OfferedTime).ToList();
                else
                    return offerData.OrderByDescending(o => o.OfferedTime).Skip(skipRecords).Take(pageSize).ToList();
            }
            else
            {
                return offerData.Skip(skipRecords).Take(pageSize).ToList();
            }
        }

        public async Task<List<OfferData>> GetAllOffersPaymentOfApplicationUser(int applicationUserId, int currentPage, int pageSize, bool isLatestFirst, bool isCount)
        {
            //int skipRecords = (currentPage - 1) * pageSize;
            ////&& o.OfferStatus == OfferStatus.Pending
            //var offers = await Db.Query<DIBZ.Common.Model.Offer>(o => o.ApplicationUserId == applicationUserId && o.IsActive &&
            //             !o.IsDeleted && (o.OfferStatus == OfferStatus.PaymentNeeded || o.OfferStatus == OfferStatus.Accept || o.OfferStatus == OfferStatus.Pending || o.OfferStatus == OfferStatus.Reject)).Preload(o => o.GameCatalog).QueryAsync();
            //var offerData = offers.Select(o => new OfferData
            //{
            //    OfferId = o.Id,
            //    GameName = o.GameCatalog.Name,
            //    GameId = o.GameCatalogId,
            //    AppUserId = o.ApplicationUserId,
            //    AppUserFullName = string.Concat(o.ApplicationUser.FirstName, " ", o.ApplicationUser.LastName),
            //    NickName = o.ApplicationUser.NickName,
            //    FirstName = o.ApplicationUser.FirstName,
            //    LastName = o.ApplicationUser.LastName,
            //    GameImageId = o.GameCatalog.GameImageId,
            //    ReturnGameImageId = (o.ReturnGameCatalogId.HasValue) ? o.ReturnGameCatalogId : null,
            //    GameFormat = o.GameCatalog.Format.Name,
            //    OfferedTime = DIBZ.Common.ConversionHelper.ConvertDateToTimeZone(o.CreatedTime),
            //    NoOfInterested = o.CounterOffers.Count,
            //    GameCategory = o.GameCatalog.Category.Name,
            //    ReturnGameName = (o.ReturnGameCatalogId.HasValue) ? o.ReturnGameCatalog.Name : string.Empty,
            //    ReturnGameFormat = (o.ReturnGameCatalogId.HasValue) ? o.ReturnGameCatalog.Format.Name : string.Empty,
            //    Status = o.OfferStatus.ToString()


            //}).ToList();

            //if (isLatestFirst == true)
            //{
            //    if (isCount == true)
            //        return offerData.OrderByDescending(o => o.OfferedTime).ToList();
            //    else
            //        return offerData.OrderByDescending(o => o.OfferedTime).Skip(skipRecords).Take(pageSize).ToList();
            //}
            //else
            //{
            //    return offerData.Skip(skipRecords).Take(pageSize).ToList();
            //}

            //item.GameSwapPsersonId == applicationUserId ? item.SwapStatus.ToString() : o.OfferStatus.ToString()

            var swaps = await Db.Query<DIBZ.Common.Model.Swap>(o => (o.Offer.ApplicationUserId == applicationUserId || o.GameSwapPsersonId == applicationUserId) && o.IsActive && !o.IsDeleted).QueryAsync();
            var distinctSwaps = swaps.OrderByDescending(g => g.CreatedTime).GroupBy(g => g.OfferId);
            List<DIBZ.Common.Model.Swap> swapList = new List<Common.Model.Swap>();
            foreach (var item in distinctSwaps)
            {
                swapList.Add((DIBZ.Common.Model.Swap)item.FirstOrDefault());
            }
            IEnumerable<DIBZ.Common.Model.Offer> offers = null;
            var offerData = new List<OfferData>();
            int skipRecords = (currentPage - 1) * pageSize;

            foreach (var item in swapList)
            {
                offers = await Db.Query<DIBZ.Common.Model.Offer>(o => item.OfferId == o.Id && o.IsActive &&
                         !o.IsDeleted && (o.OfferStatus == OfferStatus.PaymentNeeded || o.OfferStatus == OfferStatus.Accept || o.OfferStatus == OfferStatus.Pending || o.OfferStatus == OfferStatus.Reject)).Preload(o => o.GameCatalog).QueryAsync();


                offerData.AddRange(offers.Select(o => new OfferData
                {
                    OfferId = o.Id,
                    GameName = o.GameCatalog.Name,
                    GameId = o.GameCatalogId,
                    AppUserId = o.ApplicationUserId,
                    AppUserFullName = string.Concat(o.ApplicationUser.FirstName, " ", o.ApplicationUser.LastName),
                    NickName = o.ApplicationUser.NickName,
                    FirstName = o.ApplicationUser.FirstName,
                    LastName = o.ApplicationUser.LastName,
                    GameImageId = o.GameCatalog.GameImageId,
                    ReturnGameId = (o.ReturnGameCatalogId.HasValue) ? o.ReturnGameCatalogId : null,
                    GameFormat = o.GameCatalog.Format.Name,
                    OfferedTime = DIBZ.Common.ConversionHelper.ConvertDateToTimeZone(o.CreatedTime),
                    NoOfInterested = o.CounterOffers.Count,
                    GameCategory = o.GameCatalog.Category.Name,
                    ReturnGameName = (o.ReturnGameCatalogId.HasValue) ? o.ReturnGameCatalog.Name : string.Empty,
                    ReturnGameFormat = (o.ReturnGameCatalogId.HasValue) ? o.ReturnGameCatalog.Format.Name : string.Empty,
                    Status = item.SwapStatus.ToString()
                }).ToList());

            }


            if (isLatestFirst == true)
            {
                if (isCount == true)
                    return offerData.OrderByDescending(o => o.OfferedTime).ToList();
                else
                    return offerData.OrderByDescending(o => o.OfferedTime).Skip(skipRecords).Take(pageSize).ToList();
            }
            else
            {
                return offerData.Skip(skipRecords).Take(pageSize).ToList();
            }
        }

        public async Task<List<OfferData>> GetAllCounterOffersOfApplicationUser(int applicationUserId, int currentPage, int pageSize, bool isLatestFirst, bool isCount)
        {
            int skipRecords = (currentPage - 1) * pageSize;
            var gameCatalogLogic = LogicContext.Create<GameCatalogLogic>();
            //&& o.OfferStatus == OfferStatus.Pending
            var myGames = (await gameCatalogLogic.GetAllGamesOfApplicationUser(applicationUserId)).Where(o => o.IsValidForOffer).Select(o => o.GameId).ToList();
            var myCounterOffers = await Db.Query<DIBZ.Common.Model.CounterOffer>(o => o.CounterOfferPersonId == applicationUserId && (o.Offer.OfferStatus == OfferStatus.Pending || o.Offer.OfferStatus == OfferStatus.NotAvailable) &&
            !o.Offer.IsDeleted && o.IsActive && !o.IsDeleted && o.Offer.ReturnGameCatalogId.HasValue).QueryAsync();
            var offerData = myCounterOffers.Select(t => new OfferData
            {
                GameName = t.Offer.GameCatalog.Name,
                GameId = t.Offer.GameCatalogId,
                AppUserId = t.Offer.ApplicationUserId,
                AppUserFullName = string.Concat(t.Offer.ApplicationUser.FirstName, " ", t.Offer.ApplicationUser.LastName),
                NickName = t.Offer.ApplicationUser.NickName,
                GameImageId = t.Offer.GameCatalog.GameImageId,
                ReturnGameId = t.Offer.ReturnGameCatalogId,
                GameFormat = t.Offer.GameCatalog.Format.Name,
                OfferedTime = DIBZ.Common.ConversionHelper.ConvertDateToTimeZone(t.CreatedTime),
                GameCategory = t.Offer.GameCatalog.Category.Name,
                OfferId = t.Offer.Id,
                ProfileImageId = t.Offer.ApplicationUser.ProfileImageId,
                ReturnGameFormat = t.Offer.ReturnGameCatalog.Format.Name,
                ReturnGameName = t.Offer.ReturnGameCatalog.Name,
                IsCounterOfferMade = true,
                CounterOfferId = t.Id,
                OfferStatus = t.Offer.OfferStatus,
                IsPaymentRequired = !t.Offer.Transactions.Any(p => p.ApplicationUserId == applicationUserId)

            }).ToList();

            if (isLatestFirst == true)
            {
                if (isCount == true)
                    return offerData.OrderByDescending(o => o.OfferedTime).ToList();
                else
                    return offerData.OrderByDescending(o => o.OfferedTime).Skip(skipRecords).Take(pageSize).ToList();
            }
            else
            {
                return offerData.Skip(skipRecords).Take(pageSize).ToList();
            }
        }

        public async Task<int> GetExistingOffers(int appUserId, int offerGameId, int desireGameId)
        {
            IEnumerable<DIBZ.Common.Model.Offer> Offers = null;
            Offers = await Db.Query<DIBZ.Common.Model.Offer>(o => o.ApplicationUserId == appUserId && o.OfferStatus == OfferStatus.Pending && o.IsActive &&
                        !o.IsDeleted && o.GameCatalogId == offerGameId && o.ReturnGameCatalogId == desireGameId).QueryAsync();

            var offerData = Offers.Count();
            return offerData;
        }

        public async Task GetAllOfferByGameAndApplicationUser(int applicationUserId, int gameId)
        {
            var offers = await Db.Query<DIBZ.Common.Model.Offer>(o => o.ApplicationUserId == applicationUserId && o.GameCatalogId == gameId && o.OfferStatus == OfferStatus.Pending && o.IsActive && !o.IsDeleted).QueryAsync();
            //Db.RemoveAll(offers);

            foreach (var item in offers)
            {
                item.OfferStatus = OfferStatus.NotAvailable;
            }
            await Db.SaveAsync();
        }

        public async Task UpdateOfferStatusToPaymentNeeded(int id)
        {
            var OfferObj = await Db.GetObjectById<DIBZ.Common.Model.Offer>(id);
            OfferObj.OfferStatus = OfferStatus.PaymentNeeded;
            await Db.SaveAsync();

        }
        public async Task UpdateOfferStatusToAccept(int id)
        {
            var OfferObj = await Db.GetObjectById<DIBZ.Common.Model.Offer>(id);
            OfferObj.OfferStatus = OfferStatus.Accept;
            await Db.SaveAsync();

        }
        public async Task UpdateOfferStatusToPending(int id)
        {
            var OfferObj = await Db.GetObjectById<DIBZ.Common.Model.Offer>(id);
            OfferObj.OfferStatus = OfferStatus.Pending;
            await Db.SaveAsync();

        }

        public async Task<DIBZ.Common.DTO.Swap> GetOfferDetailById(int id)
        {
            return (await Db.Query<DIBZ.Common.Model.Offer>(o => o.Id == id).QueryAsync()).Select(o => new DIBZ.Common.DTO.Swap
            {
                OfferedGame = o.GameCatalog.Name,
                ReturnedGame = o.ReturnGameCatalog.Name,
                OfferedBy = o.ApplicationUser.NickName,
                GameSwapPersonNickName = o.Swaps.Select(e => e.GameSwapPserson.NickName).FirstOrDefault(),
                SwapDate = o.CreatedTime,
                SwapStatus = (SwapStatus)o.Swaps.Select(e => e.SwapStatus).LastOrDefault(),
                OfferedGameFormat = o.GameCatalog.Format.Name,
                ReturnedGameFormat = o.ReturnGameCatalog.Format.Name,
                GameOffererDFOM = o.GameOffererDFOM,
                GameSwapperDFOM = o.GameSwapperDFOM

            }).FirstOrDefault();

        }

        public List<Page> PagingLogic(int pageIndex, int pageSize, int totalRows)
        {
            //divPageNum.Visible = true;
            int totalPages = totalRows / pageSize;
            if ((totalRows % pageSize) != 0)
            {
                totalPages += 1;
            }
            if (totalPages > 1)
            {
                //divPageNum.Visible = true;

            }
            else
            {
                //divPageNum.Visible = false;
            }

            List<Page> pages = new List<Page>();

            if (totalPages > 1)
            {
                int showMax = 4;
                int startPage;
                int endPage;
                if (totalPages <= showMax)
                {
                    startPage = 1;
                    endPage = totalPages;
                }
                else
                {
                    startPage = (((pageIndex) / showMax) * showMax) + 1;

                    if ((startPage + showMax - 1) > totalPages)
                        endPage = totalPages;
                    else
                        endPage = startPage + showMax - 1;
                }
                if (((pageIndex) / showMax) > 0)
                    pages.Add(new Page((startPage - 1), pageSize, totalRows, "previous"));

                for (int i = startPage; i <= endPage; i++)
                {
                    pages.Add(new Page(i, pageSize, totalRows, i.ToString()));
                    //pages.Add(new ListItem(i.ToString(), i.ToString(), i != (pageIndex + 1)));
                }
                if ((endPage) < totalPages && (pageIndex + showMax) != totalPages)
                    pages.Add(new Page((endPage + 1), pageSize, totalRows, "next"));
                //pages.Add(new ListItem("...", (endPage + 1).ToString(), pageIndex + 1 < totalPages));
            }

            return pages;
        }
        public async Task<int> GetSearchOffersTotalCount(SearchOffer Data)
        {
            IEnumerable<DIBZ.Common.Model.Offer> Offers = null;
            Offers = await Db.Query<DIBZ.Common.Model.Offer>(o => o.OfferStatus == OfferStatus.Pending && o.GameCatalog.Name.ToLower().Contains(Data.GameName.ToLower().Trim()) &&
            o.GameCatalog.FormatId == (Data.FormatId == 0 ? o.GameCatalog.FormatId : Data.FormatId) && o.GameCatalog.CategoryId == (Data.CategoryId == 0 ? o.GameCatalog.CategoryId : Data.CategoryId) &&
            o.IsActive && !o.IsDeleted).OrderByDescending(g => g.CreatedTime).QueryAsync();

            return Offers.Count();
        }

        public async Task<DIBZ.Common.Model.GameCatalog> GetSelectedGameById(int? id)
        {
            return (await Db.Query<DIBZ.Common.Model.GameCatalog>(c => !c.IsDeleted && c.IsActive && c.Id == id).QueryAsync()).FirstOrDefault();
        }

        public DIBZ.Common.Model.Offer EditOffer(int id, int gameId, int returnGameId)
        {
            DIBZ.Common.Model.Offer offer = GetOfferByOfferId(id);
            offer.GameCatalogId = gameId;
            offer.ReturnGameCatalogId = returnGameId;
            Db.Save();
            return offer;
        }

        public DIBZ.Common.Model.Offer GetOfferByOfferId(int id)
        {
            return Db.Query<DIBZ.Common.Model.Offer>(o => o.Id == id).FirstOrDefault();
        }
    }
}

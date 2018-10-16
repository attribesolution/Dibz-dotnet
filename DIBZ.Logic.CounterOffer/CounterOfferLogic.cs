using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIBZ.Common.Model;
using DIBZ.Data;

namespace DIBZ.Logic.CounterOffer
{
    public class CounterOfferLogic:BaseLogic
    {
        static DateTime lastDateCounterWasReset = DateTime.Now;
        static int counter = 0;

        public static int GetCounter()
        {
            if (DateTime.Now.Date != lastDateCounterWasReset)
            {
                lastDateCounterWasReset = DateTime.Now.Date;
                counter = 0;
            }
            return ++counter;
        }

        public CounterOfferLogic(LogicContext context) : base(context)
        {
        }
        public async Task<IEnumerable<DIBZ.Common.Model.CounterOffer>> GetAllCounterOffers(int offerId)
        {
            var counterOffers = await Db.Query<DIBZ.Common.Model.CounterOffer>(o =>o.OfferId==offerId && o.IsActive && !o.IsDeleted).OrderByDescending(o=>o.CreatedTime).QueryAsync();
            
            return counterOffers;
        }
        public async Task<IEnumerable<DIBZ.Common.Model.CounterOffer>> GetAllCounterOffers(List<int> offerIds)
        {
            var counterOffers = await Db.Query<DIBZ.Common.Model.CounterOffer>(o =>offerIds.Contains(o.OfferId) && o.IsActive && !o.IsDeleted).OrderByDescending(o => o.CreatedTime).QueryAsync();

            return counterOffers;
        }
        public async Task<DIBZ.Common.Model.CounterOffer> GetCounterOfferById(int id)
        {
            return (await Db.Query<DIBZ.Common.Model.CounterOffer>(o => o.Id == id && o.IsActive && !o.IsDeleted).QueryAsync()).FirstOrDefault();
        }
        public async Task<DIBZ.Common.Model.CounterOffer> GetCounterOfferByOfferId(int offerId)
        {
            return (await Db.Query<DIBZ.Common.Model.CounterOffer>(o => o.OfferId == offerId && o.IsActive && !o.IsDeleted).QueryAsync()).FirstOrDefault();
        }
        public async Task<DIBZ.Common.Model.Swap> CreateDeal(int id)
        {
            var counterOffer = await Db.GetObjectById<DIBZ.Common.Model.CounterOffer>(id);

            Scorecard offerCreaterSC = await Db.GetObjectById<DIBZ.Common.Model.Scorecard>(counterOffer.Offer.ApplicationUserId);
            if (offerCreaterSC == null)
            {
                offerCreaterSC = new Scorecard();
                offerCreaterSC.ApplicationUserId = counterOffer.Offer.ApplicationUserId;
                offerCreaterSC.Proposals += 1;
                Db.Add(offerCreaterSC);
            }
            else
            {
                offerCreaterSC.ApplicationUserId = counterOffer.Offer.ApplicationUserId;
                offerCreaterSC.Proposals += 1;
            }
           

            Scorecard offerWantSC = await Db.GetObjectById<DIBZ.Common.Model.Scorecard>(counterOffer.CounterOfferPersonId);
            if (offerWantSC == null)
            {
                offerWantSC = new Scorecard();
                offerWantSC.ApplicationUserId = counterOffer.CounterOfferPersonId;
                offerWantSC.Proposals += 1;
                Db.Add(offerWantSC);
            }
            else
            {
                offerWantSC.ApplicationUserId = counterOffer.CounterOfferPersonId;
                offerWantSC.Proposals += 1;
            }
            

            DIBZ.Common.Model.Swap swap = new DIBZ.Common.Model.Swap();
            swap.OfferId = counterOffer.OfferId;
            swap.IsActive = true;
            swap.IsDeleted = false;
            swap.GameSwapPsersonId = counterOffer.CounterOfferPersonId;
            swap.GameSwapWithId = counterOffer.GameCounterOfferWithId;
            swap.SwapStatus = SwapStatus.Accepted;
            
            Db.Add(swap);

            var offer = await Db.GetObjectById<DIBZ.Common.Model.Offer>(counterOffer.OfferId);
            offer.OfferStatus = OfferStatus.Accept;
            
            await Db.SaveAsync();
            await MarkOtherOffersNotAvailable(offer);

            //handling impact 
            await HandlingOfferedImpact(counterOffer);
            return swap;
        }
        public async Task<DIBZ.Common.Model.Swap> CreateDeal(int offerID, int gameSwapPsersonId, int gameSwapWithId)
        {
            var counterOffer = await GetCounterOfferByOfferId(offerID);

            Scorecard offerCreaterSC = await Db.GetObjectById<DIBZ.Common.Model.Scorecard>(counterOffer.Offer.ApplicationUserId);
            if (offerCreaterSC == null)
            {
                offerCreaterSC = new Scorecard();
                offerCreaterSC.ApplicationUserId = counterOffer.Offer.ApplicationUserId;
                offerCreaterSC.Proposals += 1;
                Db.Add(offerCreaterSC);
            }
            else
            {
                offerCreaterSC.ApplicationUserId = counterOffer.Offer.ApplicationUserId;
                offerCreaterSC.Proposals += 1;
            }


            Scorecard offerWantSC = await Db.GetObjectById<DIBZ.Common.Model.Scorecard>(counterOffer.CounterOfferPersonId);
            if (offerWantSC == null)
            {
                offerWantSC = new Scorecard();
                offerWantSC.ApplicationUserId = counterOffer.CounterOfferPersonId;
                offerWantSC.Proposals += 1;
                Db.Add(offerWantSC);
            }
            else
            {
                offerWantSC.ApplicationUserId = counterOffer.CounterOfferPersonId;
                offerWantSC.Proposals += 1;
            }

            DIBZ.Common.Model.Swap swap = new DIBZ.Common.Model.Swap();
            swap.OfferId = offerID;
            swap.IsActive = true;
            swap.IsDeleted = false;
            swap.GameSwapPsersonId = gameSwapPsersonId;
            swap.GameSwapWithId = gameSwapWithId;
            swap.SwapStatus = SwapStatus.Accepted;
            swap.GamerOffererDFOM = this.DFOMCodeForGameOfferer();
            swap.GamerSwapperDFOM = this.DFOMCodeForGameSwapper();

            Db.Add(swap);

            var offer = await Db.GetObjectById<DIBZ.Common.Model.Offer>(offerID);
            offer.OfferStatus = OfferStatus.Accept;
            offer.GameOffererDFOM = this.DFOMCodeForGameOfferer();
            offer.GameSwapperDFOM = this.DFOMCodeForGameSwapper();
            await Db.SaveAsync();
            await MarkOtherOffersNotAvailable(offer);
            //handling impact 
            await HandlingOfferedImpact(counterOffer);
            return swap;
        }
        public async Task<DIBZ.Common.Model.Swap> CreateDealWithOfferStatusPaymentNeeded(int offerID, int gameSwapPsersonId, int gameSwapWithId)
        {
            var counterOffer = await GetCounterOfferByOfferId(offerID);

            Scorecard offerCreaterSC = await Db.GetObjectById<DIBZ.Common.Model.Scorecard>(counterOffer.Offer.ApplicationUserId);
            if (offerCreaterSC == null)
            {
                offerCreaterSC = new Scorecard();
                offerCreaterSC.ApplicationUserId = counterOffer.Offer.ApplicationUserId;
                offerCreaterSC.Proposals += 1;
                Db.Add(offerCreaterSC);
            }
            else
            {
                offerCreaterSC.ApplicationUserId = counterOffer.Offer.ApplicationUserId;
                offerCreaterSC.Proposals += 1;
            }


            Scorecard offerWantSC = await Db.GetObjectById<DIBZ.Common.Model.Scorecard>(counterOffer.CounterOfferPersonId);
            if (offerWantSC == null)
            {
                offerWantSC = new Scorecard();
                offerWantSC.ApplicationUserId = counterOffer.CounterOfferPersonId;
                offerWantSC.Proposals += 1;
                Db.Add(offerWantSC);
            }
            else
            {
                offerWantSC.ApplicationUserId = counterOffer.CounterOfferPersonId;
                offerWantSC.Proposals += 1;
            }

            DIBZ.Common.Model.Swap swap = new DIBZ.Common.Model.Swap();
            swap.OfferId = offerID;
            swap.IsActive = true;
            swap.IsDeleted = false;
            swap.GameSwapPsersonId = gameSwapPsersonId;
            swap.GameSwapWithId = gameSwapWithId;
            swap.SwapStatus = SwapStatus.Accepted;
            swap.GamerOffererDFOM = this.DFOMCodeForGameOfferer();
            swap.GamerSwapperDFOM = this.DFOMCodeForGameSwapper();

            Db.Add(swap);
            await Db.SaveAsync();

            var offer = await Db.GetObjectById<DIBZ.Common.Model.Offer>(offerID);
            offer.OfferStatus = OfferStatus.PaymentNeeded;
            offer.GameOffererDFOM = this.DFOMCodeForGameOfferer();
            offer.GameSwapperDFOM = this.DFOMCodeForGameSwapper();
            await Db.SaveAsync();

            await Task.Delay(2000);
            await MarkOtherOffersNotAvailable(offer);
            //handling impact 
            await HandlingOfferedImpact(counterOffer);
            return swap;
        }
        private string DFOMCodeForGameOfferer()
        {
            return string.Concat( DateTime.Now.ToString("ddMMyyyy"),"-", GetCounter().ToString().PadLeft(5, '0'), "-",2);
        }
        private string DFOMCodeForGameSwapper()
        {
            return string.Concat(DateTime.Now.ToString("ddMMyyyy"), "-", GetCounter().ToString().PadLeft(5, '0'), "-", 1);
        }
        private async Task HandlingOfferedImpact(DIBZ.Common.Model.CounterOffer counterOffer)
        {
            //Creating new offer for counter person's game
            DIBZ.Common.Model.Offer impactOffer = new DIBZ.Common.Model.Offer
            {

                OfferStatus = OfferStatus.Accept,
                GameCatalogId = counterOffer.GameCounterOfferWithId,
                ApplicationUserId = counterOffer.CounterOfferPersonId,
                ReturnGameCatalogId = counterOffer.Offer.GameCatalogId,
                IsActive = true,
            };
            Db.Add(impactOffer);
            //Creating new Swap for Counter Person's game
            DIBZ.Common.Model.Swap impactSwap = new DIBZ.Common.Model.Swap
            {
                OfferId = impactOffer.Id,
                IsActive = true,
                IsDeleted = false,
                GameSwapPsersonId = counterOffer.Offer.ApplicationUserId,
                GameSwapWithId = impactOffer.ReturnGameCatalogId.Value,
                SwapStatus = SwapStatus.Accepted
            };
            await Db.SaveAsync();
            await MarkOtherOffersNotAvailable(impactOffer);

        }
        private async Task MarkOtherOffersNotAvailable(DIBZ.Common.Model.Offer offer)
        {
           var notAvailableOffers= await Db.Query<DIBZ.Common.Model.Offer>(o => o.GameCatalogId == offer.GameCatalogId && o.OfferStatus == OfferStatus.Pending && o.ApplicationUserId == offer.ApplicationUserId).QueryAsync();
            foreach (var item in notAvailableOffers)
            {
                item.OfferStatus = OfferStatus.NotAvailable;
            }
            await Db.SaveAsync();
        }
        public async Task<DIBZ.Common.Model.CounterOffer> AddCounterOffer(int offerId, int gameCounterOfferId, int counterOfferPersonId)
        {
            DIBZ.Common.Model.CounterOffer counterOffer = new DIBZ.Common.Model.CounterOffer();
            counterOffer.OfferId = offerId;
            counterOffer.GameCounterOfferWithId = gameCounterOfferId;
            counterOffer.CounterOfferPersonId = counterOfferPersonId;
            counterOffer.IsActive = true;
            counterOffer.IsDeleted = false;
            Db.Add(counterOffer);
            await Db.SaveAsync();
            return counterOffer;
        }
        public async Task Delete(int id)
        {
            var counterOfferObj = await Db.GetObjectById<DIBZ.Common.Model.CounterOffer>(id);
            Db.Remove(counterOfferObj);
            await Db.SaveAsync();

        }
    }
}

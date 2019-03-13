using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DIBZ.Base;
using DIBZ.Common.DTO;
using DIBZ.Logic;
using DIBZ.Logic.Offer;
using DIBZ.Logic.Notification;
using Newtonsoft.Json;
using DIBZ.Logic.GameCatalog;
using DIBZ.Common;
using DIBZ.Logic.Auth;
using DIBZ.Filters;
using DIBZ.Logic.Swap;
using DIBZ.Logic.Transaction;
using DIBZ.Logic.CounterOffer;
using DIBZ.Common.Model;
using System.Configuration;
using DIBZ.Data;
using System.Text;

namespace DIBZ.Controllers
{
    public class OfferController : BaseWebController
    {
        string hostName = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

        //Variable Declarations
        IEnumerable<DIBZ.Common.Model.Offer> offers = new List<DIBZ.Common.Model.Offer>();
        List<DIBZ.Common.DTO.OfferModel> offerList = new List<DIBZ.Common.DTO.OfferModel>();

        [HttpGet]
        [AuthOp(LoggedInUserOnly = true)]
        // GET: MyProfile
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public async Task<ActionResult> Index()
        {
            var gameCatalogLogic = LogicContext.Create<GameCatalogLogic>();
            ViewBag.MyGames = await gameCatalogLogic.GetAllGamesOfApplicationUser(CurrentLoginSession.ApplicationUserId.GetValueOrDefault());
            ViewBag.GameCollection = await gameCatalogLogic.GetAllGameCatalog();
            return View();
        }

        public async Task<ActionResult> _SearchOffer()
        {
            var formatLogic = LogicContext.Create<FormatLogic>();
            var format = await formatLogic.GetAllFormats();
            ViewBag.Formats = new SelectList(format, "Id", "Name");

            var categoryLogic = LogicContext.Create<CategoryLogic>();
            var categories = await categoryLogic.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return PartialView("~/Views/Offer/_SearchOffer.cshtml");
        }

        [AuthOp(LoggedInUserOnly = true)]
        // GET: MyProfile
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public async Task<ActionResult> MyOffers(int currentPage = 1, bool isLatestFirst = true, int pageSize = 5)
        {
            ViewBag.PageSize = pageSize;
            ViewBag.Sorting = isLatestFirst;

            var gameCatalogLogic = LogicContext.Create<GameCatalogLogic>();
            ViewBag.MyGames = await gameCatalogLogic.GetAllGamesOfApplicationUser(CurrentLoginSession.ApplicationUserId.GetValueOrDefault());
            ViewBag.GameCollection = await gameCatalogLogic.GetAllGameCatalog();

            var offerLogic = LogicContext.Create<OfferLogic>();
            var myOffersCount = await offerLogic.GetAllOffersPaymentOfApplicationUser(CurrentLoginSession.ApplicationUserId.GetValueOrDefault(), 1, 5, true, true);

            var Pages = offerLogic.PagingLogic(currentPage - 1, pageSize, Convert.ToInt32(myOffersCount.Count));
            ViewBag.Pages = Pages;
            ViewBag.SelectedPage = currentPage;

            var myOffers = await offerLogic.GetAllOffersPaymentOfApplicationUser(CurrentLoginSession.ApplicationUserId.GetValueOrDefault(), currentPage, pageSize, isLatestFirst, false);
            return View(myOffers);
        }

        [AuthOp(LoggedInUserOnly = true)]
        // GET: MyProfile
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public async Task<ActionResult> MyAllOffers(int currentPage = 1, bool isLatestFirst = true, int pageSize = 5)
        {           
            ViewBag.PageSize = pageSize;
            ViewBag.Sorting = isLatestFirst;

            var gameCatalogLogic = LogicContext.Create<GameCatalogLogic>();
            ViewBag.MyGames = await gameCatalogLogic.GetAllGamesOfApplicationUser(CurrentLoginSession.ApplicationUserId.GetValueOrDefault());
            ViewBag.GameCollection = await gameCatalogLogic.GetAllGameCatalog();

            var offerLogic = LogicContext.Create<OfferLogic>();
            var myOffersCount = await offerLogic.GetAllOffersOfApplicationUser(CurrentLoginSession.ApplicationUserId.GetValueOrDefault(), 1, 5, true, true);

            var Pages = offerLogic.PagingLogic(currentPage - 1, pageSize, Convert.ToInt32(myOffersCount.Count));
            ViewBag.Pages = Pages;
            ViewBag.SelectedPage = currentPage;

            var myOffers = await offerLogic.GetAllOffersOfApplicationUser(CurrentLoginSession.ApplicationUserId.GetValueOrDefault(), currentPage, pageSize, isLatestFirst, false);
            return View(myOffers);
        }

        [AuthOp(LoggedInUserOnly = true)]
        public async Task<ActionResult> CheckUserProfile()
        {
            var authLogic = LogicContext.Create<AuthLogic>();
            try
            {

                var authLogic1 = LogicContext.Create<AuthLogic>();
                var userData = await authLogic.GetApplicationUserById(CurrentLoginSession.ApplicationUserId.GetValueOrDefault());
                if (userData != null)
                {
                    if (userData.Address == null || userData.FirstName == null || userData.LastName == null || userData.CellNo == null)
                    {
                       return Json(new { IsSuccess = false, msg = "Please completed your profile first" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { IsSuccess = true, msg = "Success" }, JsonRequestBehavior.AllowGet);
                    }
                }
               
            }
            catch (Exception lex)
            {
                return Json(new { IsSuccess = false, msg = lex.Message }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { IsSuccess = false, msg = "Please completed your profile first" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AuthOp(LoggedInUserOnly = true)]
        public async Task<ActionResult> CreateOffer(string gameOfferId, string gameInReturnId)
        {
            
            DIBZDbContext context = new DIBZDbContext();
            var notificationLogic = LogicContext.Create<NotificationLogic>();
            var AuthLogic = LogicContext.Create<AuthLogic>();
            var GameCatalog = LogicContext.Create<GameCatalogLogic>();
            var emailTemplateLogic = LogicContext.Create<EmailTemplateLogic>();

            List<DIBZ.Common.DTO.NotificationModel> notifications = new List<Common.DTO.NotificationModel>();
            DIBZ.Common.Model.GameCatalog gameCatalog = new DIBZ.Common.Model.GameCatalog();
            IEnumerable<DIBZ.Common.Model.ApplicationUser> allApplicationUsers = new List<DIBZ.Common.Model.ApplicationUser>();
            DIBZ.Common.Model.EmailTemplate emailTemplate = new DIBZ.Common.Model.EmailTemplate();
            EmailTemplateHelper templates = new EmailTemplateHelper();
            EmailTemplateResponse emailTemplateResponce = new EmailTemplateResponse();
            DIBZ.Common.Model.EmailNotification email = new DIBZ.Common.Model.EmailNotification();
            List<int> offeredGameIds = gameOfferId.Split(',').Select(int.Parse).ToList();
            List<int> returnedGameIds = new List<int>();
            foreach (var offeredGameId in offeredGameIds)
            {
                AuthLogic.AddGameIntoCollection(CurrentLoginSession.ApplicationUser.Id, offeredGameId.ToString());
            }

            if (!string.IsNullOrEmpty(gameInReturnId))
            {
                returnedGameIds = gameInReturnId.Split(',').Select(int.Parse).ToList();
            }

            var offerLogic = LogicContext.Create<OfferLogic>();
            try
            {
                List<DIBZ.Common.Model.Offer> allOffers = new List<Common.Model.Offer>();
                foreach (var offeredGameId in offeredGameIds)
                {
                    if (returnedGameIds.Count > 0)
                    {
                        foreach (var returnedGameId in returnedGameIds)
                        {
                            allOffers.AddRange(CreateOfferForEachSelectedGameInReturn(offeredGameIds, returnedGameId));
                        }
                    }
                    else
                    {
                        OfferModel offerRequest = new OfferModel();
                        offerRequest.ApplicationUserId = CurrentLoginSession.ApplicationUserId.GetValueOrDefault();
                        offerRequest.GameCatalogId = Convert.ToInt32(offeredGameIds);
                        offerLogic.AddUpdateOffer(offerRequest);
                    }

                    if (returnedGameIds.Count > 0)
                    {
                        NotificationModel notificationModel = new NotificationModel();
                        foreach (var offerId in allOffers)
                        {

                            gameCatalog = await GameCatalog.GetGameCatalogById(offerId.ReturnGameCatalogId.Value);
                            //get all AppUsers By GameId
                            allApplicationUsers = await AuthLogic.GetApplicationUserByGameId(offerId.ReturnGameCatalogId.Value);

                            if (allApplicationUsers.Count() > 0)
                            {
                                foreach (var applicationUser in allApplicationUsers)
                                {
                                    // we dont want to notify that user who create this offer
                                    if (applicationUser.Id != CurrentLoginSession.ApplicationUserId)
                                    {

                                        var additionalData = new { GameCatalogId = offeredGameIds, ReturnGameCatalogId = offerId.ReturnGameCatalogId.Value, ReturnGameImageId = gameCatalog.GameImageId, OfferId = offerId.Id };
                                        notificationModel.AdditionalData = Helpers.GetJson(additionalData);
                                        notificationModel.AppUserId = Convert.ToInt32(applicationUser.Id);
                                        //Channel like Android,Ios,Web
                                        notificationModel.Channel = Convert.ToInt32(DIBZ.Common.Model.Channel.Web);
                                        notificationModel.Content = "An offer is created for game " + gameCatalog.Name + "";
                                        notificationModel.CreatedTime = DateTime.Now;
                                        notificationModel.LastError = "";
                                        notificationModel.OfferIds = allOffers.Select(o => o.Id).ToList();
                                        notificationModel.Status = Convert.ToInt32(DIBZ.Common.Model.NotificationStatus.Unseen);
                                        notificationModel.Title = "Create Offer";
                                        notificationModel.NotificationType = Convert.ToInt32(DIBZ.Common.Model.NotificationType.Desktop);
                                        notificationModel.NotificationBusinessType = Convert.ToInt32(DIBZ.Common.Model.NotificationBusinessType.CreateOffer);
                                        //save notification in notification table
                                        var notification = await notificationLogic.AddNotification(notificationModel);

                                        // sent notification of offer creater to all users who have that game
                                        new DIBZ.Services.ServerNotificationService().CreateOffer(applicationUser.Id, notification.Id, notificationModel.Content, notificationModel.CreatedTime, notificationModel.AdditionalData);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex.Message, ex);
            }
            return RedirectToAction("MyAllOffers", "Offer");
        }
        private List<DIBZ.Common.Model.Offer> CreateOfferForEachSelectedGameInReturn(List<int> offeredGameIds, int returnedGameId)
        {
            List<DIBZ.Common.Model.Offer> allOffers = new List<Common.Model.Offer>();
            var offerLogic = LogicContext.Create<OfferLogic>();
            foreach (var offeredGameId in offeredGameIds)
            {
                OfferModel offerRequest = new OfferModel();
                offerRequest.ApplicationUserId = CurrentLoginSession.ApplicationUserId.GetValueOrDefault();
                offerRequest.GameCatalogId = offeredGameId;
                offerRequest.AgaintsGameCatalogId = returnedGameId;

                var myExistingOffersCount = offerLogic.GetExistingOffers(CurrentLoginSession.ApplicationUserId.GetValueOrDefault(), offeredGameId, returnedGameId);
                if (myExistingOffersCount.Result.CompareTo(0) == 0)
                {
                    allOffers.Add(offerLogic.AddUpdateOffer(offerRequest));
                }
            }
            return allOffers;
        }
        private List<DIBZ.Common.Model.Offer> CreateOfferForEachSelectedGameInReturn(int offeredGameId, int returnedGameId)
        {
            List<DIBZ.Common.Model.Offer> allOffers = new List<Common.Model.Offer>();
            var offerLogic = LogicContext.Create<OfferLogic>();
            OfferModel offerRequest = new OfferModel();
            offerRequest.ApplicationUserId = CurrentLoginSession.ApplicationUserId.GetValueOrDefault();
            offerRequest.GameCatalogId = offeredGameId;
            offerRequest.AgaintsGameCatalogId = returnedGameId;
            allOffers.Add(offerLogic.AddUpdateOffer(offerRequest));
            return allOffers;
        }

        [AuthOp(LoggedInUserOnly = true)]
        // GET: MyProfile
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public async Task<ActionResult> DeleteOffer(int id)
        {
            var offerLogic = LogicContext.Create<OfferLogic>();
            await offerLogic.Delete(id);

            var gameCatalogLogic = LogicContext.Create<GameCatalogLogic>();
            ViewBag.MyGames = await gameCatalogLogic.GetAllGamesOfApplicationUser(CurrentLoginSession.ApplicationUserId.GetValueOrDefault());
            ViewBag.GameCollection = await gameCatalogLogic.GetAllGameCatalog();

            var myOffers = await offerLogic.GetAllOffersOfApplicationUser(CurrentLoginSession.ApplicationUserId.GetValueOrDefault(), Convert.ToInt16(PageSize.FirstPage), Convert.ToInt16(PageSize.DefaultPageSize), true, false);
            return View("MyOffers", myOffers);
        }


        [AuthOp(LoggedInUserOnly = true)]
        // GET: MyProfile
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public async Task<ActionResult> ViewOfferDetail(int id)
        {
            DIBZ.Common.Model.Offer offer = new DIBZ.Common.Model.Offer();
            var offerLogic = LogicContext.Create<OfferLogic>();
            offer = await offerLogic.GetOfferById(id);
            return Json(new { IsSuccess = true, OfferedGame = offer.GameCatalog.Name, OfferedGameImageId = offer.GameCatalog.GameImageId, Status = offer.OfferStatus.ToString(), OfferCreateDate = offer.CreatedTime.ToString("dd/MM/yyyy"), SwapWithGame = (offer.ReturnGameCatalogId.HasValue) ? offer.ReturnGameCatalog.Name : string.Empty }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AuthOp(LoggedInUserOnly = true)]
        // GET: MyProfile
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public async Task<ActionResult> MyGames(int currentPage = 1, string searchGame = "", int formatId = 0, int categoryId = 0)
        {
            var offerlogic = LogicContext.Create<OfferLogic>();
            var gameCatalogLogic = LogicContext.Create<GameCatalogLogic>();
            var myGames = await gameCatalogLogic.GetAllGamesOfApplicationUser(CurrentLoginSession.ApplicationUserId.GetValueOrDefault());
            var pageSize = Convert.ToInt16(PageSize.AllGames);

            if (TempData["searchGamesData"] != null)
            {
                currentPage = Convert.ToInt16(TempData["currentPage"]);
                var searchCount = Convert.ToInt32(TempData["searchCount"]);
                var Pages = offerlogic.PagingLogic(currentPage - 1, pageSize, searchCount);
                ViewBag.Pages = Pages;
                ViewBag.FormatId = TempData["formatId"];
                ViewBag.CategoryId = TempData["categoryId"];
                ViewBag.GameName = TempData["gameName"];
                ViewBag.GameCollection = TempData["searchGamesData"];
                ViewBag.SelectedPage = currentPage;
                ViewBag.pageSize = pageSize;
                ViewBag.CurrentPage = currentPage;
            }
            else
            {
                SearchOffer Search = new SearchOffer();
                Search.GameName = searchGame;
                Search.FormatId = formatId;
                Search.CategoryId = categoryId;

                //var currentPage = selectedPage;
                var countAllGames = await gameCatalogLogic.SearchGameCatalog(CurrentLoginSession.ApplicationUserId.GetValueOrDefault(), Search, currentPage, true);
                var totalCount = Convert.ToInt32(countAllGames.Count);
                var Pages = offerlogic.PagingLogic(currentPage, pageSize, totalCount);
                ViewBag.Pages = Pages;
                ViewBag.FormatId = 0;
                ViewBag.CategoryId = 0;
                ViewBag.GameName = "";
                ViewBag.GameCollection = await gameCatalogLogic.SearchGameCatalog(CurrentLoginSession.ApplicationUserId.GetValueOrDefault(), Search, currentPage, false);
                ViewBag.SelectedPage = currentPage;
                ViewBag.pageSize = pageSize;
                ViewBag.CurrentPage = currentPage;
            }

            var formatLogic = LogicContext.Create<FormatLogic>();
            var formats = await formatLogic.GetAllFormats();
            ViewBag.Formats = new SelectList(formats, "Id", "Name");

            var categoryLogic = LogicContext.Create<CategoryLogic>();
            var categories = await categoryLogic.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");

            return View(myGames);
        }

        [AuthOp(LoggedInUserOnly = true)]
        public async Task<ActionResult> AddGameIntoCollection(string gameIds, int currentPage, string searchGame, int formatId, int categoryId)
        {
            DIBZDbContext context = new DIBZDbContext();
            var authLogic = LogicContext.Create<AuthLogic>();
            try
            {
                var notifierEmail = (from NotifierEmails in context.NotifierEmails
                                     orderby NotifierEmails.CreatedTime descending
                                     select new
                                     {
                                         EmailAddress = NotifierEmails.EmailAddress
                                     }).ToList();

                var userId = CurrentLoginSession.ApplicationUserId.GetValueOrDefault();
                var usersFirstGameData = (from ApplicationUser in context.ApplicationUsers
                                          where ApplicationUser.Id == userId
                                          select new GameData
                                          {
                                              GameName = ApplicationUser.GameCatalogs.FirstOrDefault().Name
                                          }).ToList();
                if (usersFirstGameData.First().GameName == null)
                {
                    // Get UserId, UserName, Game and Format
                    DIBZ.Common.Model.ApplicationUser user = authLogic.GetUserById(userId);
                    var nickName = user.NickName;

                    DIBZ.Common.Model.GameCatalog selectedGame = new DIBZ.Common.Model.GameCatalog();
                    var offerLogic = LogicContext.Create<OfferLogic>();
                    selectedGame = await offerLogic.GetSelectedGameById(Convert.ToInt32(gameIds));
                    var gameName = selectedGame.Name;
                    var gameFormat = selectedGame.Format.Name;

                    if (notifierEmail.Count != 0)
                    {
                        // Email Sending to Specific Email address which is specified by DON.
                        var notifierLastEmail = notifierEmail.FirstOrDefault().EmailAddress;
                        EmailTemplateHelper templates = new EmailTemplateHelper();
                        EmailTemplateResponse emailTemplateResponse = new EmailTemplateResponse();
                        DIBZ.Common.Model.EmailNotification Email = new DIBZ.Common.Model.EmailNotification();

                        var emailTemplateLogic = LogicContext.Create<EmailTemplateLogic>();
                        emailTemplateResponse = await emailTemplateLogic.GetEmailTemplate(EmailType.Email, EmailContentType.UserFirstGame);

                        templates.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, nickName);
                        templates.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name, gameName);
                        templates.AddParam(DIBZ.Common.Model.Contants.GameFormat, gameFormat);

                        var emailBody = templates.FillTemplate(emailTemplateResponse.Body);
                        await emailTemplateLogic.SaveEmailNotification(notifierLastEmail, emailTemplateResponse.Title, emailBody, EmailType.Email, Priority.Low);
                        EmailHelper.Email(notifierLastEmail, emailTemplateResponse.Title, emailBody);
                    }
                }

                authLogic.AddGameIntoCollection(userId, gameIds);
                return RedirectToAction("MyGames", new { currentPage, searchGame, formatId, categoryId });
            }
            catch (Exception lex)
            {
                return Json(new { IsSuccess = false, fail = lex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //[HttpGet]
        [AuthOp(LoggedInUserOnly = true)]
        // GET: MyProfile
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public async Task<ActionResult> MySwaps(int currentPage = 1, int pageSize = 5, bool isLatestFirst = true)
        {
            //ViewBag.PageSize = pageSize;
            //ViewBag.Sorting = isLatestFirst;
            //ViewBag.Login = CurrentLoginSession.ApplicationUserId.GetValueOrDefault();

            //var swapLogic = LogicContext.Create<SwapLogic>();
            //var offerLogic = LogicContext.Create<OfferLogic>();
            //var mySwapsTotalCount = await swapLogic.GetAllOffersOfApplicationUser(CurrentLoginSession.ApplicationUserId.GetValueOrDefault(), currentPage, pageSize, true, true);

            //var Pages = offerLogic.PagingLogic(currentPage - 1, pageSize, Convert.ToInt32(mySwapsTotalCount.Count));
            //ViewBag.Pages = Pages;
            //ViewBag.SelectedPage = currentPage;

            //var mySwaps = await swapLogic.GetAllOffersOfApplicationUser(CurrentLoginSession.ApplicationUserId.GetValueOrDefault(), currentPage, pageSize, false, isLatestFirst);
            //return View(mySwaps);

            ViewBag.PageSize = pageSize;
            ViewBag.Sorting = isLatestFirst;
            ViewBag.Login = CurrentLoginSession.ApplicationUserId.GetValueOrDefault();

            var gameCatalogLogic = LogicContext.Create<GameCatalogLogic>();
            ViewBag.MyGames = (await gameCatalogLogic.GetAllGamesOfApplicationUser(Convert.ToInt32(CurrentLoginSession.ApplicationUserId))).Where(o => o.IsValidForOffer).Select(o => o.GameId).ToList();
            ViewBag.GameCollection = await gameCatalogLogic.GetAllGameCatalog();

            var offerLogic = LogicContext.Create<OfferLogic>();
            // var myOffersCount = await offerLogic.GetAllOffersPaymentOfApplicationUser(CurrentLoginSession.ApplicationUserId.GetValueOrDefault(), 1, 5, true, true);



            var myOffers = await offerLogic.GetAllOffersPaymentOfApplicationUser(CurrentLoginSession.ApplicationUserId.GetValueOrDefault(), currentPage, pageSize, isLatestFirst, false);
            var Pages = offerLogic.PagingLogic(currentPage - 1, pageSize, Convert.ToInt32(myOffers.Count));
            ViewBag.Pages = Pages;
            ViewBag.SelectedPage = currentPage;
            return View(myOffers);
        }

        [HttpGet]
        [AuthOp(LoggedInUserOnly = true)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public async Task<ActionResult> AddUpdate(int Id)
        {
            DIBZ.Common.Model.Offer offer = new DIBZ.Common.Model.Offer();

            if (Id > 0)
            {
                var offerLogic = LogicContext.Create<OfferLogic>();
                offer = await offerLogic.GetOfferById(Id);
                return RedirectToAction("Edit", offer);
            }
            return View("Edit", offer);
        }

        [AuthOp(LoggedInUserOnly = true)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public async Task<ActionResult> OfferDetail(int id)
        {
            var swapLogic = LogicContext.Create<SwapLogic>();
            //DIBZ.Common.Model.ApplicationUser appUser = new DIBZ.Common.Model.ApplicationUser();
            var authLogic = LogicContext.Create<AuthLogic>();
            var swapStatusOfferer = await swapLogic.GetSwapById(id, "Offerer");
            var swapStatusSwapper = await swapLogic.GetSwapById(id, "Swapper");

            ViewBag.SwapStatusOfferer = swapStatusOfferer.SwapStatus == SwapStatus.Accepted ? "Awaiting Payment" : swapStatusOfferer.SwapStatus.ToString().Replace("_", " ");
            ViewBag.SwapStatusSwapper = swapStatusSwapper.SwapStatus == SwapStatus.Accepted ? "Awaiting Payment" : swapStatusSwapper.SwapStatus.ToString().Replace("_", " ");

            var swapStatus = (swapStatusOfferer.Offer.Transactions.Count == 2 &&
                             (swapStatusOfferer.SwapStatus == SwapStatus.Payment_Done_By_Offerer || swapStatusOfferer.SwapStatus == SwapStatus.Payment_Done_By_Swapper))
                             ? SwapStatus.Payment_Successful : swapStatusOfferer.SwapStatus;
            ViewBag.SwapStatus = swapStatus.ToString().Replace("_", " ");
            ViewBag.Login = CurrentLoginSession.ApplicationUserId.GetValueOrDefault() == swapStatusOfferer.GameSwapPsersonId ? "Swapper" : "Offerer";

            DIBZ.Common.Model.Offer offer = new DIBZ.Common.Model.Offer();
            var offerLogic = LogicContext.Create<OfferLogic>();
            if (id > 0)
            {
                offer = await offerLogic.GetOfferById(id);
                var appUser = authLogic.GetUserById(swapStatusOfferer.GameSwapPsersonId);
                ViewBag.SwapWith = appUser.NickName.ToString();
                return View(offer);
            }
            return RedirectToAction("Index", "Myprofile");
        }

        [AuthOp(LoggedInUserOnly = true)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public async Task<ActionResult> PossibleSwaps(int? id = 0, int currentPage = 1, int pageSize = 5, bool isLatestFirst = true, int formatId = 0, string gameName = "", int categoryId = 0)
        {
            var formatLogic = LogicContext.Create<FormatLogic>();
            var formats = await formatLogic.GetAllFormats();
            ViewBag.Formats = new SelectList(formats, "Id", "Name");

            var categoryLogic = LogicContext.Create<CategoryLogic>();
            var categories = await categoryLogic.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");

            ViewBag.PageSize = pageSize;
            ViewBag.Sorting = isLatestFirst;

            SearchOffer SearchRequest = new SearchOffer();

            SearchRequest.GameName = gameName;
            SearchRequest.FormatId = formatId;
            SearchRequest.CategoryId = categoryId;

            ViewBag.GameName = SearchRequest.GameName;
            ViewBag.FormatId = SearchRequest.FormatId;
            ViewBag.CategoryId = SearchRequest.CategoryId;

            var offerLogic = LogicContext.Create<OfferLogic>();

            SearchRequest.AppUserId = CurrentLoginSession.ApplicationUser.Id;
            SearchRequest.AppUserRegisteredTime = CurrentLoginSession.ApplicationUser.CreatedTime;
            SearchRequest.OfferId = id.GetValueOrDefault();

            var possibleSwapDataCount = await offerLogic.ShowPossibleSwapToUser(SearchRequest, 1, 5, true, true);
            var Pages = offerLogic.PagingLogic(currentPage - 1, pageSize, Convert.ToInt32(possibleSwapDataCount.Count));
            ViewBag.Pages = Pages;
            ViewBag.SelectedPage = currentPage;

            var possibleSwapData = await offerLogic.ShowPossibleSwapToUser(SearchRequest, currentPage, pageSize, isLatestFirst);
            return View(possibleSwapData);
        }

        [AuthOp(LoggedInUserOnly = true)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public async Task<ActionResult> AddTransaction(string offerId, string amount, string counterOfferId)
        {
            var transactionLogic = LogicContext.Create<TransactionLogic>();
            var offerLogic = LogicContext.Create<OfferLogic>();
            try
            {
                var offer = await offerLogic.GetOfferById(ConversionHelper.SafeConvertToInt32(offerId));
                var x = await transactionLogic.AddTransaction(ConversionHelper.SafeConvertToInt32(offerId), Convert.ToDecimal(amount), CurrentLoginSession.ApplicationUser.Id, offer.Swaps.FirstOrDefault().Id);


                var isNeedToRedirect = false;
                if (offer.OfferStatus == OfferStatus.PaymentNeeded && offer.Transactions.Count() == 2)
                {
                    await new CounterOfferController().AcceptOfferAfterTransactionDone(ConversionHelper.SafeConvertToInt32(counterOfferId));
                    isNeedToRedirect = true;
                }

                if (x > 0)
                {
                    return Json(new { IsSuccess = true, IsNeedToRedirect = isNeedToRedirect }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { IsSuccess = false, fail = "Some Thing Wrong!" }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception lex)
            {
                return Json(new { IsSuccess = false, fail = lex.Message }, JsonRequestBehavior.AllowGet);
            }

        }
        [AuthOp(LoggedInUserOnly = true)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public async Task<ActionResult> AddPaymentTransaction(string offerId, string amount)
        {
            var transactionLogic = LogicContext.Create<TransactionLogic>();
            var offerLogic = LogicContext.Create<OfferLogic>();
            var emailTemplateLogic = LogicContext.Create<EmailTemplateLogic>();
            DIBZ.Common.Model.EmailTemplate emailTemplate = new DIBZ.Common.Model.EmailTemplate();
            EmailTemplateHelper templates = new EmailTemplateHelper();
            EmailTemplateResponse emailTemplateResponse = new EmailTemplateResponse();
            DIBZ.Common.Model.EmailNotification email = new DIBZ.Common.Model.EmailNotification();
            try
            {
                var offer = await offerLogic.GetOfferById(ConversionHelper.SafeConvertToInt32(offerId));
                var transactionData = await transactionLogic.AddTransaction(ConversionHelper.SafeConvertToInt32(offerId), Convert.ToDecimal(amount), CurrentLoginSession.ApplicationUser.Id, offer.Swaps.FirstOrDefault().Id);

                if (offer.OfferStatus == OfferStatus.PaymentNeeded && offer.Transactions.Count() == 2)
                {
                    await offerLogic.UpdateOfferStatusToAccept(offer.Id);
                }

                if (transactionData > 0)
                {
                    string QRCodeImagePath = QRHelper.GenerateAndSaveQrCodeForOffer(CurrentLoginSession.ApplicationUser.Email, offer.Id, this.Url.Action("ReadQR", "Offer", new { id = offer.Id }, this.Request.Url.Scheme));
                    if (CurrentLoginSession.ApplicationUserId == offer.ApplicationUserId)
                    {
                        emailTemplateResponse = await emailTemplateLogic.GetEmailTemplate(DIBZ.Common.Model.EmailType.Email, DIBZ.Common.Model.EmailContentType.PaymentDone);
                        templates.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, offer.ApplicationUser.NickName);
                        templates.AddParam(DIBZ.Common.Model.Contants.AppUserNickName_Swapper, offer.Swaps.FirstOrDefault().GameSwapPserson.NickName);

                        templates.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name, offer.GameCatalog.Name);
                        templates.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name_Swapper, offer.ReturnGameCatalog.Name);

                        templates.AddParam(DIBZ.Common.Model.Contants.GameFormat, offer.GameCatalog.Format.Name);
                        templates.AddParam(DIBZ.Common.Model.Contants.GameFormatSwapper, offer.ReturnGameCatalog.Format.Name);

                        templates.AddParam(DIBZ.Common.Model.Contants.DFOM_Code, offer.GameOffererDFOM);

                        templates.AddParam(DIBZ.Common.Model.Contants.UrlContactUs, string.Format("<a href='{0}'>here</a>", hostName + "/Dashboard/ContactUs"));
                        var emailBodyOfferrer = templates.FillTemplate(emailTemplateResponse.Body);

                        //save email data in table
                        await emailTemplateLogic.SaveEmailNotification(CurrentLoginSession.ApplicationUser.Email, emailTemplateResponse.Title, emailBodyOfferrer, EmailType.Email, Priority.Low);
                        EmailHelper.EmailAttachement(CurrentLoginSession.ApplicationUser.Email, emailTemplateResponse.Title, emailBodyOfferrer, QRCodeImagePath);
                    }
                    else
                    {
                        EmailTemplateHelper TemplatesSwapper = new EmailTemplateHelper();
                        EmailTemplateResponse emailTemplateResponseSwapper = new EmailTemplateResponse();
                        //get email template
                        emailTemplateResponseSwapper = await emailTemplateLogic.GetEmailTemplate(DIBZ.Common.Model.EmailType.Email, DIBZ.Common.Model.EmailContentType.PaymentDone);
                        TemplatesSwapper.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, CurrentLoginSession.ApplicationUser.NickName);
                        TemplatesSwapper.AddParam(DIBZ.Common.Model.Contants.AppUserNickName_Swapper, offer.ApplicationUser.NickName);

                        TemplatesSwapper.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name, offer.ReturnGameCatalog.Name);
                        TemplatesSwapper.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name_Swapper, offer.GameCatalog.Name);

                        TemplatesSwapper.AddParam(DIBZ.Common.Model.Contants.GameFormat, offer.ReturnGameCatalog.Format.Name);
                        TemplatesSwapper.AddParam(DIBZ.Common.Model.Contants.GameFormatSwapper, offer.GameCatalog.Format.Name);

                        TemplatesSwapper.AddParam(DIBZ.Common.Model.Contants.DFOM_Code, offer.GameSwapperDFOM);

                        TemplatesSwapper.AddParam(DIBZ.Common.Model.Contants.UrlContactUs, string.Format("<a href='{0}'>here</a>", hostName + "/Dashboard/ContactUs"));
                        var emailBody = TemplatesSwapper.FillTemplate(emailTemplateResponseSwapper.Body);

                        //save email data in table
                        await emailTemplateLogic.SaveEmailNotification(CurrentLoginSession.ApplicationUser.Email, emailTemplateResponseSwapper.Title, emailBody, EmailType.Email, Priority.Low);
                        EmailHelper.EmailAttachement(CurrentLoginSession.ApplicationUser.Email, emailTemplateResponseSwapper.Title, emailBody, QRCodeImagePath);
                    }

                    EmailHelper.EmailAttachement(CurrentLoginSession.ApplicationUser.Email, "Transaction From PayPal Account", SendEmailAfterTransaction(amount), string.Empty);

                    return Json(new { IsSuccess = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { IsSuccess = false, fail = "Some Thing Wrong!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception lex)
            {
                return Json(new { IsSuccess = false, fail = lex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [AuthOp(LoggedInUserOnly = true)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public async Task<ActionResult> UpdateOfferStatusToPaymentNeeded(int id)
        {
            var offerLogic = LogicContext.Create<OfferLogic>();
            await offerLogic.UpdateOfferStatusToPaymentNeeded(id);
            return RedirectToAction("PossibleSwaps", "Offer");
        }

        public async Task<ActionResult> ReadQR(int id)
        {
            var offerLogic = LogicContext.Create<OfferLogic>();
            var offerDetail = await offerLogic.GetOfferDetailById(id);
            return View(offerDetail);
        }

        [AuthOp(LoggedInUserOnly = true)]
        public async Task<ActionResult> DeleteMyGameById(int gameId, int currentPage, string searchGame, int formatId, int categoryId)
        {
            var formatLogic = LogicContext.Create<FormatLogic>();
            var formats = await formatLogic.GetAllFormats();
            ViewBag.Formats = new SelectList(formats, "Id", "Name");

            var categoryLogic = LogicContext.Create<CategoryLogic>();
            var categories = await categoryLogic.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");

            var gameCatalogLogic = LogicContext.Create<GameCatalogLogic>();
            await gameCatalogLogic.DeleteMyGameById(CurrentLoginSession.ApplicationUserId.GetValueOrDefault(), gameId);
            return RedirectToAction("MyGames", new { currentPage, searchGame, formatId, categoryId });
        }

        public async Task<ActionResult> SearchGames(int currentPage = 1, int formatId = 0, string gameName = "", int categoryId = 0)
        {
            SearchOffer SearchGames = new SearchOffer();

            SearchGames.GameName = gameName;
            SearchGames.FormatId = formatId;
            SearchGames.CategoryId = categoryId;

            var gameLogic = LogicContext.Create<GameCatalogLogic>();
            var searchGamesData = await gameLogic.SearchGameCatalog(CurrentLoginSession.ApplicationUserId.GetValueOrDefault(), SearchGames, currentPage);
            var searchGamesTotalCount = await gameLogic.SearchGameCatalog(CurrentLoginSession.ApplicationUserId.GetValueOrDefault(), SearchGames, currentPage, true);

            TempData["searchCount"] = searchGamesTotalCount.Count;
            TempData["searchGamesData"] = searchGamesData;
            TempData["currentPage"] = currentPage;
            TempData["gameName"] = SearchGames.GameName;
            TempData["formatId"] = SearchGames.FormatId;
            TempData["categoryId"] = SearchGames.CategoryId;

            string view = Request.UrlReferrer.Segments[2];
            if (view == "MyGames")
                return RedirectToAction("MyGames", "Offer");
            else
                return RedirectToAction("CreateOffer", "Offer");
        }

        public async Task<ActionResult> CancelSwap(int swapStatus, int offerId, int gameSwapWithId, int gameSwapPersonId, bool isActive, DateTime updatedTime,
            string offererEmail, string swapperEmail, string offererNickName, string swapperNickName)
        {
            DIBZ.Common.Model.Swap swap = new DIBZ.Common.Model.Swap();
            var swapLogic = LogicContext.Create<SwapLogic>();

            swap.SwapStatus = (DIBZ.Common.Model.SwapStatus)swapStatus;
            swap.OfferId = offerId;
            swap.GameSwapWithId = gameSwapWithId;
            swap.GameSwapPsersonId = gameSwapPersonId;
            swap.IsActive = true;
            swap.UpdatedTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss"));
            swap = await swapLogic.AddSwap(swap);

            //EmailTemplateHelper templates = new EmailTemplateHelper();
            EmailTemplateResponse emailTemplateResponse = new EmailTemplateResponse();
            var emailTemplateLogic = LogicContext.Create<EmailTemplateLogic>();
            emailTemplateResponse = await emailTemplateLogic.GetEmailTemplate(DIBZ.Common.Model.EmailType.Email, DIBZ.Common.Model.EmailContentType.SwapCancel);

            EmailTemplateHelper OffererTemplate = new EmailTemplateHelper();
            OffererTemplate.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, offererNickName);
            OffererTemplate.AddParam(DIBZ.Common.Model.Contants.UrlContactUs, string.Format("<a href='{0}'>here</a>", hostName + "/Dashboard/ContactUs"));
            var emailBodyOfferrer = OffererTemplate.FillTemplate(emailTemplateResponse.Body);
            //save email data in table
            await emailTemplateLogic.SaveEmailNotification(CurrentLoginSession.ApplicationUser.Email, emailTemplateResponse.Title, emailBodyOfferrer, EmailType.Email, Priority.Low);
            EmailHelper.Email(offererEmail, emailTemplateResponse.Title, emailBodyOfferrer);

            EmailTemplateHelper SwapperTemplate = new EmailTemplateHelper();
            SwapperTemplate.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, swapperNickName);
            SwapperTemplate.AddParam(DIBZ.Common.Model.Contants.UrlContactUs, string.Format("<a href='{0}'>here</a>", hostName + "/Dashboard/ContactUs"));
            var emailBodySwapperr = SwapperTemplate.FillTemplate(emailTemplateResponse.Body);
            //save email data in table
            await emailTemplateLogic.SaveEmailNotification(CurrentLoginSession.ApplicationUser.Email, emailTemplateResponse.Title, emailBodySwapperr, EmailType.Email, Priority.Low);
            EmailHelper.Email(swapperEmail, emailTemplateResponse.Title, emailBodySwapperr);

            return RedirectToAction("MyAllOffers", "Offer");
        }

        public async Task<ActionResult> CreateOffer(int? id, int? offerId, int? gameId, int? returnGameId)
        {            
            string view = ""; //Request.UrlReferrer;// == null ? null : Request.UrlReferrer.Segments[2];
            if (view == "MyOffers")
            {
                DIBZ.Common.Model.GameCatalog selectedGame = new DIBZ.Common.Model.GameCatalog();
                var offerLogic = LogicContext.Create<OfferLogic>();
                selectedGame = await offerLogic.GetSelectedGameById(gameId);
                ViewBag.SelectedGame = selectedGame;

                selectedGame = await offerLogic.GetSelectedGameById(returnGameId);
                ViewBag.DesiredGame = selectedGame;
                ViewBag.OfferId = offerId;
                ViewBag.View = "EditOffer";
            }
            else
            {
                DIBZ.Common.Model.GameCatalog selectedGame = new DIBZ.Common.Model.GameCatalog();
                var offerLogic = LogicContext.Create<OfferLogic>();
                selectedGame = await offerLogic.GetSelectedGameById(id);
                ViewBag.SelectedGame = selectedGame;
                ViewBag.DesiredGame = selectedGame;
            }

            var offerlogic = LogicContext.Create<OfferLogic>();
            var gameCatalogLogic = LogicContext.Create<GameCatalogLogic>();
            var myGames = await gameCatalogLogic.GetAllGamesOfApplicationUser(CurrentLoginSession.ApplicationUserId.GetValueOrDefault());
            var pageSize = Convert.ToInt16(PageSize.AllGames);

            if (TempData["searchGamesData"] != null)
            {
                var currentPage = Convert.ToInt16(TempData["currentPage"]);
                var searchCount = Convert.ToInt32(TempData["searchCount"]);
                var Pages = offerlogic.PagingLogic(currentPage - 1, pageSize, searchCount);
                ViewBag.Pages = Pages;
                ViewBag.FormatId = TempData["formatId"];
                ViewBag.CategoryId = TempData["categoryId"];
                ViewBag.GameName = TempData["gameName"];
                ViewBag.GameCollection = TempData["searchGamesData"];
                ViewBag.SelectedPage = currentPage;
                ViewBag.pageSize = pageSize;
            }
            else
            {
                SearchOffer Search = new SearchOffer();
                Search.GameName = "";
                Search.FormatId = 0;
                Search.CategoryId = 0;

                var currentPage = 1;
                var countAllGames = await gameCatalogLogic.SearchGameCatalog(CurrentLoginSession.ApplicationUserId.GetValueOrDefault(), Search, currentPage, true);
                var totalCount = Convert.ToInt32(countAllGames.Count);
                var Pages = offerlogic.PagingLogic(currentPage, pageSize, totalCount);
                ViewBag.Pages = Pages;
                ViewBag.FormatId = 0;
                ViewBag.CategoryId = 0;
                ViewBag.GameName = "";
                ViewBag.GameCollection = await gameCatalogLogic.SearchGameCatalog(CurrentLoginSession.ApplicationUserId.GetValueOrDefault(), Search, currentPage, false);
                ViewBag.SelectedPage = 1;
                ViewBag.pageSize = pageSize;
            }

            var formatLogic = LogicContext.Create<FormatLogic>();
            var formats = await formatLogic.GetAllFormats();
            ViewBag.Formats = new SelectList(formats, "Id", "Name");

            var categoryLogic = LogicContext.Create<CategoryLogic>();
            var categories = await categoryLogic.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");

            return View(myGames);
        }

        [AuthOp(LoggedInUserOnly = true)]
        public ActionResult MyGameForCreateOffer(int gameId)
        {
            return RedirectToAction("CreateOffer", "Offer", new { id = gameId });
        }

        [AuthOp(LoggedInUserOnly = true)]
        public void UpdateOffer(int offerId, int gameId, int returnGameId)
        {
            var offerLogic = LogicContext.Create<OfferLogic>();
            var offer = offerLogic.EditOffer(offerId, gameId, returnGameId);
        }

        public string SendEmailAfterTransaction(string amount)
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("Dear User");
            str.AppendLine("Your payment of £" + amount + "has been made successfully on DIBZ via PayPal.");
            str.AppendLine("Regards,");
            str.AppendLine("DIBZ Team");
            return str.ToString();
        }
    }
}
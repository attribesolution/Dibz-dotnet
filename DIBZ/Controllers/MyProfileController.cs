using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DIBZ.Base;
using DIBZ.Common;
using DIBZ.Common.DTO;
using DIBZ.Filters;
using DIBZ.Logic.Auth;
using DIBZ.Logic.GameCatalog;
using DIBZ.Logic.Offer;
using DIBZ.Logic.Scorecard;
using DIBZ.Logic.Swap;
using DIBZ.Logic.Notification;

namespace DIBZ.Controllers
{
    public class MyProfileController : BaseWebController
    {
        [HttpGet]
        [AuthOp(LoggedInUserOnly = true)]
        // GET: MyProfile
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public async Task<ActionResult> Index()
        {

            var authLogic = LogicContext.Create<AuthLogic>();
           
            var scorecardLogic = LogicContext.Create<ScorecardLogic>();
           

            var applicationUserData = await authLogic.GetApplicationUserById(CurrentLoginSession.ApplicationUserId.GetValueOrDefault());
           
            ViewBag.Scorecard = await scorecardLogic.GetScoreCardByAppUserId(CurrentLoginSession.ApplicationUserId.GetValueOrDefault());//applicationUserData.Scorecard;
            
            return View(applicationUserData);
        }

        [AuthOp(LoggedInUserOnly = true)]
        public ActionResult EditProfile(string firstName, string lastName, string nickName, string address, string aboutMe, string mobileNo, string birthYear, string postalCode)
        {
            var authLogic = LogicContext.Create<AuthLogic>();
            try
            {

                var user = authLogic.EditProfile(CurrentLoginSession.ApplicationUserId.GetValueOrDefault(), firstName, lastName, nickName, address, aboutMe, mobileNo, birthYear,postalCode);
                if (user != null)
                {
                    return Json(new { IsSuccess = true,  nickName=user.NickName, firstName=user.FirstName,lastName=user.LastName,address=user.Address,aboutMe=user.AboutMe, mobileNo=user.CellNo,birthYear=user.YearOfBirth,postalCode=user.PostalCode }, JsonRequestBehavior.AllowGet);
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
        //public ActionResult SaveProfileImage(string abc, HttpPostedFileBase profileImage)
        [AuthOp(LoggedInUserOnly = true)]
        public ActionResult SaveProfileImage(HttpPostedFileBase file)
        {

            var authLogic = LogicContext.Create<AuthLogic>();
            try
            {

                var serverPath = Server.MapPath("~/Uploads");

                string profileImageName = string.Empty;
                if (file != null)
                {
                    profileImageName = file.FileName;
                }
                var appUser = authLogic.EditProfileImage(CurrentLoginSession.ApplicationUserId.GetValueOrDefault(), profileImageName);

                if (appUser != null)
                {
                    var savePathBackgroundImage = Path.Combine(serverPath, appUser.ProfileImage.Id.ToString());
                    if (file != null)
                        file.SaveAs(savePathBackgroundImage);
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

        public ActionResult DeleteProfileImage()
        {
            var authLogic = LogicContext.Create<AuthLogic>();
            try
            {
                var appUser = authLogic.DeleteProfileImage(CurrentLoginSession.ApplicationUserId.GetValueOrDefault());
                return Json(new { IsSuccess = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception lex)
            {
                return Json(new { IsSuccess = false, fail = lex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public async Task<ActionResult> ViewProfile(int id)
        {
            var authLogic = LogicContext.Create<AuthLogic>();
            var scorecardLogic = LogicContext.Create<ScorecardLogic>();
            var applicationUserData = await authLogic.GetApplicationUserByIdWithUpdateViewCounter(id);
            ViewBag.Scorecard = await scorecardLogic.GetApplicationUserScorecard(id);
            return View(applicationUserData);
        }

        
    }
}
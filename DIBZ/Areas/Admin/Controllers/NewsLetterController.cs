using DIBZ.Base;
using DIBZ.Data;
using DIBZ.Filters;
using DIBZ.Logic;
using DIBZ.Logic.NewsLetter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DIBZ.Areas.Admin.Controllers
{
    public class NewsLetterController : BaseWebController
    {
        // GET: Admin/NewsLetter
        [AuthOp(AdminOnly = true)]
        public async Task<ActionResult> Index()
        {
            var newsLetterLogic = LogicContext.Create<NewsLetterLogic>();
            var newsLetterData = await newsLetterLogic.GetAllNewsLetterSubscriber();

            DIBZDbContext context = new DIBZDbContext();
            var notifierEmail = (from NotifierEmails in context.NotifierEmails
                                 orderby NotifierEmails.CreatedTime descending
                                 select new
                                 {
                                     EmailAddress = NotifierEmails.EmailAddress
                                 }).ToList();

            if (notifierEmail.Count != 0)
            {
                ViewBag.NotifierEmail = notifierEmail.FirstOrDefault().EmailAddress;
            }
            else
            {
                ViewBag.NotifierEmail = "No email exist";
            }
            return View(newsLetterData);
        }

        [HttpPost]
        [AuthOp(AdminOnly = true)]
        public async Task<ActionResult> AddNotifierEmail(FormCollection formData)
        {
            var newsLetterLogic = LogicContext.Create<NewsLetterLogic>();
            DIBZ.Common.Model.NotifierEmail request = new Common.Model.NotifierEmail();
            request.EmailAddress = formData["email"].ToString();
            await newsLetterLogic.AddNotifierEmail(request);
            return RedirectToAction("Index");
        }
    }
}
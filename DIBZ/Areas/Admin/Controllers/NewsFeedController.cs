using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DIBZ.Base;
using DIBZ.Filters;
using DIBZ.Logic.NewsFeed;

namespace DIBZ.Areas.Admin.Controllers
{
    public class NewsFeedController : BaseWebController
    {
        // GET: Admin/NewsFeed
        [AuthOp(AdminOnly = true)]
        public async Task<ActionResult> Index()
        {
            var newsFeedLogicLogic = LogicContext.Create<NewsFeedLogic>();
            var newsFeedData = await newsFeedLogicLogic.GetAllNewsFeedAdmin();
            return View(newsFeedData);
        }

        [AuthOp(AdminOnly = true)]
        public async Task<ActionResult> AddUpdate(int? id)
        {

            var newsFeedLogicLogic = LogicContext.Create<NewsFeedLogic>();
            DIBZ.Common.Model.NewsFeed newsFeed = new DIBZ.Common.Model.NewsFeed();

            if (id > 0)
            {
                newsFeed = await newsFeedLogicLogic.GetNewsFeedById(id.Value);
            }

            return View(newsFeed);
        }

        [HttpPost]
        [AuthOp(AdminOnly = true)]
        public async Task<ActionResult> AddUpdateNewsFeed(FormCollection formData)
        {
            int id = Convert.ToInt32(formData["Id"]);

            var newsFeedLogicLogic = LogicContext.Create<NewsFeedLogic>();
            DIBZ.Common.Model.NewsFeed request = new Common.Model.NewsFeed();
            request.Id = id;
            request.News = formData["news"];
            await newsFeedLogicLogic.AddUpdate(request);
            return RedirectToAction("Index");
        }


        [AuthOp(AdminOnly = true)]
        public async Task<ActionResult> Delete(int id)
        {
            if (id > 0)
            {
                var newsFeedLogicLogic = LogicContext.Create<NewsFeedLogic>();
                await newsFeedLogicLogic.Delete(id);
                return RedirectToAction("Index", "NewsFeed");
            }
            return View("Index");
        }
    }
}
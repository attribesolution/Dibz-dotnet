using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DIBZ.Base;
using DIBZ.Logic;
using DIBZ.Logic.GameCatalog;
using DIBZ.Logic.Offer;
using DIBZ.Common.DTO;
using DIBZ.Logic.Auth;
using DIBZ.Common;
using DIBZ.Logic.Notification;
using DIBZ.Services;
using DIBZ.Filters;
using System.Configuration;
using DIBZ.Logic.NewsLetter;
using DIBZ.Logic.NewsFeed;
using DIBZ.Logic.SupportsQueries;
using DIBZ.Common.Model;
using DIBZ.Data;
//using MailChimp.Types;
using MailChimp;

namespace DIBZ.Areas.Admin.Controllers
{
    public class DibzChargesController : BaseWebController
    {
        string hostName = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

        // public ActionResult Index()

        [AuthOp(AdminOnly = true)]
        public async Task<ActionResult> Index()
        {
            var chargesLogic = LogicContext.Create<DibzChargesLogic>();
            var chargesData = await chargesLogic.GetAllDibzChargesData();
            return View(chargesData);
        }
        [AuthOp(AdminOnly = true)]
        public async Task<ActionResult> AddUpdate(int? id)
        {

            var chargesLogic = LogicContext.Create<DibzChargesLogic>();
            DIBZ.Common.Model.DibzCharges charges = new DIBZ.Common.Model.DibzCharges();

            if (id > 0)
            {
                charges = await chargesLogic.GetDibzChargesById(id.Value);
            }

            return View(charges);
        }

        [HttpPost, ValidateInput(false)]
        [AuthOp(AdminOnly = true)]
        public async Task<ActionResult> AddUpdateCharges(FormCollection formData)
        {            
            int id = Convert.ToInt32(formData["Id"]);
            var chargesLogic = LogicContext.Create<DibzChargesLogic>();
            DIBZ.Common.Model.DibzCharges request = new Common.Model.DibzCharges();
            request.Id = id;
            request.Charges = formData["Charges"];                     
            var status = formData["status"];
            if (status == "1")
            {
                request.IsActive = true;
            }
            else
            {
                request.IsActive = false;
            }

            await chargesLogic.AddUpdate(request);
            return RedirectToAction("Index");
        }


        [AuthOp(AdminOnly = true)]
        public async Task<ActionResult> Delete(int id)
        {
            if (id > 0)
            {
                var chargesLogic = LogicContext.Create<DibzChargesLogic>();
                await chargesLogic.Delete(id);
                return RedirectToAction("Index", "DibzCharges");
            }
            return View("Index");
        }


        [AuthOp(AdminOnly = true)]
        public ActionResult DibzChargesDeActive(int id, int Status)
        {
            bool status = false;
            var chargesLogic = LogicContext.Create<DibzChargesLogic>();
            if (Status == 0)
            {
                status = true;
            }
            var charges = chargesLogic.UpdateStatus(id, status);
            return View("~/Areas/Admin/Views/DibzCharges/Index.cshtml", charges);

        }

    }
}
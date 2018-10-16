using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DIBZ.Base;
using DIBZ.Common;
using DIBZ.Filters;
using DIBZ.Logic.NonWorkingDay;

namespace DIBZ.Areas.Admin.Controllers
{
    public class NonWorkingDayController : BaseWebController
    {
        // GET: Admin/NonWorkingDay
        [AuthOp(AdminOnly = true)]
        public async Task<ActionResult> Index()
        {
            var nonWokringDayLogic = LogicContext.Create<NonWorkingDayLogic>();
            var nonWokringDayData = await nonWokringDayLogic.GetAllNonWorkingDays();
            return View(nonWokringDayData);
        }

        [AuthOp(AdminOnly = true)]
        public async Task<ActionResult> AddUpdate(int? id)
        {

            var nonWokringDayLogic = LogicContext.Create<NonWorkingDayLogic>();
            DIBZ.Common.Model.NonWorkingDay nonWorkingDay = new DIBZ.Common.Model.NonWorkingDay();

            if (id > 0)
            {
                nonWorkingDay = await nonWokringDayLogic.GetNonWorkingDayId(id.Value);
            }
            
            return View(nonWorkingDay);
        }

        [HttpPost]
        [AuthOp(AdminOnly = true)]
        public async Task<ActionResult> AddUpdateNonWorkingDay(FormCollection formData)
        {
            int id = Convert.ToInt32(formData["Id"]);

            var nonWokringDayLogic = LogicContext.Create<NonWorkingDayLogic>();
            DIBZ.Common.Model.NonWorkingDay request = new Common.Model.NonWorkingDay();
            request.Id = id;
            request.Title = formData["title"];
            request.NonWorkingDate = DateTime.ParseExact(formData["date"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
            request.Reason = formData["reason"];

            await nonWokringDayLogic.AddUpdate(request);
            return RedirectToAction("Index");
        }

        
        [AuthOp(AdminOnly = true)]
        public async Task<ActionResult> Delete(int id)
        {
            if (id > 0)
            {
                var nonWokringDayLogic = LogicContext.Create<NonWorkingDayLogic>();
                await nonWokringDayLogic.Delete(id);
                return RedirectToAction("Index", "NonWorkingDay");
            }
            return View("Index");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DIBZ.Base;
using DIBZ.Filters;
using DIBZ.Logic.NonWorkingDay;

namespace DIBZ.Controllers
{
    public class NonWorkingDaysController : BaseWebController
    {
        // GET: NonWorkingDays
        
        [AuthOp(LoggedInUserOnly = true)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public async Task <ActionResult> Index()
        {
            var nonWokringDayLogic = LogicContext.Create<NonWorkingDayLogic>();
            var nonWokringDayData = await nonWokringDayLogic.GetAllNonWorkingDaysPublic();
            return View(nonWokringDayData);
            
        }
    }
}
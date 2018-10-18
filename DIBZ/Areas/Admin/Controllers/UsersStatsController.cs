using DIBZ.Base;
using DIBZ.Filters;
using DIBZ.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DIBZ.Areas.Admin.Controllers
{
    public class UsersStatsController : BaseWebController
    {
        // GET: Admin/UsersStats
        [AuthOp(AdminOnly = true)]
        public ActionResult Index()
        {
            return View();
        }

        // Shoaib Code
        public async Task<ActionResult> GetStatsCount()
        {
            var usersStatsLogic = LogicContext.Create<UsersStatsLogic>();

            // for Users Registration Count
            var todayReg = await usersStatsLogic.GetTodayRegisterCount();
            var sevenDayReg = await usersStatsLogic.GetLastSevenDayRegisterCount();
            var fourWeekReg = await usersStatsLogic.GetLastFourWeekRegisterCount();
            var sixMonthReg = await usersStatsLogic.GetLastSixMonthRegisterCount();
            var yearReg = await usersStatsLogic.GetLastOneYearRegisterCount();

            // for Users Login Count
            var todayLogin = await usersStatsLogic.GetTodayLoginCount();
            var sevenDayLogin = await usersStatsLogic.GetLastSevenDayLoginCount();
            var fourWeekLogin = await usersStatsLogic.GetLastFourWeekLoginCount();
            var sixMonthLogin = await usersStatsLogic.GetLastSixMonthLoginCount();
            var yearLogin = await usersStatsLogic.GetLastOneYearLoginCount();

            // for Total Games Count
            var ps4Games = await usersStatsLogic.GetPS4Count();
            var nswGames = await usersStatsLogic.GetNSWCount();
            var ndsGames = await usersStatsLogic.GetNDSCount();
            var xb1Games = await usersStatsLogic.GetXB1Count();

            if (todayReg != null && sevenDayReg != null && fourWeekReg != null
                && sixMonthReg != null && yearReg != null && todayLogin != null
                && sevenDayLogin != null && fourWeekLogin != null && sixMonthLogin != null
                && yearLogin != null && ps4Games != null && nswGames != null
                && ndsGames != null && xb1Games != null)
            {
                return Json(new
                {
                    IsSuccess = true,
                    todayReg = "" + todayReg.Count().ToString() + "",
                    sevenDayReg = "" + sevenDayReg.Count().ToString() + "",
                    fourWeekReg = "" + fourWeekReg.Count().ToString() + "",
                    sixMonthReg = "" + sixMonthReg.Count().ToString() + "",
                    yearReg = "" + yearReg.Count().ToString() + "",
                    todayLogin = "" + todayLogin.Count().ToString() + "",
                    sevenDayLogin = "" + sevenDayLogin.Count().ToString() + "",
                    fourWeekLogin = "" + fourWeekLogin.Count().ToString() + "",
                    sixMonthLogin = "" + sixMonthLogin.Count().ToString() + "",
                    yearLogin = "" + yearLogin.Count().ToString() + "",
                    ps4Games = "" + ps4Games.Count().ToString() + "",
                    nswGames = "" + nswGames.Count().ToString() + "",
                    ndsGames = "" + ndsGames.Count().ToString() + "",
                    xb1Games = "" + xb1Games.Count().ToString() + ""
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { IsSuccess = false, fail = "some thing wrong" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DIBZ.Base;
using DIBZ.Logic;
using DIBZ.Logic.Swap;

namespace DIBZ.Controllers
{
    public class QRTestController : BaseWebController
    {
        // GET: QRTest
        public ActionResult Index()
        {
            return View();
        }
        public async Task<ActionResult> ReadQR(string id)
        {
            var swapLogic = LogicContext.Create<SwapLogic>();
            var swapDetail = await swapLogic.GetSwapDetailByDFOMCode(id);

            return View(swapDetail);
        }
    }
}
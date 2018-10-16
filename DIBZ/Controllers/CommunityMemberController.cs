using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DIBZ.Base;
using DIBZ.Common.DTO;
using DIBZ.Logic;
using DIBZ.Logic.CommunityMember;

namespace DIBZ.Controllers
{
    public class CommunityMemberController : BaseController
    {

        //Variable Declarations
        IEnumerable<DIBZ.Common.Model.CommunityMember> communityMembers = new List<DIBZ.Common.Model.CommunityMember>();
        List<DIBZ.Common.DTO.CommunityMemberModel> communityMemberList = new List<DIBZ.Common.DTO.CommunityMemberModel>();
        public async Task<ActionResult> Index()
        {
            var communityMemberLogic = LogicContext.Create<CommunityMemberLogic>();

            //get all CommunityMembers
            communityMembers = await communityMemberLogic.GetAllCommunityMember();

            //Get values for Presentation Layer Model To show 
            communityMemberList = communityMembers.Select(x => new CommunityMemberModel
            {
                Id = x.Id,
                Name = x.Name,
                Address = x.Address,
                Email = x.Email,
                Password = x.Password,
                UpdatedBy =x.UpdatedBy,
                CreatedBy = x.CreatedBy,
                CreatedTime = x.CreatedTime,
                UpdatedTime = x.UpdatedTime,

            }).ToList();
            return View(communityMemberList);
        }


        /// <summary>
        /// This Method Use for Update And Add Both Purpuse
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> AddUpdate(CommunityMemberModel request)
        {
            if (TryValidateModel(Request))
            {
                var communityMemberLogic = LogicContext.Create<CommunityMemberLogic>();
                await communityMemberLogic.AddUpdateCommunityMember(request);
                return RedirectToAction("Index", "CommunityMember");
            }
            return View("Index", Request);
        }

        /// <summary>
        /// Delete GameCatalog
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Delete(int Id)
        {
            if (Id > 0)
            {
                var communityMemberLogic = LogicContext.Create<CommunityMemberLogic>();
                var result = await communityMemberLogic.Delete(Id);
                if (result == true)
                {
                    return RedirectToAction("Index", "CommunityMember");
                }
                else
                {
                    // you can ignore if and else condition here,,it will be manage ltr
                    return RedirectToAction("Index", "CommunityMember");
                }
            }
            return View("Index");
        }


        /// <summary>
        /// Uupdate Get Method
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> AddUpdate(int Id)
        {
            DIBZ.Common.Model.CommunityMember communityMember = new DIBZ.Common.Model.CommunityMember();
            communityMember = null;
            if (Id > 0)
            {
                var communityMemberLogic = LogicContext.Create<CommunityMemberLogic>();
                communityMember = await communityMemberLogic.GetCommunityMemberById(Id);
                return RedirectToAction("Edit", communityMember);
            }
            return View("Edit", communityMember);
        }
    }
}
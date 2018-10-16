using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DIBZ.Base;
using DIBZ.Common.DTO;
using DIBZ.Logic;
using DIBZ.Logic.Testimonial;

namespace DIBZ.Controllers
{
    public class TestimonialController : BaseController
    {
        //Variable Declarations
        IEnumerable<DIBZ.Common.Model.Testimonial> testimonials = new List<DIBZ.Common.Model.Testimonial>();
        List<DIBZ.Common.DTO.TestimonialModel> testimonialList = new List<DIBZ.Common.DTO.TestimonialModel>();
        public async Task<ActionResult> Index()
        {
            var testimonialLogic = LogicContext.Create<TestimonialLogic>();

            //get all CommunityMembers
            testimonials = await testimonialLogic.GetAllTestimonial();

            //Get values for Presentation Layer Model To show 
            testimonialList = testimonials.Select(x => new TestimonialModel
            {
                Id = x.Id,
                Description = x.Description,

            }).ToList();
            return View(testimonialList);
        }

        /// <summary>
        /// Add Testimonial
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Add(TestimonialModel request)
        {
            if (TryValidateModel(request))
            {
                var testimonialLogic = LogicContext.Create<TestimonialLogic>();
                await testimonialLogic.AddTestimonial(request);
                return RedirectToAction("Index", "Testimonial");
            }
            return View("Index", request);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIBZ.Common.DTO;
using DIBZ.Common.Model;
using DIBZ.Data;

namespace DIBZ.Logic.Testimonial
{
    public class TestimonialLogic : BaseLogic
    {
        public TestimonialLogic(LogicContext context) : base(context)
        {
        }

        public async Task<DIBZ.Common.Model.Testimonial> AddTestimonial(TestimonialModel request)
        {
            DIBZ.Common.Model.Testimonial testimonial = null;
            testimonial.Description = request.Description;
            await Db.SaveAsync();
            return testimonial;
        }


        public async Task<IEnumerable<DIBZ.Common.Model.Testimonial>> GetAllTestimonial()
        {
            return await Db.Query<DIBZ.Common.Model.Testimonial>().QueryAsync();
        }
    }
}

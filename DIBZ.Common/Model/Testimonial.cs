using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIBZ.Common.Model
{
    public class Testimonial
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string CommunityMemberId { get; set; }
        public string Description { get; set; }

        public string CreatedTime { get; set; }
        public string CreatedBy { get; set; }
    }
}

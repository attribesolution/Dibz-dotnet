using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIBZ.Common.Model
{
    public class NonWorkingDay: BaseModelObject
    {
        public DateTime NonWorkingDate { get; set; }
        public string Title { get; set; }
        public string Reason { get; set; }

    }
}

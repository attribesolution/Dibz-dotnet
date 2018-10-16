using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIBZ.Common.Model
{
    public class CommunityMember : BaseModelObject
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string CellNo { get; set; }

        public string CountryId { get; set; }

        public string CityId { get; set; }

        public string AreaId { get; set; }

        public string Address { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }
    }
}

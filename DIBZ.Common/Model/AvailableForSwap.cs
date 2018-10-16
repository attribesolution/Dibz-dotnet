using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIBZ.Common.Model
{
    public class AvailableForSwap 
    {
        public int Id { get; set; }
        public string CommunityMemberId { get; set; }
        public string GameCatalogId { get; set; }
        public string AgaintsGameCatalogId { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}


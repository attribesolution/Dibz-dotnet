using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIBZ.Common.Model
{
    public class ApplicatonUserGameCatalog : BaseModelObject
    {

        public virtual GameCatalog GameCatalog { get; set; }
        public virtual CommunityMember CommunityMember { get; set; }
        public int GamesCatalogId { get; set; }

        public int CommunityMemberId { get; set; }

        public string CreatedBy { get; set; }
    }
}

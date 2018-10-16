using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIBZ.Common.Model
{
    public class Transaction: BaseModelObject
    {
        public virtual Offer Offer { get; set; }
        public int OfferId { get; set; }

        public bool IsPaid { get; set; }
        public decimal Aomunt { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
        public int? ApplicationUserId { get; set; }

    }
}

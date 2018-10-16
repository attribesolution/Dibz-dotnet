using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIBZ.Common.Model
{
    public class CounterOffer:BaseModelObject
    {
        public virtual Offer Offer { get; set; }
        public int OfferId { get; set; }

        public virtual GameCatalog GameCounterOfferWith { get; set; }
        [ForeignKey("GameCounterOfferWith")]
        public int GameCounterOfferWithId { get; set; }

        public virtual ApplicationUser CounterOfferPerson { get; set; }
        [ForeignKey("CounterOfferPerson")]
        public int CounterOfferPersonId { get; set; }
    }
}

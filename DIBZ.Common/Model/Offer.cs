using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIBZ.Common.Model
{
    public class Offer: BaseModelObject
    {

        public string Description { get; set; }
        public virtual GameCatalog ReturnGameCatalog { get; set; }
        public int? ReturnGameCatalogId { get; set; }

        public OfferStatus OfferStatus { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public int ApplicationUserId { get; set; }

        public virtual GameCatalog GameCatalog { get; set; }
        public int GameCatalogId { get; set; }

        public string GameOffererDFOM { get; set; }
        public string GameSwapperDFOM { get; set; }

        public virtual ICollection<CounterOffer> CounterOffers { get; set; }
        public virtual ICollection<Swap> Swaps { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public Offer()
        {
            Swaps = new Collection<Swap>();
            CounterOffers = new Collection<CounterOffer>();
            Transactions = new Collection<Transaction>();
        }
        
    }
}

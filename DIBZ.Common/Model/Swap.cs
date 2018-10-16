using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace DIBZ.Common.Model
{
    public class Swap:BaseModelObject
    {
        public SwapStatus SwapStatus { get; set; }
        
        public virtual Offer Offer { get; set; }
        public int OfferId { get; set; }

        public virtual GameCatalog GameSwapWith { get; set; }
        [ForeignKey("GameSwapWith")]
        public int GameSwapWithId { get; set; }

        public virtual ApplicationUser GameSwapPserson { get; set; }
        [ForeignKey("GameSwapPserson")]
        public int GameSwapPsersonId { get; set; }

        public string GamerOffererDFOM { get; set; }
        public string GamerSwapperDFOM { get; set; }
    }
}

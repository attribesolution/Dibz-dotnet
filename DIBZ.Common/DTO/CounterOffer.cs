using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIBZ.Common.DTO
{
    public class CounterOffer
    {
        public int GameCatalogId { get; set; }
        public int ReturnGameCatalogId { get; set; }
        public string Description { get; set; }
        public int OfferId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIBZ.Common.DTO
{
    public class Swap
    {
        public int Id { get; set; }
        public DateTime SwapDate { get; set; }
        public int OfferedGameImageId { get; set; }
        public int SwapGameImageId { get; set; }
        public int GameSwapWithId { get; set; }
        public int GameSwapPersonId { get; set; }
        public int OfferPersonId { get; set; }
        public int OfferId { get; set; }
        public DateTime CreatedTime { get; set; }
        public string OfferedGame { get; set; }
        public string GameSwapPersonNickName { get; set; }
        public string OfferPersonNickName { get; set; }
        public string OfferedBy { get; set; }
        public string ReturnedGame { get; set; }
        public string ReturnedBy { get; set; }
        public DIBZ.Common.Model.SwapStatus SwapStatus { get; set; }
        public DIBZ.Common.Model.SwapStatus SwapButtonToShow{ get; set; }
        public string OfferedGameFormat { get; set; }
        public string ReturnedGameFormat { get; set; }
        public string GameOffererDFOM { get; set; }
        public string GameSwapperDFOM { get; set; }

        public DIBZ.Common.Model.OfferStatus OfferStatus { get; set; }

    }
    public class MySwap
    {
        public int Id { get; set; }
        public int OfferId { get; set; }

        public DIBZ.Common.Model.SwapStatus SwapStatus { get; set; }
        public int OfferedGameImageId { get; set; }

        public DIBZ.Common.Model.OfferStatus OfferStatus { get; set; }
        public bool IsPaymentRequired { get; set; }
        public int GameSwapPersonId { get; set; }
        public int OfferPersonId { get; set; }

        public string GameSwapPersonNickName { get; set; }
        public string OfferPersonNickName { get; set; }

        public DateTime CreatedTime { get; set; }
        public string GameName { get; set; }
        public string GameFormat { get; set; }
        public string ReturnGameName { get; set; }
        public string ReturnGameFormat { get; set; }
        public int SwappedGameImageId { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public string OffererEmail { get; set; }
        public string SwapperEmail { get; set; }
    }
}

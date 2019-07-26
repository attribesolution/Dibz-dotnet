using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIBZ.Common.DTO
{
    public class OfferModel
    {
        public int Id { get; set; }
        public int ApplicationUserId { get; set; }
        public int GameCatalogId { get; set; }
        public int? AgaintsGameCatalogId { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
    }
    public class SearchOffer
    {
        public int OfferId { get; set; }
        public int AppUserId { get; set; }
        public string AppUserFullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string imgpath { get; set; }
        public decimal sellprice { get; set; }
        public decimal cashprice { get; set; }
        public decimal voucherprice { get; set; }
        public int GameId { get; set; }
        public string GameName { get; set; }
        public int GameImageId { get; set; }
        public string GameFormat { get; set; }
        public DateTime OfferedTime { get; set; }
        public string OfferedTimeValue { get; set; }
        public int FormatId { get; set; }
        public int CategoryId { get; set; }

        public DateTime AppUserRegisteredTime { get; set; }
    }
}

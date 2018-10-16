using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIBZ.Common.DTO
{
    public class NotificationModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int AppUserId { get; set; }
        public int OfferId { get; set; }
        public List<int> OfferIds { get; set; }
        public int Channel { get; set; }
        public int NotificationType { get; set; }
        public int NotificationBusinessType { get; set; }
        public int Status { get; set; }
        public string LastError { get; set; }
        public string AdditionalData { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
    }

    public class NotificationAdditionalData
    {
        public int GameCatalogId { get; set; }
        public int ReturnGameCatalogId { get; set; }
        public int OfferId { get; set; }
        public string Description { get; set; }

    }
}

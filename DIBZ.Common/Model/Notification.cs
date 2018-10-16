using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIBZ.Common.Model
{
    public class Notification : BaseModelObject
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int AppUserId { get; set; }
        public int OfferId { get; set; }
        public int Channel { get; set; }
        public int NotificationType { get; set; }
        public int NotificationBusinessType { get; set; }
        public string AdditionalData { get; set; }
        public int Status { get; set; }
        public string LastError { get; set; }
        public string  DisplayDateTime { get; set; }
        public Notification()
        {

        }
    }
}

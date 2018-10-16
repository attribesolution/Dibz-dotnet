using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIBZ.Common.Model
{
    public class LoginSession : BaseModelObject
    {
        [MaxLength(500)]
        public string Token { get; set; }
        [MaxLength(100)]
        public string Platform { get; set; }
        [MaxLength(500)]
        public string DeviceToken { get; set; }
        public DateTime LastAccessTime { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public int? ApplicationUserId { get; set; }

        public int? AdminId { get; set; }

        public virtual Admin Admin { get; set; }

        public LoginSession()
        {
            
        }
    }
}

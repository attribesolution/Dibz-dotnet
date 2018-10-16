using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIBZ.Common.Model
{
    public class User : BaseModelObject
    {
        //[MaxLength(100)]
        public string FirstName { get; set; }
        //[MaxLength(100)]
        public string LastName { get; set; }
        //[MaxLength(100)]
        public string Email { get; set; }
        [MaxLength(500)]
        public string Password { get; set; }
        
        [MaxLength(500)]
        public string PasswordResetToken { get; set; }
        public virtual ICollection<LoginSession> LoginSessions { get; set; }

        public User()
        {
            LoginSessions = new Collection<LoginSession>();

        }
    }
}

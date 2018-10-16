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
    public class MyQueries : BaseModelObject
    {

        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }

        public bool IsDeletedByAdmin { get; set; }
        public bool IsDeletedByAppUser { get; set; }

        public QueryStatus QueryStatus { get; set; }

        

        public virtual ICollection<MyQueryDetails> querylog { get; set; }

        public virtual ICollection<ApplicationUser> appuser { get; set; }
        public int? AppUserId { get; set; }



        //public virtual UploadedFile ProfileImage { get; set; }
        //[ForeignKey("ProfileImage")]
        //public int? ProfileImageId { get; set; }


        //public string AdminMessage { get; set; }
        //public virtual Admin admin { get; set; }
        //[ForeignKey("Admin")]
        //public int? adminId { get; set; }

        //public string AppUserMessage { get; set; }
        //public virtual ApplicationUser appUser { get; set; }
        //[ForeignKey("ApplicationUser")]
        //public int? AppUserId { get; set; }

    }
}

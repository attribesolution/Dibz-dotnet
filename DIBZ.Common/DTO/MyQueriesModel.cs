using DIBZ.Common.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIBZ.Common.DTO
{
   public class MyQueriesModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string LastUpdateBy { get; set; }
        public int? UserImageId { get; set; }
        

        public QueryStatus QueryStatus { get; set; }
        public int QueryStatusValue { get; set; }
        public int Id { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }

        public int Myqueryid { get; set; }
        public int Userid { get; set; }
        public int? Adminid { get; set; }

        //public int? ProfileImageId { get; set; }

        //public virtual UploadedFile ProfileImage { get; set; }
        //[ForeignKey("ProfileImage")]
    }
    
}

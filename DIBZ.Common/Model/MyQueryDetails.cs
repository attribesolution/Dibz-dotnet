using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIBZ.Common.Model
{
   public class MyQueryDetails:BaseModelObject
    {
        
        public int MyQueryId { get; set; }
        public int? AdminId { get; set; }
        public string Message  { get; set; }
        public virtual MyQueries myquery { get; set; }
        public virtual Admin admin { get; set; }

        //public virtual ApplicationUser appuser { get; set; }
        



        ////public int QueryLogId { get; set; }
        ////public virtual MyQuery myQuery { get; set; }
        ////[ForeignKey("MyQuery")]
        //public int MyQuerDetailsId { get; set; }
        ////public virtual ApplicationUser appUser { get; set; }
        ////[ForeignKey("ApplicationUser")]
        //public int AppUserDetailId { get; set; }
        //public string Conversation { get; set; }

        //public virtual ICollection<MyQuery> myQuery { get; set; }

        //public virtual ICollection<ApplicationUser> appuser { get; set; }
        //public QueryLog()
        //{
        //    myQuery = new Collection<MyQuery>();
        //    appuser = new Collection<ApplicationUser>();
    }

}

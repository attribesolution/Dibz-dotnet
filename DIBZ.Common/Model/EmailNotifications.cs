using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIBZ.Common.Model
{
public class EmailNotification:BaseModelObject
    {
        public string Tiltle { get; set; }
        public string Body { get; set; }
        public string ApplicationUserEmail { get; set; }
        public  EmailType EmailType { get; set; }
        public bool IsSend { get; set; }
       public Priority Priority { get; set; }

    }
}

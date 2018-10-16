using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIBZ.Common.Model
{
   public class EmailTemplate:BaseModelObject
    {
        public EmailType EmailType { get; set; }
        public EmailContentType EmailContentType { get; set; }
        public bool IsHtml { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIBZ.Common.Model
{
    public class Competition : BaseModelObject
    {
       public string Name { get; set; }
       public string Title { get; set; }        
        public string Content { get; set; }
       public string FileOrignalName { get; set; }
       public string FileNewName { get; set; }
       public Competition()
       {

       }
    }

}

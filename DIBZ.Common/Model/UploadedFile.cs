using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIBZ.Common.Model
{
    public class UploadedFile : BaseModelObject
    {
        [MaxLength(200)]
        public string Filename { get; set; }
    }
}

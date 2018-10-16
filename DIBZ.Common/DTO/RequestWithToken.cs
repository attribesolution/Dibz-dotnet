using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIBZ.Common.DTO
{
    public abstract class RequestWithToken
    {
        public string Token { get; set; }
    }
}

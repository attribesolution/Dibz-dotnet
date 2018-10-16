using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIBZ.Common.DTO
{
    public class BaseResponse
    {
        public BaseResponse()
        {
            Success = true;
        }

        public string FaultMessage { get; set; }
        public bool Success { get; set; }
    }
}

using DevOne.Security.Cryptography.BCrypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace DIBZ.Common
{
    public class Helpers
    {
        public static string Hash(string srcString)
        {
            return BCryptHelper.HashPassword(srcString, BCryptHelper.GenerateSalt());
        }

        public static bool ValidateHash(string srcString, string hash)
        {
            return BCryptHelper.CheckPassword(srcString, hash);
        }

        public static string GetJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }       
    }
}

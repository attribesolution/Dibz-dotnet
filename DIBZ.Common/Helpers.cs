using DevOne.Security.Cryptography.BCrypt;
using Newtonsoft.Json;

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

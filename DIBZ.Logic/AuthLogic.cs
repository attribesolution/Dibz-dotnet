using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIBZ.Common.Model;
using DIBZ.Data;
using DIBZ.Common;

namespace DIBZ.Logic
{
    public class AuthLogic:BaseLogic
    {
        public AuthLogic(LogicContext context) : base(context)
        {
        }
        public async Task<ApplicationUser> GetApplicationUserByEmail(string email)
        {
            return (await Db.Query<ApplicationUser>(c => c.Email.ToLower() == email.ToLower()).QueryAsync()).FirstOrDefault();
        }
        public LoginSession GetLoginSessionByToken(string token)
        {
            return Db.Query<LoginSession>(l => l.Token == token && l.IsDeleted == false).FirstOrDefault();
        }

        public DIBZ.Common.Model.CommunityMember AddUpdateUser(int id, string userName, string email, string password)
        {
            DIBZ.Common.Model.CommunityMember User = null;
            if (id > 0)
            {
                User =  GetUserById(id);
            }
            else
            {
                User = new DIBZ.Common.Model.CommunityMember();
            }

            User.IsActive = true;
            User.IsDeleted = false;
            User.Name = userName;
            User.Email = email;
            User.Password = Helpers.Hash(password);
            Db.Add(User);
            Db.Save();
            return User;
        }

        public  DIBZ.Common.Model.CommunityMember GetUserById(int id)
        {
            return Db.Query<DIBZ.Common.Model.CommunityMember>(c => c.Id == id).FirstOrDefault();
        }
    }
}

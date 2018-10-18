using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIBZ.Common.DTO;
using DIBZ.Common.Model;
using DIBZ.Data;
using DIBZ.Common;
using DIBZ.Logic.GameCatalog;
using DIBZ.Logic.Scorecard;

namespace DIBZ.Logic.Auth
{
    public class AuthLogic : BaseLogic
    {
        public AuthLogic(LogicContext context) : base(context)
        {
        }

        public ApplicationUser GetApplicationUserByEmail(string email)
        {
            return  Db.Query<ApplicationUser>(c => c.Email.ToLower() == email.ToLower()).FirstOrDefault();
        }

        public Admin GetAdminByEmail(string email)
        {
            return Db.Query<Admin>(c => c.Email.ToLower() == email.ToLower()).FirstOrDefault();
        }

        public async Task<IEnumerable<ApplicationUser>> GetApplicationUserByGameId(int gameId)
        {

            return await Db.Query<ApplicationUser>(c => c.GameCatalogs.Any(x => x.Id == gameId)).QueryAsync();
        }
        public LoginSession GetLoginSessionByToken(string token)
        {
            return Db.Query<LoginSession>(l => l.Token == token && l.IsDeleted == false).FirstOrDefault();
        }

        private bool IsEmailAlreadyRegistered(string email)
        {
            return Db.Query<ApplicationUser>(c => c.Email.ToLower() == email.ToLower()).Any();
        }

        public DIBZ.Common.Model.ApplicationUser AddUpdateUser(int id, string firstName,string surname, string nickName, string email, string password, string mobileNo, string birthYear, string postalCode, string address)
        {
            
            if (IsEmailAlreadyRegistered(email))
            {
                //throw new LogicException("The provided email is already registered. If you are having trouble logging in, please use the 'Forgot Password' to recover your account.");
                throw new LogicException("The provided email is already registered.");
            }
            

                DIBZ.Common.Model.ApplicationUser user = null;
            if (id > 0)
            {
                user = GetUserById(id);
            }
            else
            {
                user = new DIBZ.Common.Model.ApplicationUser();
            }

            user.IsActive = true;
            user.IsDeleted = false;
            user.FirstName = firstName;
            user.LastName = surname;
            user.NickName = nickName;
            user.Email = email;
            user.Password = Helpers.Hash(password);
            user.CellNo = mobileNo;
            user.YearOfBirth = birthYear;
            user.PostalCode = postalCode;
            user.Address = address;
            Db.Add(user);
            Db.Save();
            return user;
        }
        public DIBZ.Common.Model.ApplicationUser EditProfile(int id, string firstName, string lastName,string nickName, string address, string aboutMe, string mobileNo, string birthYear, string postalCode)
        {
            DIBZ.Common.Model.ApplicationUser user = GetUserById(id);

            user.FirstName = firstName;
            user.LastName = lastName;
            user.NickName = nickName;
            user.Address = address;
            user.PostalCode = postalCode;
            user.AboutMe = aboutMe;
            user.CellNo = mobileNo;
            user.YearOfBirth = birthYear;
            Db.Save();
            return user;
        }
        public void AddGameIntoCollection(int applicationUserId,string ids)
        {
            DIBZ.Common.Model.ApplicationUser user = GetUserById(applicationUserId);
            List<int> gameIds = ids.Split(',').Select(int.Parse).ToList();
            foreach (var id in gameIds)
            {
                var game = Db.Query<DIBZ.Common.Model.GameCatalog>(g => g.Id == id).FirstOrDefault();
                user.GameCatalogs.Add(game);
            }
            Db.Save();
        }
        public DIBZ.Common.Model.ApplicationUser EditProfileImage(int id, string profileImageName)
        {
            DIBZ.Common.Model.ApplicationUser user = GetUserById(id);
            if (!string.IsNullOrEmpty(profileImageName))
            {
                user.ProfileImage = new UploadedFile { Filename = profileImageName };
            }
            Db.Save();
            return user;
        }
        public DIBZ.Common.Model.ApplicationUser DeleteProfileImage(int id)
        {
            DIBZ.Common.Model.ApplicationUser user = GetUserById(id);
            user.ProfileImageId = null;
            Db.Save();
            return user;
        }
        public ApplicationUser GetUserById(int id)
        {
            return Db.Query<ApplicationUser>(c => c.Id == id).FirstOrDefault();
        }
        public async Task<ApplicationUser> GetApplicationUserById(int id)
        {
            var userData= await Db.GetObjectById<ApplicationUser>(id);
            return userData;
        }
        public async Task<ApplicationUser> GetAppUserWithScorecardById(int id)
        {
            var scorecardLogic = LogicContext.Create<ScorecardLogic>();

            var appUser = (await Db.Query<ApplicationUser>(c => c.Id == id).Preload(o => o.Scorecard).QueryAsync()).FirstOrDefault();
             appUser.Scorecard = await scorecardLogic.GetScoreCardByAppUserId(appUser.Id);
            return appUser;
        }
        public async Task<ApplicationUser> GetApplicationUserByIdWithUpdateViewCounter(int id)
        {
            var userData = await Db.GetObjectById<ApplicationUser>(id);
            userData.ProfileViewedCounter += 1;
            await Db.SaveAsync();
            return userData;
        }

        public async Task<bool> Delete(int id)
        {
            ApplicationUser User = null;
            if (id > 0)
            {
                User =  GetUserById(id);

            }
            User.IsDeleted = true;
            //We can delete both Hard/soft delete
            //Db.Remove(GameCatalog);
            await Db.SaveAsync();
            return true;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllApplicationUser()
        {
            return await Db.Query<DIBZ.Common.Model.ApplicationUser>(c => !c.IsDeleted).QueryAsync();
        }
        public LoginSession CreateLoginSession(string email, string password, bool isAdminPanel = false)
        {
            ApplicationUser appUser = null;
            if (!isAdminPanel)
                appUser = GetApplicationUserByEmail(email);
            else
            {
                //for admin work

                //appUser = await GetAdminByEmail(request.Email);
                //if (!appUser.IsActive)
                //    throw new LogicException("This user is not active please contact your administrator");
            }

            if (appUser == null || !Helpers.ValidateHash(password, appUser.Password))
                throw new LogicException("The email or the password is incorrect");
                //throw new Exception("The email or the password is incorrect");


            var token = Guid.NewGuid().ToString().Replace("-", string.Empty).ToLower();

            LoginSession loginSession = null;
            if (isAdminPanel)
                loginSession = new LoginSession { Token = token };
            else
                loginSession = new LoginSession { Token = token, ApplicationUser = (ApplicationUser)appUser };
            Db.Add(loginSession);

            Db.Save();
            return loginSession;
        }

        public LoginSession CreateLoginSessionForAdmin(string email, string password, bool isAdminPanel = false)
        {
            Admin admin = null;
            admin = GetAdminByEmail(email);
            if (admin == null || !Helpers.ValidateHash(password, admin.Password))
                throw new LogicException("The email or the password is incorrect");
            //throw new Exception("The email or the password is incorrect");

            var token = Guid.NewGuid().ToString().Replace("-", string.Empty).ToLower();

            LoginSession loginSession = null;

            loginSession = new LoginSession { Token = token, Admin = (Admin)admin };
            //else
            //    loginSession = new LoginSession { Token = token, Admin = (Admin)admin };
            Db.Add(loginSession);
            Db.Save();
            return loginSession;
        }
        public async Task CloseLoginSession(string token)
        {
            var loginSession = (await Db.Query<LoginSession>(l => l.Token == token).QueryAsync()).FirstOrDefault();

            if (loginSession == null)
            {
                throw new LogicException("The specified token is invalid");
            }
            loginSession.LastAccessTime = DateTime.UtcNow;
            loginSession.IsDeleted = true;
            await Db.SaveAsync();
        }

        public async Task<int> ChangePassword(int id,string password)
        {
            ApplicationUser appUser = GetUserById(id);
            appUser.Password = Helpers.Hash(password);
            return await Db.SaveAsync();
        }

        // Shoaib Code
        public async Task<IEnumerable<ApplicationUser>> GetLastWeekRegisterUserCount()
        {
            DateTime dt = DateTime.Now.AddDays(-6);
            return await Db.Query<ApplicationUser>(c => c.CreatedTime >= dt).QueryAsync();
        }

        public async Task<IEnumerable<LoginSession>> GetLastWeekLoginUserCount()
        {
            DateTime dt = DateTime.Now.AddDays(-6);
            return await Db.Query<LoginSession>(c => c.CreatedTime >= dt).QueryAsync();
        }
    }
}

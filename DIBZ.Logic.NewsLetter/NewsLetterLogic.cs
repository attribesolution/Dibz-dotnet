using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIBZ.Common.Model;
using DIBZ.Data;

namespace DIBZ.Logic.NewsLetter
{
    public class NewsLetterLogic : BaseLogic
    {
        public NewsLetterLogic(LogicContext context) : base(context)
        {
        }

        public async Task<bool> AddNewsLetter(string emailAddress)
        {
            DIBZ.Common.Model.NewsLetter newsLetter = new DIBZ.Common.Model.NewsLetter();
            newsLetter.Email = emailAddress;
            newsLetter.IsDeleted = false;
            newsLetter.IsActive = true;
            newsLetter.CreatedTime = DateTime.Now;
            Db.Add(newsLetter);
            await Db.SaveAsync();
            if (newsLetter.Id > 0)
            {
                return true;
            }
            return false;
        }
        public async Task<int> AddPhoneNumberByEmail(string emailAddress, string phoneNumber)
        {
            var newletterData = (await Db.Query<DIBZ.Common.Model.NewsLetter>(o => o.Email == emailAddress).OrderByDescending(o => o.Id).QueryAsync()).FirstOrDefault();
            newletterData.PhoneNumber = phoneNumber;
            return await Db.SaveAsync();
        }
        public DIBZ.Common.Model.NewsLetter GetAddNewsLetterByEmail(string emailAddress)
        {
            return Db.Query<DIBZ.Common.Model.NewsLetter>().Where(x => x.Email == emailAddress).SingleOrDefault();
        }

        public async Task<List<DIBZ.Common.Model.NewsLetter>> GetAllNewsLetterSubscriber()
        {
            var newsLetterData = await Db.Query<DIBZ.Common.Model.NewsLetter>(c => !c.IsDeleted && c.IsActive).OrderByDescending(o => o.CreatedTime).ThenByDescending(o => o.UpdatedTime).QueryAsync();
            return newsLetterData.ToList();
        }

        public async Task AddNotifierEmail(Common.Model.NotifierEmail request)
        {
            NotifierEmail NotifierEmail = new NotifierEmail();
            NotifierEmail.EmailAddress = request.EmailAddress;
            NotifierEmail.IsActive = true;
            Db.Add(NotifierEmail);
            await Db.SaveAsync();
        }

        public async Task<IEnumerable<Common.Model.NotifierEmail>> GetNotifierEmailAddress()
        {
            return await Db.Query<Common.Model.NotifierEmail>(o => o.IsActive && !o.IsDeleted).OrderByDescending(o => o.CreatedTime).QueryAsync();
        }
    }
}

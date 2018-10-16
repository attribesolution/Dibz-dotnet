using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIBZ.Data;

namespace DIBZ.Logic.NewsFeed
{
    public class NewsFeedLogic : BaseLogic
    {
        public NewsFeedLogic(LogicContext context) : base(context)
        {
        }
        public IEnumerable<DIBZ.Common.Model.NewsFeed> GetAllNewsFeed()
        {
            return Db.Query<DIBZ.Common.Model.NewsFeed>(c => !c.IsDeleted && c.IsActive).OrderByDescending(o=>o.CreatedTime).ThenByDescending(o=>o.UpdatedTime).ToList();
        }
        public async Task<List<DIBZ.Common.Model.NewsFeed>> GetAllNewsFeedAdmin()
        {
            var newsFeedData = await Db.Query<DIBZ.Common.Model.NewsFeed>(c => !c.IsDeleted && c.IsActive).OrderByDescending(o => o.CreatedTime).ThenByDescending(o => o.UpdatedTime).QueryAsync();

            return newsFeedData.ToList();
        }
        public async Task<Common.Model.NewsFeed> GetNewsFeedById(int id)
        {
            return (await Db.Query<DIBZ.Common.Model.NewsFeed>(c => c.Id == id).QueryAsync()).FirstOrDefault();
        }

        public async Task AddUpdate(Common.Model.NewsFeed request)
        {
            DIBZ.Common.Model.NewsFeed newsFeed = null;
            if (request.Id > 0)
            {
                newsFeed = await GetNewsFeedById(request.Id);


                newsFeed.News = request.News;
               

            }
            else
            {
                newsFeed = new DIBZ.Common.Model.NewsFeed
                {
                    IsActive = true,
                    News = request.News,
                   
                };

                Db.Add(newsFeed);

            }
            await Db.SaveAsync();

        }

        public async Task Delete(int id)
        {
            DIBZ.Common.Model.NewsFeed newsFeed = null;
            if (id > 0)
            {
                newsFeed = await GetNewsFeedById(id);

            }
            newsFeed.IsDeleted = true;
            //We can delete both Hard/soft delete
            //Db.Remove(GameCatalog);
            await Db.SaveAsync();

        }
    }
}

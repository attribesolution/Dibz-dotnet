using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIBZ.Data;

namespace DIBZ.Logic
{    
     public class CompetitionLogic : BaseLogic
        {
            public CompetitionLogic(LogicContext context) : base(context)
            {

            }
            public async Task<List<DIBZ.Common.Model.Competition>> GetAllCompetition()
            {
                var competitionData = await Db.Query<DIBZ.Common.Model.Competition>(c => !c.IsDeleted).OrderByDescending(o => o.CreatedTime).ThenByDescending(o => o.UpdatedTime).QueryAsync();

                return competitionData.ToList();
            }

            public async Task<DIBZ.Common.Model.Competition> GetFileById(int id)
            {
                return (await Db.Query<DIBZ.Common.Model.Competition>(f => f.Id == id).QueryAsync()).FirstOrDefault();
            }
            public async Task<Common.Model.Competition> GetCompetitionById(int id)
            {
                return (await Db.Query<DIBZ.Common.Model.Competition>(c => c.Id == id).QueryAsync()).FirstOrDefault();
            }
            public async Task<Common.Model.Competition> GetAllCompetitionData()            
            {
            //return await Db.Query<DIBZ.Common.Model.Competition>(c => c.IsActive == true && c.IsDeleted == false).FirstorDefaultAsync();
            return (await Db.Query<DIBZ.Common.Model.Competition>(c => c.IsActive == true && c.IsDeleted == false).QueryAsync()).FirstOrDefault();
        }

        public async Task AddUpdate(Common.Model.Competition request)
            {
                DIBZ.Common.Model.Competition competition = null;
                if (request.Id > 0)
                {

                    competition = await GetCompetitionById(request.Id);
                    competition.Name = request.Name;
                    competition.Title = request.Title;
                    competition.Content = request.Content;
                    if(!String.IsNullOrEmpty(request.FileNewName))
                    {
                    competition.FileOrignalName = request.FileOrignalName;
                    competition.FileNewName = request.FileNewName;
                    }
                    competition.UpdatedTime = DateTime.Now;
                    competition.IsActive = request.IsActive;


                }
                else
                {
                   competition = new DIBZ.Common.Model.Competition
                    {                                                
                        Name = request.Name,
                        Title = request.Title,
                        FileOrignalName = request.FileOrignalName,
                        FileNewName = request.FileNewName,
                        CreatedTime = DateTime.Now,
                        IsActive = request.IsActive,

                    };

                    Db.Add(competition);

                }
                await Db.SaveAsync();

            }

            public async Task<int> UpdateStatus(int bannerId, bool statusCode)
            {
                var myBannerObj = await Db.GetObjectById<DIBZ.Common.Model.Competition>(bannerId);
                myBannerObj.IsActive = statusCode;
                return await Db.SaveAsync();
            }

            public async Task Delete(int id)
            {
                DIBZ.Common.Model.Competition banner = null;
                if (id > 0)
                {
                    banner = await GetCompetitionById(id);

                }
                banner.IsDeleted = true;
                await Db.SaveAsync();

            }
        
        }
 
}

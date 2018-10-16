using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIBZ.Common;
using DIBZ.Common.Model;
using DIBZ.Data;

namespace DIBZ.Logic.NonWorkingDay
{
    public class NonWorkingDayLogic : BaseLogic
    {
        public NonWorkingDayLogic(LogicContext context) : base(context)
        {
        }
        public async Task<List<DIBZ.Common.Model.NonWorkingDay>> GetAllNonWorkingDays()
        {
            var nonWorkingData = await Db.Query<DIBZ.Common.Model.NonWorkingDay>(c => !c.IsDeleted && c.IsActive).QueryAsync();
            
            return nonWorkingData.OrderByDescending(o => o.CreatedTime).ToList();
        }
        public async Task<List<DIBZ.Common.Model.NonWorkingDay>> GetAllNonWorkingDaysPublic()
        {
            var nonWorkingData = await Db.Query<DIBZ.Common.Model.NonWorkingDay>(c => !c.IsDeleted && c.IsActive && c.NonWorkingDate>=DateTime.Now).QueryAsync();

            return nonWorkingData.OrderByDescending(o => o.CreatedTime).ToList();
        }
        public async Task<Common.Model.NonWorkingDay> GetNonWorkingDayId(int id)
        {
            return (await Db.Query<DIBZ.Common.Model.NonWorkingDay>(c => c.Id == id).QueryAsync()).FirstOrDefault();
        }

        public async Task AddUpdate(Common.Model.NonWorkingDay request)
        {
            DIBZ.Common.Model.NonWorkingDay nonWorkingDay = null;
            if (request.Id > 0)
            {
                nonWorkingDay = await GetNonWorkingDayId(request.Id);


                nonWorkingDay.Title= request.Title;
                nonWorkingDay.Reason= request.Reason;
                nonWorkingDay.NonWorkingDate = request.NonWorkingDate;

            }
            else
            {
                nonWorkingDay = new DIBZ.Common.Model.NonWorkingDay
                {
                    IsActive = true,
                    Title = request.Title,
                    Reason=request.Reason,
                    NonWorkingDate=request.NonWorkingDate
                };

                Db.Add(nonWorkingDay);

            }
            await Db.SaveAsync();
            
        }

        public async Task Delete(int id)
        {
            DIBZ.Common.Model.NonWorkingDay nonWorkingDay = null;
            if (id > 0)
            {
                nonWorkingDay = await GetNonWorkingDayId(id);

            }
            nonWorkingDay.IsDeleted = true;
            //We can delete both Hard/soft delete
            //Db.Remove(GameCatalog);
            await Db.SaveAsync();
            
        }
    }
}

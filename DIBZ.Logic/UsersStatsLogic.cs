using DIBZ.Common.Model;
using DIBZ.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIBZ.Logic
{
    public class UsersStatsLogic : BaseLogic
    {
        public UsersStatsLogic(LogicContext context) : base(context)
        {
        }

        // Shoaib Code

        // For Login's count

        public async Task<IEnumerable<LoginSession>> GetTodayLoginCount()
        {
            DateTime startDateTime = DateTime.Today; //Today at 00:00:00
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59

            return await Db.Query<LoginSession>(c => c.CreatedTime >= startDateTime && c.CreatedTime <= endDateTime).QueryAsync();
        }

        public async Task<IEnumerable<LoginSession>> GetLastSevenDayLoginCount()
        {
            DateTime dt = DateTime.Now.AddDays(-6);
            return await Db.Query<LoginSession>(c => c.CreatedTime >= dt).QueryAsync();
        }

        public async Task<IEnumerable<LoginSession>> GetLastFourWeekLoginCount()
        {
            DateTime dt = DateTime.Now.AddDays(-27);
            return await Db.Query<LoginSession>(c => c.CreatedTime >= dt).QueryAsync();
        }

        public async Task<IEnumerable<LoginSession>> GetLastSixMonthLoginCount()
        {
            DateTime dt = DateTime.Now.AddMonths(-6);
            return await Db.Query<LoginSession>(c => c.CreatedTime >= dt).QueryAsync();
        }

        public async Task<IEnumerable<LoginSession>> GetLastOneYearLoginCount()
        {
            DateTime dt = DateTime.Now.AddYears(-1);
            return await Db.Query<LoginSession>(c => c.CreatedTime >= dt).QueryAsync();
        }

        // For Registration count

        public async Task<IEnumerable<ApplicationUser>> GetTodayRegisterCount()
        {
            DateTime startDateTime = DateTime.Today; //Today at 00:00:00
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59

            return await Db.Query<ApplicationUser>(c => c.CreatedTime >= startDateTime && c.CreatedTime <= endDateTime).QueryAsync();

        }

        public async Task<IEnumerable<ApplicationUser>> GetLastSevenDayRegisterCount()
        {
            DateTime dt = DateTime.Now.AddDays(-6);
            return await Db.Query<ApplicationUser>(c => c.CreatedTime >= dt).QueryAsync();
        }

        public async Task<IEnumerable<ApplicationUser>> GetLastFourWeekRegisterCount()
        {
            DateTime dt = DateTime.Now.AddDays(-27);
            return await Db.Query<ApplicationUser>(c => c.CreatedTime >= dt).QueryAsync();
        }

        public async Task<IEnumerable<ApplicationUser>> GetLastSixMonthRegisterCount()
        {
            DateTime dt = DateTime.Now.AddMonths(-6);
            return await Db.Query<ApplicationUser>(c => c.CreatedTime >= dt).QueryAsync();
        }

        public async Task<IEnumerable<ApplicationUser>> GetLastOneYearRegisterCount()
        {
            DateTime dt = DateTime.Now.AddYears(-1);
            return await Db.Query<ApplicationUser>(c => c.CreatedTime >= dt).QueryAsync();
        }

        // For Total Games

        public async Task<IEnumerable<GameCatalog>> GetPS4Count()
        {
            return await Db.Query<GameCatalog>(c => c.FormatId == 3).QueryAsync();
        }

        public async Task<IEnumerable<GameCatalog>> GetXB1Count()
        {
            return await Db.Query<GameCatalog>(c => c.FormatId == 2).QueryAsync();
        }

        public async Task<IEnumerable<GameCatalog>> GetNDSCount()
        {
            return await Db.Query<GameCatalog>(c => c.FormatId == 4).QueryAsync();
        }

        public async Task<IEnumerable<GameCatalog>> GetNSWCount()
        {
            return await Db.Query<GameCatalog>(c => c.FormatId == 1).QueryAsync();
        }

        //public async Task<IEnumerable<IGrouping<String, GameCatalog>>> GetPS4Count()
        //{
        //    return await Db.Query<GameCatalog>(c => c.FormatId == 3).GroupBy(c => c.Name).QueryAsync();
        //}

        //public async Task<IEnumerable<IGrouping<String, GameCatalog>>> GetXB1Count()
        //{
        //    return await Db.Query<GameCatalog>(c => c.FormatId == 2).GroupBy(c => c.Name).QueryAsync();
        //}

        //public async Task<IEnumerable<IGrouping<String, GameCatalog>>> GetNDSCount()
        //{
        //    return await Db.Query<GameCatalog>(c => c.FormatId == 4).GroupBy(c => c.Name).QueryAsync();
        //}

        //public async Task<IEnumerable<IGrouping<String, GameCatalog>>> GetNSWCount()
        //{
        //    return await Db.Query<GameCatalog>(c => c.FormatId == 1).GroupBy(c => c.Name).QueryAsync();
        //}
    }
}

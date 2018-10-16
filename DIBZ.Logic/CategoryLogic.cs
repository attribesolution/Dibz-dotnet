using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIBZ.Common.Model;
using DIBZ.Data;

namespace DIBZ.Logic
{
    public class CategoryLogic:BaseLogic
    {
        public CategoryLogic(LogicContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            return await Db.Query<Category>(o => o.IsActive && !o.IsDeleted).QueryAsync();
        }
    }
}

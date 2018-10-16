using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIBZ.Common.Model;
using DIBZ.Data;

namespace DIBZ.Logic
{
    public class FormatLogic: BaseLogic
    {
        public FormatLogic(LogicContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Format>> GetAllFormats()
        {
            return await Db.Query<Format>(o=>o.IsActive && !o.IsDeleted).QueryAsync();
        }
    }
}

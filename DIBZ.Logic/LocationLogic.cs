using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIBZ.Common.Model;
using DIBZ.Data;

namespace DIBZ.Logic
{
    public class LocationLogic: BaseLogic
    {
        public LocationLogic(LogicContext context) : base(context)
        {
        }
        public IEnumerable <DIBZLocation> GetAllLocations()
        {
            return Db.Query<DIBZLocation>(o => o.IsActive && !o.IsDeleted);
        }
    }
}

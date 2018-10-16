using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIBZ.Data;

namespace DIBZ.Logic
{
    public abstract class BaseLogic
    {
        private LogicContext _context;

        protected LogicContext LogicContext
        {
            get
            {
                return _context;
            }
        }

        protected DataContext Db
        {
            get
            {
                return _context.DataContext;
            }
        }

        public BaseLogic(LogicContext context)
        {
            _context = context;
        }
    }
}

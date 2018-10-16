using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIBZ.Data;

namespace DIBZ.Logic
{
    public class LogicContext : IDisposable
    {
        private DataContext _context;

        internal DataContext DataContext
        {
            get
            {
                return _context;
            }
        }

        public LogicContext()
        {
            _context = new DataContext();
        }

        public T Create<T>() where T : BaseLogic
        {
            return (T)Activator.CreateInstance(typeof(T), this);
        }

        public void Save()
        {
            DataContext.Save();
        }

        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
                _context = null;
            }
        }
    }
}

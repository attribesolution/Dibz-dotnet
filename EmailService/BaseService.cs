using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIBZ.Logic;

namespace DIBZ.EmailService
{
    public abstract class BaseService : IDisposable
    {
        private LogicContext _context;
        protected LogicContext Context
        {
            get
            {
                return _context;
            }
        }
        protected int Elapsed { get; set; }

        public BaseService()
        {
            _context = new LogicContext();
        }


        //public async Task Tick()
        //{
        //    Elapsed += ServiceRunner.TickInterval;
        //    try
        //    {
        //        var resetElapsed = await Run();
        //        if (resetElapsed)
        //            Elapsed = 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        Elapsed = 0;
        //        throw new ServiceException(this.GetType().Name, "An error occurred while running service", ex);
        //    }
        //}

        public void Dispose()
        {
            _context.Dispose();
        }

        //protected abstract Task<bool> Run();

    }
}

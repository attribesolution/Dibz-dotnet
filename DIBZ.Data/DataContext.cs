using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Diagnostics;
using DIBZ.Common.Model;

namespace DIBZ.Data
{
    public class DataContext : IDisposable
    {
        private DIBZDbContext _context;

        public DataContext()
        {
            _context = new DIBZDbContext();
        }

        public void Add<T>(T obj) where T : class
        {
            var dbSet = _context.Set<T>();
            dbSet.Add(obj);
        }

        public void AddAll<T>(IEnumerable<T> obj) where T : class
        {
            var dbSet = _context.Set<T>();
            dbSet.AddRange(obj);
        }

        public void Remove<T>(T obj) where T : class
        {
            var dbSet = _context.Set<T>();
            dbSet.Remove(obj);
        }

        public void RemoveAll<T>(IEnumerable<T> obj) where T : class
        {
            var dbSet = _context.Set<T>();
            dbSet.RemoveRange(obj);
        }

        public async Task<T> GetObjectById<T>(int id) where T : BaseModelObject
        {
            var dbSet = _context.Set<T>();
            return await dbSet.FindAsync(id);
        }

        public IQueryable<T> Query<T>(Expression<Func<T, bool>> query = null) where T : class
        {
            var dbSet = _context.Set<T>();
            return query != null ? dbSet.Where(query) : dbSet;
        }

        public int Save()
        {
            SaveInner();
            return _context.SaveChanges();
        }
        /// <summary>
        /// Assumming site will be host in UK that is why we are using current date instead of UTC
        /// </summary>
        private void SaveInner()
        {
            var createdObjects = _context.ChangeTracker.Entries<BaseModelObject>().Where(e => e.State == EntityState.Added).Select(e => e.Entity);

            foreach (var obj in createdObjects)
            {
                obj.CreatedTime = DateTime.UtcNow;
            }

            var updatedObjects = _context.ChangeTracker.Entries<BaseModelObject>().Where(e => e.State == EntityState.Modified).Select(e => e.Entity);

            foreach (var obj in updatedObjects)
            {
                obj.UpdatedTime = DateTime.UtcNow;
            }
        }

        public async Task<int> SaveAsync()
        {
            SaveInner();
            return await _context.SaveChangesAsync();
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

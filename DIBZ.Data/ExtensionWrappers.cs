using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DIBZ.Data
{
    public static class ExtensionWrappers
    {
        public async static Task<IEnumerable<T>> QueryAsync<T>(this IQueryable<T> query) where T : class
        {
            return await query.ToListAsync();
        }
        public async static Task<IEnumerable<T>> FirstorDefaultAsync<T>(this IQueryable<T> query) where T : class
        {
            return await query.FirstorDefaultAsync();
        }

        public static IQueryable<T> Preload<T, P>(this IQueryable<T> query, Expression<Func<T, P>> path) where T : class
        {
            return query.Include(path);
        }
    }
}

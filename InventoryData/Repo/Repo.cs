using InventoryData.Data;
using InventoryData.IRepo;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace InventoryData.Repo
{

    public class Repo<T> : IRepo<T> where T : class
    {
        private readonly AppDbContext _context;
        internal DbSet<T> dbset;


        public Repo(AppDbContext appDbContext) 
        {
            _context = appDbContext;
            dbset = _context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await dbset.AddAsync(entity);
        }



        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await dbset.AddRangeAsync(entities);
        }



        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeOthers = null)
        {
            IQueryable<T> query = dbset;

            if (filter != null)
                query = query.Where(filter);

            if (includeOthers != null)
            {
                foreach (var item in includeOthers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item.Trim());
                }
            }

            return await query.ToListAsync();
        }



        public async Task<T?> GetOneAsync(Expression<Func<T, bool>> filter, string? includeOthers = null)
        {
            IQueryable<T> query = dbset;

            query = query.Where(filter); //.AsNoTracking() 

            if (!string.IsNullOrWhiteSpace(includeOthers))
            {
                var includes = includeOthers.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var include in includes)
                {
                    query = query.Include(include.Trim());
                }
            }

            return await query.FirstOrDefaultAsync();
        }



        public void Remove(T entity)
        {
            dbset.Remove(entity);
        }



        public void RemoveRange(IEnumerable<T> entities)
        {
            dbset.RemoveRange(entities);
        }

      

        public void UpdateRange(IEnumerable<T> entities)
        {
            dbset.UpdateRange(entities);
        }
    }
}

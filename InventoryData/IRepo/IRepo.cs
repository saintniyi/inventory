using System.Linq.Expressions;


namespace InventoryData.IRepo
{
    public interface IRepo<T> where T : class
    {
        Task AddAsync(T entity);

        Task AddRangeAsync(IEnumerable<T> entities);

        Task<T?> GetOneAsync(Expression<Func<T, bool>> filter, string? includeOthers = null);

        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeOthers = null);

        void Remove(T entity);

        void RemoveRange(IEnumerable<T> entities);

        void UpdateRange(IEnumerable<T> entities);




    }
}

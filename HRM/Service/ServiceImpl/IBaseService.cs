namespace HRM.Service.ServiceImpl;

public interface IBaseService<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(int id);
    Task<IEnumerable<TEntity?>> GetAllAsync();
    Task<TEntity?> AddAsync(TEntity? entity);
    Task UpdateAsync(TEntity? entity);
    Task DeleteAsync(int id);
}
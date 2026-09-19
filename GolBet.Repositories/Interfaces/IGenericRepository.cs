using GolBet.Entities.Common;

namespace GolBet.Repositories.Interfaces;

public interface IGenericRepository<T>
    where T : AuditableEntity
{
    Task<IEnumerable<T>> GetAllAsync(bool includeInactive = false);

    Task<T?> GetByIdAsync(int id);

    Task<T> AddAsync(T entity);

    Task UpdateAsync(T entity);

    Task DeactivateAsync(int id);
}

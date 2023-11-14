using Appointment_Scheduler.Model;

namespace Appointment_Scheduler.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        IQueryable<T> GetAllAsQueryable();
        Task<T> GetById(Guid id);
        Task<T> Create(T entity);
        Task<bool> Exists(Guid id);
        Task Update(T entity);
        Task Delete(T entity);
        Task Save();
    }
}
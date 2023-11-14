using Appointment_Scheduler.Data;
using Appointment_Scheduler.Model;
using Microsoft.EntityFrameworkCore;

namespace Appointment_Scheduler.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppointmentDbContext _dbcontext;

        public GenericRepository(AppointmentDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public async Task<T> Create(T entity)
        {
            await _dbcontext.AddAsync(entity);
            return entity;
        }

        public async Task Delete(T entity)
        {
            _dbcontext.Set<T>().Remove(entity);
        }

        public async Task<bool> Exists(Guid id)
        {
            var entity = await GetById(id);
            return entity != null;
        }

        public IQueryable<T> GetAllAsQueryable()
        {
            return _dbcontext.Set<T>().AsQueryable();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbcontext.Set<T>().ToListAsync();
        }

        public async Task<T> GetById(Guid id)
        {
            return await _dbcontext.Set<T>().FindAsync(id);
        }

        public async Task Save()
        {
            await _dbcontext.SaveChangesAsync();
        }

        public async Task Update(T entity)
        {
            _dbcontext.Entry(entity).State = EntityState.Modified;
        }
    }
}

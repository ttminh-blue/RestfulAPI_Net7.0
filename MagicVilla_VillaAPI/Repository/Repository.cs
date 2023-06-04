using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _db;
        internal DbSet<T> dbSet;
        public Repository(AppDbContext db)
        {
            _db = db; 
            this.dbSet = _db.Set<T>();  
        }
        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }
        public async Task Create(T entity)
        {
            await dbSet.AddAsync(entity);
            await Save();
        }
        public async Task Update(T entity)
        {
            dbSet.Update(entity);
            await Save();
        }
        public async Task<T> GetOne(Expression<Func<T, bool>> filter = null, bool tracked = true)
        {
            IQueryable<T> query = dbSet;
            if (!tracked)
            {
                query = query.AsNoTracking();
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            
            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<T>> GetAll(Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            var result = await query.ToListAsync();
            return result;

        }
        public async Task Remove(T entity)
        {
            dbSet.Remove(entity);
            await Save();
        }

        public async Task<int> GetLength()
        {
            var length = dbSet.Count();
            return length;
        }
    }
}

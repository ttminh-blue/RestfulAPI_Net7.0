using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace MagicVilla_VillaAPI.Repository
{
    public class VillaRepository : Repository<Villa>, IVillaRepository
    {
            private readonly AppDbContext _db;
            public VillaRepository(AppDbContext db) : base(db)
            {
                _db = db;
            }
           
            public async Task<Villa> Update(Villa entity)
            {
                entity.UpdatedDate = DateTime.Now;      
                _db.villas.Update(entity);
                await _db.SaveChangesAsync();
                return entity;
            }   
           

    }
}

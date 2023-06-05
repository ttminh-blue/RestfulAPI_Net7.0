using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace MagicVilla_VillaAPI.Repository
{
    public class VillaNumberRepository : Repository<VillaNumber>, IVillaNumberRepository
    {
            private readonly AppDbContext _db;
            public VillaNumberRepository(AppDbContext db) : base(db)
            {
                _db = db;
            }
           
            public async Task<VillaNumber> Update(VillaNumber entity)
            {
                entity.UpdatedDate = DateTime.Now;      
                _db.villaNumber.Update(entity);
                await _db.SaveChangesAsync();
                return entity;
            }   
           

    }
}

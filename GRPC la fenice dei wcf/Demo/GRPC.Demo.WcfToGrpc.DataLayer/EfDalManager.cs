using GRPC.Demo.WcfToGrpc.DomainModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRPC.Demo.WcfToGrpc.DataLayer
{
    public static class EfDalManager
    {
        public static async Task<Libro> GetById(Guid id,DbSet<Libro> context)
        {
            return await context.AsNoTracking().Include("Autore").FirstOrDefaultAsync(i => i.Id.Equals(id));
        }

        public static async Task<IEnumerable<Libro>> GetAll(DbSet<Libro> context)
        {
            return await context.AsNoTracking().Include("Autore").ToListAsync();
        }

    }
}

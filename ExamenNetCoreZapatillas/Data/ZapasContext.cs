using ExamenNetCoreZapatillas.Models;
using Microsoft.EntityFrameworkCore;

namespace ExamenNetCoreZapatillas.Data
{
    public class ZapasContext: DbContext
    {
        public ZapasContext(DbContextOptions<ZapasContext> options) : base(options) { }

        public DbSet<Zapatilla> Zapatillas { get; set; }
        public DbSet<ImagenZapatilla> ImagenZapatillas { get; set; }
    }
}

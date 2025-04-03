using Microsoft.EntityFrameworkCore;

namespace L02P02_2022PM650_2022SG650.Models
{
    public class libreriaDbContext : DbContext
    {
        public libreriaDbContext(DbContextOptions<libreriaDbContext> options) : base(options)
        { 

        }
    }
}

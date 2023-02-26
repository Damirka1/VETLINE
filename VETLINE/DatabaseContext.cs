using Microsoft.EntityFrameworkCore;

namespace VETLINE
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }
    }
}

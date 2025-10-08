using Microsoft.EntityFrameworkCore;

namespace FCG.Infrastructure.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }


    }
}
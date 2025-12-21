using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace TransitApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    }
}
using Microsoft.EntityFrameworkCore;

namespace ITBus.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Використовуємо повні шляхи до класів
        public DbSet<ITBus.Models.Seat> Seats { get; set; }

        // Тут точно не буде конфлікту, бо ми вказали повний шлях
        public DbSet<ITBus.Models.Route> Routes { get; set; }
    }
}
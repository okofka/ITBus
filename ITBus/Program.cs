using Microsoft.EntityFrameworkCore;
using ITBus.Data;
using ITBus.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();

// --- ј¬“ќћј“»„Ќ≈ «јѕќ¬Ќ≈ЌЌя (SEEDING) ---
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();

    if (!context.Routes.Any())
    {
        // 1. —творюЇмо маршрути
        var routes = new List<ITBus.Models.Route>
        {
            new ITBus.Models.Route { Number = "603", Name = "Ћьв≥в - ўирець" },
            new ITBus.Models.Route { Number = "118", Name = "Ћьв≥в - —трий" },
            new ITBus.Models.Route { Number = "113", Name = "Ћьв≥в - ѕотелич" },
            new ITBus.Models.Route { Number = "614", Name = "Ћьв≥в - ∆идач≥в" }
        };
        context.Routes.AddRange(routes);
        context.SaveChanges(); // «бер≥гаЇмо, щоб отримати Id маршрут≥в

        // 2. —творюЇмо м≥сц€ дл€  ќ∆Ќќ√ќ маршруту
        var allSeats = new List<ITBus.Models.Seat>();
        foreach (var route in routes)
        {
            for (int i = 1; i <= 20; i++)
            {
                allSeats.Add(new ITBus.Models.Seat
                {
                    RouteId = route.Id, // ѕрив'€зуЇмо до конкретного маршруту
                    Number = i,
                    IsOccupied = false
                });
            }
        }
        context.Seats.AddRange(allSeats);
        context.SaveChanges();
        Console.WriteLine("Ѕаза даних оновлена: ћаршрути та м≥сц€ створено!");
    }
}
// ----------------------------------------

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();
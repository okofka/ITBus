using ITBus.Data;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using ITBus.Data;
using ITBus.Models;
using Route = ITBus.Models.Route;

namespace ITBus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RouteController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RouteController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("list")]
        public IActionResult GetAllRoutes()
        {
            return Ok(_context.Routes.ToList());
        }

        // ВИПРАВЛЕНО: Прості назви зупинок, щоб збігалися з фронтендом
        private static readonly Dictionary<string, List<string>> RouteStopsData = new()
        {
            { "603", new List<string> { "Львів", "Сокільники", "Наварія", "Пустомити", "Семенівка", "Щирець" } },
            { "118", new List<string> { "Львів", "Солонка", "Липники", "Дроговиж", "Стрий" } },
            { "113", new List<string> { "Львів", "Грибовичі", "Куликів", "Жовква", "Потелич" } },
            { "614", new List<string> { "Львів", "Зубра", "Жирівка", "Бібрка", "Жидачів" } }
        };

        [HttpGet]
        public IActionResult GetRoute([FromQuery] string? currentStop, [FromQuery] string routeNumber = "603")
        {
            if (!RouteStopsData.ContainsKey(routeNumber)) return NotFound("Маршрут не знайдено");
            var fullRoute = RouteStopsData[routeNumber];

            if (string.IsNullOrEmpty(currentStop)) return Ok(new { stops = fullRoute });

            int index = fullRoute.IndexOf(currentStop);
            if (index == -1) return Ok(new { stops = fullRoute });

            return Ok(new { stops = fullRoute.Skip(index).ToList() });
        }


        [HttpGet("price")]
        public IActionResult GetPrice(string routeNumber, string from, string to)
        {
            if (!RouteStopsData.ContainsKey(routeNumber)) return Ok(new { price = 0 });

            var stops = RouteStopsData[routeNumber];
            int startIdx = stops.IndexOf(from);
            int endIdx = stops.IndexOf(to);

            // Якщо зупинки не знайдено або ми їдемо "назад"
            if (startIdx == -1 || endIdx == -1 || endIdx <= startIdx)
                return Ok(new { price = 0 });

            // Тариф: 20 грн посадка + 10 грн за кожну зупинку
            int distance = endIdx - startIdx;
            int price = 20 + (distance * 10);

            return Ok(new { price = price });
        }
    }
}
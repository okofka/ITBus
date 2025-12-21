using ITBus.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using ITBus.Data;
using ITBus.Models;

namespace ITBus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SeatController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/seat?routeNumber=603
        [HttpGet]
        public IActionResult GetSeats([FromQuery] string routeNumber)
        {
            var route = _context.Routes.FirstOrDefault(r => r.Number == routeNumber);
            if (route == null) return NotFound("Маршрут не знайдено");

            var seats = _context.Seats
                .Where(s => s.RouteId == route.Id)
                .OrderBy(s => s.Number)
                .ToList();

            return Ok(seats);
        }

        // POST: api/seat/book?seatNumber=5&routeNumber=603
        [HttpPost("book")]
        public IActionResult BookSeat([FromQuery] int seatNumber, [FromQuery] string routeNumber)
        {
            var route = _context.Routes.FirstOrDefault(r => r.Number == routeNumber);
            if (route == null) return NotFound("Маршрут не знайдено");

            var seat = _context.Seats.FirstOrDefault(s => s.Number == seatNumber && s.RouteId == route.Id);

            if (seat == null) return NotFound("Місце не знайдено");
            if (seat.IsOccupied) return BadRequest("Місце вже зайняте!");

            var ticketId = Guid.NewGuid().ToString();
            seat.IsOccupied = true;
            seat.TicketId = ticketId;
            _context.SaveChanges();

            return Ok(new { message = "Успішно", ticketId = ticketId });
        }

        // Валідація
        [HttpPost("validate")]
        public IActionResult ValidateTicket([FromQuery] string ticketId, [FromQuery] int seatNumber, [FromQuery] string routeNumber)
        {
            // 1. Знаходимо маршрут, який вибрав водій (наприклад, 118)
            var route = _context.Routes.FirstOrDefault(r => r.Number == routeNumber);
            if (route == null) return NotFound("Маршрут водія не знайдено");

            // 2. Шукаємо місце з таким номером САМЕ В ЦЬОМУ маршруті
            var seat = _context.Seats.FirstOrDefault(s => s.Number == seatNumber && s.RouteId == route.Id);

            if (seat == null) return NotFound($"Місце {seatNumber} не існує в автобусі №{routeNumber}");

            if (!seat.IsOccupied) return BadRequest("Це місце вільне (Заєць?) 🐰");

            // 3. Звіряємо ID квитка
            if (seat.TicketId != ticketId)
                return BadRequest("Фейковий або старий квиток! ❌");

            return Ok(new { valid = true, message = $"Місце {seatNumber}: ОПЛАЧЕНО ✅" });
        }
    }
}
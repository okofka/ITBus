namespace ITBus.Models
{
    public class Seat
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public bool IsOccupied { get; set; }
        public string? TicketId { get; set; }

        // Повертаємо прямий зв'язок з маршрутом
        public int RouteId { get; set; }
    }
}
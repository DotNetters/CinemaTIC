using System.Collections.Generic;

namespace Cinematic.Web.Models
{
    public class TicketSellingIndexViewModel
    {
        /// <summary>
        /// Inicializa una instancia de <see cref="TicketSellingIndexViewModel"/>
        /// </summary>
        public TicketSellingIndexViewModel()
        {
            AvailableSessions = new List<Session>();
            AvailableSeats = new List<Seat>();
            SelectedSession = new Session();
        }

        /// <summary>
        /// Sesiones disponibles para la venta de entradas
        /// </summary>
        public IEnumerable<Session> AvailableSessions { get; set; }

        /// <summary>
        /// Butacas disponibles para la sesión seleccionada
        /// </summary>
        public IEnumerable<Seat> AvailableSeats { get; set; }

        /// <summary>
        /// Sesión seleccionada para la venta de entradas
        /// </summary>
        public Session SelectedSession { get; set; }
    }
}
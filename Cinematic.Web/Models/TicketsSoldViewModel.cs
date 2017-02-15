using System.Collections.Generic;

namespace Cinematic.Web.Models
{
    public class TicketsSoldViewModel
    {
        /// <summary>
        /// Inicializa una instancia de <see cref="TicketsSoldViewModel"/>
        /// </summary>
        public TicketsSoldViewModel()
        {
            Errors = new List<string>();
            Tickets = new List<Ticket>();
        }

        /// <summary>
        /// Entradas emitidas
        /// </summary>
        public IList<Ticket> Tickets { get; set; }

        /// <summary>
        /// Errores que han impedido que la cventa se realice
        /// </summary>
        public IList<string> Errors { get; set; }
    }
}
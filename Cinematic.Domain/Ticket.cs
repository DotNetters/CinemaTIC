using TicketMan.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketMan.Core
{
    /// <summary>
    /// Ticket o entrada que se vende y representa la reserva de una butaca
    /// </summary>
    public class Ticket : IBusinessEntity
    {
        /// <summary>
        /// Identificador interno del ticket en el sistema
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// <see cref="Seat">Butaca</see> reservada en el sistema por este ticket
        /// </summary>
        public Seat Seat { get; set; }

        /// <summary>
        /// Precio de venta del ticket
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// Fecha y hora en la que se ha vendido el ticket
        /// </summary>
        public DateTime TimeAndDate { get; set; }
    }
}

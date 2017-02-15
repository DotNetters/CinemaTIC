using System;

namespace Cinematic.Contracts
{
    /// <summary>
    /// Servicio que gestiona la venta de tickets
    /// </summary>
    public interface ITicketManager
    {
        /// <summary>
        /// Cancela la venta de un <see cref="Ticket"/>, quedando su reserva sin efecto en el sistema
        /// </summary>
        /// <param name="ticket"><see cref="Ticket"/> que se va a anular</param>
        void CancelTicket(Ticket ticket);
        /// <summary>
        /// Ejecuta la venta de un <see cref="Ticket"/>, 
        /// para una <see cref="Seat">butaca</see> determinada 
        /// </summary>
        /// <param name="seat">Butaca reservada por el <see cref="Ticket"/></param>
        /// <returns>
        /// <see cref="Ticket"/> resultante de la operación, 
        /// que constituye la reserva de la <see cref="Seat">butaca</see> 
        /// </returns>
        Ticket SellTicket(Seat seat);
    }
}

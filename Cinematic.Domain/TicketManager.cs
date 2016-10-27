using Cinematic.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cinematic.Resources;

namespace Cinematic.Core
{
    /// <summary>
    /// Servicio que gestiona la venta de tickets
    /// </summary>
    public class TicketManager : ITicketManager
    {
        ISeatManager _seatManager = null;
        IPriceManager _priceManager = null;
        IDataContext _dataContext = null;

        /// <summary>
        /// Inicializa una intancia de <see cref="TicketManager"/>
        /// </summary>
        /// <param name="seatManager">Instancia de <see cref="ISeatManager"/></param>
        /// <param name="priceManager">Instancia de <see cref="IPriceManager"/></param>
        /// <param name="dataContext">Instancia de <see cref="IDataContext"/></param>
        public TicketManager(ISeatManager seatManager, IPriceManager priceManager, IDataContext dataContext)
        {
            if (seatManager == null)
                throw new ArgumentNullException("seatManager");
            if (priceManager == null)
                throw new ArgumentNullException("priceManager");
            if (dataContext == null)
                throw new ArgumentNullException("dataContext");

            _seatManager = seatManager;
            _priceManager = priceManager;
            _dataContext = dataContext;
        }

        /// <inheritdoc />
        public Ticket SellTicket(Seat seat)
        {
            if (seat == null)
                throw new ArgumentNullException("seat");

            if (seat.Session == null)
                throw new ArgumentNullException("seat.Session");

            if (seat.Session.Status == SessionStatus.Closed)
                throw new TicketManCoreException(Messages.SessionIsClosedNoTicketsAvailable);

            if (seat.Session.Status == SessionStatus.Cancelled)
                throw new TicketManCoreException(Messages.SessionIsCancelledNoTicketsAvailable);

            var allocatedSeat = _seatManager.AllocateSeat(seat);

            var newTicket = new Ticket()
            {
                Price = _priceManager.GetTicketPrice(allocatedSeat.Session, allocatedSeat.Row, allocatedSeat.SeatNumber),
                Seat = allocatedSeat,
                TimeAndDate = DateTime.Now
            };

            _dataContext.Add(newTicket);

            return (newTicket);
        }

        /// <inheritdoc />
        public void CancelTicket(Ticket ticket)
        {
            if (ticket == null)
                throw new ArgumentNullException("ticket");

            if (ticket.Seat == null)
                throw new ArgumentNullException("ticket.Seat");

            if (ticket.Seat.Session == null)
                throw new ArgumentNullException("ticket.Seat.Session");

            if (ticket.Seat.Session.Status == SessionStatus.Closed)
                throw new TicketManCoreException(Messages.SessionIsClosedCannotReturnTickets);

            _seatManager.DeAllocateSeat(ticket.Seat);

            _dataContext.Remove(ticket);
        }
    }
}

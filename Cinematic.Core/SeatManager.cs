using Cinematic.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cinematic.Resources;

namespace Cinematic
{
    /// <summary>
    /// Servicio que gestiona la disponibilidad de butacas y su reserva
    /// </summary>
    public class SeatManager : ISeatManager
    {
        IDataContext _dataContext = null;

        /// <summary>
        /// Inicializa una instancia de <see cref="SeatManager"/>
        /// </summary>
        public SeatManager(IDataContext dataContext)
        {
            if (dataContext == null)
                throw new ArgumentNullException("dataContext");

            _dataContext = dataContext;
        }

        /// <inheritdoc />
        public Seat GetSeat(Session session, int row, int seatNumber)
        {
            if (session == null)
                throw new ArgumentNullException("session");

            if (row > Session.NUMBER_OF_ROWS)
                throw new CinematicException(Messages.RowNumberIsAboveMaxAllowed);

            if (row < 1)
                throw new CinematicException(Messages.RowNumberIsBelowMinAllowed);

            if (seatNumber > Session.NUMBER_OF_SEATS)
                throw new CinematicException(Messages.SeatNumberIsAboveMaxAllowed);

            if (seatNumber < 1)
                throw new CinematicException(Messages.SeatNumberIsBelowMinAllowed);

            var q = from s in _dataContext.Seats
                    where
                        s.Row == row &&
                        s.SeatNumber == seatNumber &&
                        s.Session == session
                    select s;

            var seat = q.SingleOrDefault();

            if (seat == null)
            {
                seat = new Seat()
                {
                    Row = row,
                    SeatNumber = seatNumber,
                    Session = session
                };
            }

            return seat;
        }

        /// <inheritdoc />
        public IEnumerable<Seat> GetAvailableSeats(Session session)
        {
            if (session == null)
                throw new ArgumentNullException("session");

            var retVal = new List<Seat>();

            if (session.Status == SessionStatus.Closed)
                return retVal;

            if (session.Status == SessionStatus.Cancelled)
                return retVal;

            var q = from s in _dataContext.Seats.AsQueryable().Include(s => s.Session)
                    where s.Session.Id == session.Id
                    select s;

            var reservedSeats = q.ToList();

            for (int row = 1; row <= Session.NUMBER_OF_ROWS; row++)
            {
                for (int seatNumber = 1; seatNumber <= Session.NUMBER_OF_SEATS; seatNumber++)
                {
                    var seat = reservedSeats.Where(s => s.Row == row && s.SeatNumber == seatNumber).SingleOrDefault();
                    if (seat == null)
                    {
                        retVal.Add(new Seat() { Row = row, SeatNumber = seatNumber, Session = session });
                    }
                    else
                    {
                        retVal.Add(seat);
                    }
                }
            }

            return retVal;
        }

        /// <inheritdoc />
        public Seat AllocateSeat(Seat seat)
        {
            if (seat == null)
                throw new ArgumentNullException("seat");

            var reservedSeat = _dataContext.Seats
                .Where(s => s.Session == seat.Session)
                .Where(s => s.Row == seat.Row && s.SeatNumber == seat.SeatNumber).SingleOrDefault();

            if (reservedSeat != null)
                throw new CinematicException(Messages.SeatIsPreviouslyReserved);

            seat.Reserved = true;

            _dataContext.Add(seat);

            return (seat);
        }

        /// <inheritdoc />
        public Seat DeallocateSeat(Seat seat)
        {
            if (seat == null)
                throw new ArgumentNullException("seat");

            var reservedSeat = _dataContext.Seats
                .Where(s => s.Session == seat.Session)
                .Where(s => s.Row == seat.Row && s.SeatNumber == seat.SeatNumber).SingleOrDefault();

            if (reservedSeat == null)
                throw new CinematicException(Messages.SeatIsNotReserved);

            seat.Reserved = false;

            _dataContext.Remove(seat);

            return (seat);
        }
    }
}

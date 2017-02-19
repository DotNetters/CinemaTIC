using Cinematic.Contracts;
using Cinematic.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinematic
{
    /// <summary>
    /// Servicio que gestiona las sesiones para las que se ponen <see cref="Ticket">tickets</see> a la venta
    /// </summary>
    public class SessionManager : ISessionManager
    {
        IDataContext _dataContext = null;

        /// <summary>
        /// Inicializa una instancia de <see cref="SessionManager"/>
        /// </summary>
        /// <param name="dataContext">Contexto de acceso a datos</param>
        public SessionManager(IDataContext dataContext)
        {
            if (dataContext == null)
                throw new ArgumentNullException("dataContext");

            _dataContext = dataContext;
        }

        /// <inheritdoc />
        public IEnumerable<Session> GetAvailableSessions()
        {
            return _dataContext.Sessions.Where(s => s.Status == SessionStatus.Open);
        }

        /// <inheritdoc />
        public Session CreateSession(DateTime timeAndDate)
        {
            var session = new Session();

            session.Status = SessionStatus.Open;
            session.TimeAndDate = timeAndDate;

            _dataContext.Add(session);

            return session;
        }

        /// <inheritdoc />
        public Session CloseSession(Session session)
        {
            if (session == null)
                throw new ArgumentNullException("session");

            session.Status = SessionStatus.Closed;

            return session;
        }

        /// <inheritdoc />
        public Session CancelSession(Session session)
        {
            if (session == null)
                throw new ArgumentNullException("session");

            session.Status = SessionStatus.Cancelled;

            return session;
        }

        /// <inheritdoc />
        public Session RemoveSession(Session session)
        {
            if (session == null)
                throw new ArgumentNullException("session");

            var q = _dataContext.Tickets.AsQueryable().Include(t => t.Seat).Where(t => t.Seat.Session.Id == session.Id);

            var hasTickets = q.FirstOrDefault() != null;

            if (hasTickets)
            {
                throw new CinematicException(
                    string.Format(Messages.SessionCannotBeRemovedBecauseItHasSoldTickets, session.TimeAndDate.ToString("dd/MM/yyyy HH:mm")));
            }
            else
            {
                _dataContext.Remove(session);
            }

            return session;
        }
    }
}

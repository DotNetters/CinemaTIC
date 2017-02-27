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
    /// Servicio que gestiona las sesiones para las que se ponen <see cref="Ticket">entradas</see> a la venta
    /// </summary>
    public class SessionManager : ISessionManager
    {

        IDataContext DataContext { get; set; } = null;

        /// <summary>
        /// Inicializa una instancia de <see cref="SessionManager"/>
        /// </summary>
        /// <param name="dataContext">Contexto de acceso a datos</param>
        public SessionManager(IDataContext dataContext)
        {
            if (dataContext == null)
                throw new ArgumentNullException("dataContext");

            DataContext = dataContext;
        }

        /// <inheritdoc />
        public IEnumerable<Session> GetAvailableSessions()
        {
            return DataContext.Sessions.Where(s => s.Status == SessionStatus.Open);
        }

        /// <inheritdoc />
        public Session Get(int id)
        {
            return DataContext.Find<Session>(id);
        }

        /// <inheritdoc />
        public IEnumerable<Session> GetAll()
        {
            return DataContext.Sessions;
        }

        /// <inheritdoc />
        public SessionsPageInfo GetAll(int page, int sessionsPerPage)
        {
            var pageCount = Math.Ceiling((double)DataContext.Sessions.Count() / sessionsPerPage);
            var sessions = DataContext.Sessions.Skip((page - 1) * 10).Take(10);

            var pageInfo = new SessionsPageInfo(pageCount, sessions);

            return pageInfo;
        }

        /// <inheritdoc />
        public Session CreateSession(DateTime timeAndDate)
        {
            var session = new Session();

            CheckDupedSession(timeAndDate);

            session.TimeAndDate = timeAndDate;

            DataContext.Add(session);

            return session;
        }

        /// <inheritdoc />
        public Session UpdateSessionTimeAndDate(int sessionId, DateTime timeAndDate)
        {
            var session = DataContext.Find<Session>(sessionId);

            if (session == null)
            {
                throw new CinematicException(Messages.SessionNotAvailableOrNotFound);
            }

            CheckDupedSession(timeAndDate, sessionId);

            session.TimeAndDate = timeAndDate;

            return session;
        }

        /// <inheritdoc />
        public Session RemoveSession(int sessionId)
        {
            var session = DataContext.Find<Session>(sessionId);

            if (session == null)
                throw new CinematicException(Messages.SessionNotAvailableOrNotFound);

            var hasTickets = DataContext.Tickets.AsQueryable().Include(t => t.Seat).Where(t => t.Seat.Session.Id == session.Id).Count() > 0;

            if (hasTickets)
            {
                throw new CinematicException(
                    string.Format(Messages.SessionCannotBeRemovedBecauseItHasSoldTickets, session.TimeAndDate.ToString("dd/MM/yyyy HH:mm")));
            }
            else
            {
                DataContext.Remove(session);
            }

            return session;
        }

        private void CheckDupedSession(DateTime timeAndDate, int? selfId = null)
        {
            var q = DataContext.Sessions.Where(s => s.TimeAndDate == timeAndDate);

            if (selfId.HasValue)
            {
                q = q.Where(s => s.Id != selfId.Value);
            }

            var dupedSession = q.Count();

            if (dupedSession > 0)
            {
                if (!selfId.HasValue)
                {
                    throw new CinematicException(Messages.SessionCannotBeCreatedBecauseIsDupe);
                }
                else
                {
                    throw new CinematicException(Messages.SessionCannotBeUpdatedBecauseDateIsDupe);
                }
            }
        }
    }
}

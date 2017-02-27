using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinematic
{
    /// <summary>
    /// DTO para recuperar la información de una página de la lista total de sesiones del sistema
    /// </summary>
    public class SessionsPageInfo
    {
        /// <summary>
        /// Inicializa una instancia de <see cref="SessionsPageInfo"/>
        /// </summary>
        /// <param name="pageCount">Número de páginas totales registradas</param>
        /// <param name="sessionsPage">Página de sesiones</param>
        public SessionsPageInfo(double pageCount, IEnumerable<Session> sessionsPage)
        {
            PageCount = pageCount;

            if (sessionsPage == null)
                SessionsPage = new List<Session>();
            else
                SessionsPage = sessionsPage;
        }

        /// <summary>
        /// Número de páginas totales registradas
        /// </summary>
        public double PageCount { get; private set; }

        /// <summary>
        /// Página de sesiones
        /// </summary>
        public IEnumerable<Session> SessionsPage { get; private set; }
    }
}

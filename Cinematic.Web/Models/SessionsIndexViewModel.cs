using System.Collections.Generic;

namespace Cinematic.Web.Models
{
    public class SessionsIndexViewModel
    {
        /// <summary>
        /// Lista de sesiones a renderizar
        /// </summary>
        public IEnumerable<Session> Sessions { get; set; }

        /// <summary>
        /// Página que estamos renderizando
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Número total de páginas
        /// </summary>
        public double PageCount { get; set; }

        /// <summary>
        /// Indica si hay página previa
        /// </summary>
        public bool HasPrevious { get; set; }

        /// <summary>
        /// Inidica si hay página siguiente
        /// </summary>
        public bool HasNext { get; set; }
    }
}

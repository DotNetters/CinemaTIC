using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Cinematic
{
    /// <summary>
    /// Excepción general de la capa de dominio / aplicación
    /// </summary>
    [Serializable]
    public class CinematicException : ApplicationException
    {
        /// <inheritdoc />
        public CinematicException() { }
        /// <inheritdoc />
        public CinematicException(string message) : base(message) { }
        /// <inheritdoc />
        public CinematicException(string message, Exception inner) : base(message, inner) { }
        /// <inheritdoc />
        protected CinematicException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }
    }
}

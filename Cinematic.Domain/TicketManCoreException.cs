using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TicketMan.Core
{
    /// <summary>
    /// Excepción general de la capa de dominio / aplicación
    /// </summary>
    [Serializable]
    public class TicketManCoreException : ApplicationException
    {
        /// <inheritdoc />
        public TicketManCoreException() { }
        /// <inheritdoc />
        public TicketManCoreException(string message) : base(message) { }
        /// <inheritdoc />
        public TicketManCoreException(string message, Exception inner) : base(message, inner) { }
        /// <inheritdoc />
        protected TicketManCoreException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }
    }
}

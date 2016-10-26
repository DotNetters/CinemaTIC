using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketMan.Core.Contracts
{
    /// <summary>
    /// Entidad de negocio del sistema
    /// </summary>
    public interface IBusinessEntity
    {
        /// <summary>
        /// Identificador interno de la entidad de negocio
        /// </summary>
        int Id { get; set; }
    }
}

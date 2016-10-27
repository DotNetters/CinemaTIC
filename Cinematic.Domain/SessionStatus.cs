using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Cinematic.Resources;

namespace Cinematic.Domain
{
    /// <summary>
    /// Estados en los que se puede encontrar una <see cref="Session">sesión</see>
    /// </summary>
    public enum SessionStatus
    {
        /// <summary>
        /// Los <see cref="Ticket">tickets</see> de la <see cref="Session">sesión</see> están a la venta
        /// </summary>
        [Display(ResourceType = typeof(Literals), Name = "ValueObject_SessionStatus_Open_DisplayName")]
        Open = 0,
        /// <summary>
        /// Ya no se pueden vender ni cancelar más <see cref="Ticket">tickets</see> de la <see cref="Session"/>
        /// </summary>
        [Display(ResourceType = typeof(Literals), Name = "ValueObject_SessionStatus_Closed_DisplayName")]
        Closed = 1,
        /// <summary>
        /// La <see cref="Session">sesión</see> ha sido cancelada. Sólo se pueden devolver <see cref="Ticket">tickets</see>
        /// </summary>
        [Display(ResourceType = typeof(Literals), Name = "ValueObject_SessionStatus_Cancelled_DisplayName")]
        Cancelled = 2
    }
}

using Cinematic.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Cinematic.Resources;

namespace Cinematic.Domain
{
    /// <summary>
    /// Representa una butaca en el sistema
    /// </summary>
    public class Seat : IBusinessEntity
    {
        /// <summary>
        /// Identificador interno de la butaca en el sistema
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Fila en la que está ubicada la butaca
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:00}")]
        [Display(ResourceType = typeof(Literals), Name = "Entity_Seat_Row_DisplayName")]
        public int Row { get; set; }

        /// <summary>
        /// Número de la butaca en la fila en la que está ubicada
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:00}")]
        [Display(ResourceType = typeof(Literals), Name = "Entity_Seat_SeatNumber_DisplayName")]
        public int SeatNumber { get; set; }

        /// <summary>
        /// <see cref="Session">Sesión</see> a la que pertenece la <see cref="Seat">butaca</see>
        /// </summary>
        public Session Session { get; set; }

        /// <summary>
        /// Indica si la butaca está reservada mediante la venta previa de un <see cref="Ticket"/>
        /// </summary>
        public bool Reserved { get; internal set; }
    }
}

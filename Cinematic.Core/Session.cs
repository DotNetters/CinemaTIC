using Cinematic.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Cinematic.Resources;

namespace Cinematic
{
    /// <summary>
    /// Representa una sesión para la que se venden <see cref="Ticket">tickets</see>  
    /// </summary>
    public class Session : IBusinessEntity
    {
        /// <summary>
        /// Número de filas de asientos que tiene la sala
        /// </summary>
        public const int NUMBER_OF_ROWS = 15;

        /// <summary>
        /// Número de asientos por fila que tiene la sala
        /// </summary>
        public const int NUMBER_OF_SEATS = 15;

        /// <summary>
        /// Identificador interno de la sesión
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Fecha y hora de la ejecución de la sesión
        /// </summary>
        [Display(ResourceType = typeof(Literals), Name = "Entity_Session_TimeAndDate_DisplayName")]
        public DateTime TimeAndDate { get; set; }

        /// <summary>
        /// Inicializa una instancia de <see cref="Session"/>
        /// </summary>
        public Session()
        {
            Status = SessionStatus.Open;
        }

        /// <summary>
        /// Indica el estado actual de la sesión
        /// </summary>
        [Display(ResourceType = typeof(Literals), Name = "Entity_Session_Status_DisplayName")]
        public SessionStatus Status { get; private set; }

        /// <summary>
        /// Cierra la sesión (Ya se está celebrando, no se pueden vender más tickets, ni devolverlos)
        /// </summary>
        public void Close()
        {
            if (Status != SessionStatus.Open)
                throw new CinematicException(Messages.CannotCloseSessionBecauseIsCancelledOrClosed);

            Status = SessionStatus.Closed;
        }

        /// <summary>
        /// Cancela la sesión (no se ha podido celebrar, se pueden devolver tickets, pero no vender más)
        /// </summary>
        public void Cancel()
        {
            if (Status != SessionStatus.Open)
                throw new CinematicException(Messages.CannotCancelSessionBecauseIsClosedOrCancelled);

            Status = SessionStatus.Cancelled;
        }

        /// <summary>
        /// Reabre una sesión cerrada o cancelada
        /// </summary>
        public void Reopen()
        {
            if (Status == SessionStatus.Open)
                throw new CinematicException(Messages.CannotReopenSessionBecauseIsOpen);

            Status = SessionStatus.Open;
        }
    }
}

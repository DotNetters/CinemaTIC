using System;
using System.Collections.Generic;

namespace Cinematic.Domain.Contracts
{
    /// <summary>
    /// Servicio que gestiona la disponibilidad de butacas y su reserva
    /// </summary>
    public interface ISeatManager
    {
        /// <summary>
        /// Adjudica (reserva) una <see cref="Seat">butaca</see>
        /// </summary>
        /// <param name="seat"><see cref="Seat">Butaca</see> a adjudicar (reservar)</param>
        /// <returns><see cref="Seat">Butaca</see> reservada</returns>
        Seat AllocateSeat(Seat seat);
        /// <summary>
        /// Cancela la adjudicación (reserva) de una <see cref="Seat">butaca</see>
        /// </summary>
        /// <param name="seat"><see cref="Seat">Butaca</see> a cancelar su adjudicación (reserva)</param>
        /// <returns><see cref="Seat">Butaca</see> disponible</returns>
        Seat DeallocateSeat(Seat seat);

        /// <summary>
        /// Obtiene la lista de <see cref="Seat">butacas</see> disponibles para una <see cref="Session">sesión</see> concreta
        /// </summary>
        /// <param name="session"><see cref="Session">Sesión</see> de la que obtener la lista de <see cref="Seat">butacas</see> disponibles</param>
        /// <returns></returns>
        IEnumerable<Seat> GetAvailableSeats(Session session);

        /// <summary>
        /// Obtiene una <see cref="Seat">butaca</see> del sistema
        /// </summary>
        /// <param name="session"><see cref="Session">Sesión</see> de la que se quiere obtener la <see cref="Seat">butaca</see></param>
        /// <param name="row">Fila en la que está ubicada la <see cref="Seat">butaca</see></param>
        /// <param name="seatNumber">Número de la <see cref="Seat">butaca</see> en la fila</param>
        /// <returns></returns>
        Seat GetSeat(Session session, int row, int seatNumber);
    }
}

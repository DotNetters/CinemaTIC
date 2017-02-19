using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinematic.Contracts
{
    /// <summary>
    /// Servicio que gestiona las <see cref="Session">sesiones</see>
    /// </summary>
    public interface ISessionManager
    {
        /// <summary>
        /// Obtiene las <see cref="Session">sesiones</see> disponibles para la venta de <see cref="Ticket">tickets</see>
        /// </summary>
        /// <returns>Lista de las <see cref="Session">sesiones</see> disponibles para la venta de <see cref="Ticket">tickets</see></returns>
        IEnumerable<Session> GetAvailableSessions();

        /// <summary>
        /// Crea una nueva <see cref="Session">sesión</see> disponible en el sistema
        /// </summary>
        /// <param name="timeAndDate">Fecha y hora en que tendrá lugar la <see cref="Session">sesión</see></param>
        /// <returns><see cref="Session">Sesión</see> recién creada en el sistema</returns>
        Session CreateSession(DateTime timeAndDate);

        /// <summary>
        /// Cierra la <see cref="Session">sesión</see> recibida como parámetro
        /// </summary>
        /// <param name="session"><see cref="Session">Sesión</see> a cerrar</param>
        /// <returns><see cref="Session">Sesión</see> cerrada</returns>
        Session CloseSession(Session session);

        /// <summary>
        /// Cancela la <see cref="Session">sesión</see> recibida como parámetro
        /// </summary>
        /// <param name="session"><see cref="Session">Sesión</see> a cancelar</param>
        /// <returns><see cref="Session">Sesión</see> cancelada</returns>
        Session CancelSession(Session session);

        /// <summary>
        /// Elimina una sesión del sistema 
        /// (si no tiene ventas de tickets asociadas, si no, lanza una excepción)
        /// </summary>
        /// <param name="session">Sesión a eliminar</param>
        /// <returns>Sesión eliminada o no</returns>
        Session RemoveSession(Session session);
    }
}

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
        /// Obtiene las <see cref="Session">sesiones</see> disponibles para la venta de <see cref="Ticket">entradas</see>
        /// </summary>
        /// <returns>Lista de las <see cref="Session">sesiones</see> disponibles para la venta de <see cref="Ticket">entradas</see></returns>
        IEnumerable<Session> GetAvailableSessions();

        /// <summary>
        /// Obtiene una sesión por su identificador
        /// </summary>
        /// <param name="id">Identificador de la sesión a obtener</param>
        /// <returns>Sesión obtenida o null en caso de no haberla encontrado</returns>
        Session Get(int id);

        /// <summary>
        /// Obtiene todas las sesiones registradas en el sistema
        /// </summary>
        /// <returns>Lista de sesiones completa</returns>
        IEnumerable<Session> GetAll();

        /// <summary>
        /// Obtiene una página del total de sesiones registradas en el sistema
        /// </summary>
        /// <param name="page">Cardinal de la página a obtener</param>
        /// <param name="sessionsPerPage">Número de sesiones a obtener por página</param>
        /// <returns></returns>
        SessionsPageInfo GetAll(int page, int sessionsPerPage);

        /// <summary>
        /// Crea una nueva <see cref="Session">sesión</see> disponible en el sistema
        /// </summary>
        /// <param name="timeAndDate">Fecha y hora en que tendrá lugar la <see cref="Session">sesión</see></param>
        /// <returns><see cref="Session">Sesión</see> recién creada en el sistema</returns>
        Session CreateSession(DateTime timeAndDate);

        /// <summary>
        /// Elimina una sesión del sistema 
        /// (si no tiene ventas de tickets asociadas, si no, lanza una excepción)
        /// </summary>
        /// <param name="sessionId">Idntificador de la sesión a eliminar</param>
        /// <returns>Sesión eliminada o no</returns>
        Session RemoveSession(int sessionId);

        /// <summary>
        /// Actualiza la fecha/hora de la sesión, siempre que no exista otra con la misma fecha/hora
        /// </summary>
        /// <param name="sessionId">Identificador de la sesión a actualizar</param>
        /// <param name="timeAndDate">Nueva fecha/hora a establecer</param>
        /// <returns></returns>
        Session UpdateSessionTimeAndDate(int sessionId, DateTime timeAndDate);
    }
}

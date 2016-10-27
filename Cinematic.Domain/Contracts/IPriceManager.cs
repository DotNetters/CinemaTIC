using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketMan.Core.Contracts
{
    /// <summary>
    /// Servicio que gestiona los precios de los <see cref="Ticket">tickets</see>
    /// </summary>
    public interface IPriceManager
    {
        /// <summary>
        /// Obtiene el precio de un <see cref="Ticket"/> en el momento de ser vendido
        /// </summary>
        /// <param name="session"><see cref="Session">Sesión</see> a la que pertenece el <see cref="Ticket">ticket</see> del que calcular su precio</param>
        /// <param name="row">Fila en la que está ubicada la <see cref="Seat">butaca</see> que reservará el <see cref="Ticket"/> del que calcular su precio</param>
        /// <param name="seatNumber">Número de la <see cref="Seat">butaca</see>, en su fila, que reservará el <see cref="Ticket"/> del que calcular su precio</param>
        /// <returns>Precio del <see cref="Ticket"/> según la sesión, la posición de la <see cref="Seat">butaca</see> y/o otras circunstancias</returns>
        double GetTicketPrice(Session session, int row, int seatNumber);
    }
}

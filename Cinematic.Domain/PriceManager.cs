using Cinematic.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinematic.Domain
{
    /// <summary>
    /// Servicio que gestiona los precios de los <see cref="Ticket">tickets</see> a la venta
    /// </summary>
    public class PriceManager : IPriceManager
    {
        /// <inheritdoc />
        public double GetTicketPrice(Session session, int row, int seatNumber)
        {
            return 5.00;
        }
    }
}

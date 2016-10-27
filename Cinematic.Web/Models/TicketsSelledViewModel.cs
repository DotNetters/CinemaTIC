using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TicketMan.Core;

namespace TicketMan.Web.Models
{
    public class TicketsSelledViewModel
    {
        /// <summary>
        /// Inicializa una instancia de <see cref="TicketsSelledViewModel"/>
        /// </summary>
        public TicketsSelledViewModel()
        {
            this.Errors = new List<string>();
            this.Tickets = new List<Ticket>();
        }

        /// <summary>
        /// Entradas emitidas
        /// </summary>
        public IList<Ticket> Tickets { get; set; }

        /// <summary>
        /// Errores que han impedido que la cventa se realice
        /// </summary>
        public IList<string> Errors { get; set; }
    }
}
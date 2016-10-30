using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinematic.Domain
{
    /// <summary>
    /// Permite acceder a utilidades de medición de tiempo en el sistema
    /// </summary>
    public static class SystemTime
    {
        /// <summary>
        /// Hora y fecha actual del sistema
        /// </summary>
        public static Func<DateTime> Now = () => DateTime.Now;
    }
}

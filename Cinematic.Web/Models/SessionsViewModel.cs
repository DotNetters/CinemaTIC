using Cinematic.Infrastructure;
using Cinematic.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cinematic.Web.Models
{
    public class SessionsViewModel
    {
        [Display(ResourceType = typeof(Literals), Name = "Entity_Session_TimeAndDate_DisplayName")]
        [Required]
        public DateTime TimeAndDate { get; set; } = SystemTime.Now();
    }
}

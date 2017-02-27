using Cinematic.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cinematic.Web.Models
{
    public class SessionsEditViewModel : SessionsViewModel
    {
        public int SessionId { get; set; }

        [Display(ResourceType = typeof(Literals), Name = "Entity_Session_Status_DisplayName")]
        public SessionStatus Status { get; set; }

        public SessionStatus? ToNextStatus1 { get; set; }
        public SessionStatus? ToNextStatus2 { get; set; }

        public string ToNextStatus1Literal
        {
            get
            {
                return ToStatusLiteral(ToNextStatus1);
            }
        }

        public string ToNextStatus2Literal
        {
            get
            {
                return ToStatusLiteral(ToNextStatus2);
            }
        }

        private string ToStatusLiteral(SessionStatus? toStatus)
        {
            var retVal = string.Empty;

            if (toStatus.HasValue)
            {
                switch (toStatus.Value)
                {
                    case SessionStatus.Open:
                        retVal = Literals.Open;
                        break;
                    case SessionStatus.Closed:
                        retVal = Literals.Close;
                        break;
                    case SessionStatus.Cancelled:
                        retVal = Literals.Cancel;
                        break;
                    default:
                        break;
                }
            }

            return retVal;
        }

        public SessionsEditViewModel(Session session)
        {
            SessionId = session.Id;
            TimeAndDate = session.TimeAndDate;
            Status = session.Status;
            ToNextStatus1 = null;
            ToNextStatus2 = null;

            switch (Status)
            {
                case SessionStatus.Open:
                    ToNextStatus1 = SessionStatus.Closed;
                    ToNextStatus2 = SessionStatus.Cancelled;
                    break;
                case SessionStatus.Closed:
                    ToNextStatus1 = SessionStatus.Open;
                    break;
                case SessionStatus.Cancelled:
                    ToNextStatus1 = SessionStatus.Open;
                    break;
                default:
                    break;
            }
        }

        public SessionsEditViewModel() { }
    }
}

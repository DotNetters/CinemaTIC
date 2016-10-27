using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cinematic.Core;
using Cinematic.Core.Contracts;
using Cinematic.Resources;
using Cinematic.Web.Models;

namespace Cinematic.Web.Controllers
{
    public class TicketSellingController : Controller
    {
        ISessionManager _sessionManager = null;
        ISeatManager _seatManager = null;
        ITicketManager _ticketManager = null;
        IDataContext _dataContext = null;

        public TicketSellingController(
            ISessionManager sessionManager,
            ISeatManager seatManager,
            ITicketManager ticketManager,
            IDataContext dataContext)
        {
            _sessionManager = sessionManager;
            _seatManager = seatManager;
            _ticketManager = ticketManager;
            _dataContext = dataContext;
        }

        // GET: TicketSelling
        public ActionResult Index(int? id)
        {
            var viewModel = new TicketSellingIndexViewModel();

            if (_sessionManager.GetAvailableSessions().Count() > 0)
            {
                viewModel.AvailableSessions = _sessionManager.GetAvailableSessions();
                if (id.HasValue)
                {
                    var selectedSession = _sessionManager.GetAvailableSessions().Where(s => s.Id == id).FirstOrDefault();
                    if (selectedSession != null)
                    {
                        viewModel.SelectedSession = selectedSession;
                    }
                    else
                    {
                        return HttpNotFound(Messages.SessionNotAvailableOrNotFound);
                    }
                }
                else
                {
                    viewModel.SelectedSession = viewModel.AvailableSessions.First();
                }
            }

            if (viewModel.SelectedSession != null)
            {
                viewModel.AvailableSeats = _seatManager.GetAvailableSeats(viewModel.SelectedSession);
            }

            return View(viewModel);
        }

        public ActionResult TicketsSelled(int? id, string selectedSeats)
        {
            var viewModel = new TicketsSelledViewModel();

            try
            {
                if (!string.IsNullOrWhiteSpace(selectedSeats))
                {
                    var session = _sessionManager.GetAvailableSessions().Where(s => s.Id == id).FirstOrDefault();

                    var seatLocators = GetSeatLocators(selectedSeats);

                    foreach (var seatLocator in seatLocators)
                    {
                        var seat = _seatManager.GetSeat(session, seatLocator.Item1, seatLocator.Item2);
                        viewModel.Tickets.Add(_ticketManager.SellTicket(seat));
                    }
                }
                else
                {
                    viewModel.Errors.Add(Messages.SeatsNotFound);
                }
            }
            catch (Exception ex)
            {
                viewModel.Errors.Add(ex.Message);
            }

            if (viewModel.Errors.Count() <= 0)
            {
                _dataContext.SaveChanges();
            }

            return View(viewModel);
        }

        private List<Tuple<int, int>> GetSeatLocators(string seats)
        {
            var retVal = new List<Tuple<int, int>>();

            var ss = seats.Split(',');

            foreach (var item in ss)
            {
                var seat = item.Split('_');
                retVal.Add(new Tuple<int, int>(int.Parse(seat[0]), int.Parse(seat[1])));
            }

            return retVal;
        }
    }
}
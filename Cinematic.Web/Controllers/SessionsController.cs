using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Cinematic.Web.Models;
using Cinematic.Contracts;

namespace Cinematic.Web.Controllers
{
    public class SessionsController : Controller
    {
        IDataContext DataContext { get; set; } = null;

        ISessionManager SessionManager { get; set; } = null;

        public SessionsController(IDataContext dataContext, ISessionManager sessionManager)
        {
            if (dataContext == null)
                throw new ArgumentNullException("dataContext");
            if (sessionManager == null)
                throw new ArgumentNullException("sessionManager");

            DataContext = dataContext;
            SessionManager = sessionManager;
        }

        // GET: Sessions
        public ActionResult Index(int? page)
        {
            if (!page.HasValue)
                page = 1;

            var viewModel = new SessionsIndexViewModel();
            var sessionsPageInfo = SessionManager.GetAll(page.Value, 10);

            viewModel.PageCount = sessionsPageInfo.PageCount;
            viewModel.HasPrevious = page.Value > 1 ? true : false;
            viewModel.HasNext = page.Value < viewModel.PageCount ? true : false;
            viewModel.Sessions = sessionsPageInfo.SessionsPage;
            viewModel.Page = page.Value;

            return View(viewModel);
        }

        // GET: Sessions/Details/5
        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest);
            }
            Session session = SessionManager.Get(id.Value);
            if (session == null)
            {
                return HttpNotFound();
            }
            return View(session);
        }

        // GET: Sessions/Create
        public ActionResult Create()
        {
            var viewModel = new SessionsViewModel();
            return View(viewModel);
        }

        // POST: Sessions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SessionsViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    SessionManager.CreateSession(viewModel.TimeAndDate);
                    DataContext.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (CinematicException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return View(viewModel);
                }
            }

            return View(viewModel);
        }

        // GET: Sessions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest);
            }
            Session session = SessionManager.Get(id.Value);
            if (session == null)
            {
                return HttpNotFound();
            }

            var viewModel = new SessionsEditViewModel(session);

            return View(viewModel);
        }

        // POST: Sessions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SessionsEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    SessionManager.UpdateSessionTimeAndDate(viewModel.SessionId, viewModel.TimeAndDate);
                    DataContext.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (CinematicException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return View(viewModel);
                }
            }
            return View(viewModel);
        }

        public ActionResult NextStatus(int? id, string targetStatus, SessionsEditViewModel viewModel)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest);
            }
            Session session = SessionManager.Get(id.Value);
            if (session == null)
            {
                return HttpNotFound();
            }
            else
            {
                try
                {
                    switch (targetStatus)
                    {
                        case "Open":
                            session.Reopen();
                            break;
                        case "Closed":
                            session.Close();
                            break;
                        case "Cancelled":
                            session.Cancel();
                            break;
                        default:
                            break;
                    }
                    DataContext.SaveChanges();
                    return RedirectToAction("Edit", new { id = id });
                }
                catch (CinematicException ex)
                {
                    ModelState.AddModelError(string.Empty, ex);
                    return View("Edit", viewModel);
                }
            }
        }

        // GET: Sessions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest);
            }
            Session session = SessionManager.Get(id.Value);
            if (session == null)
            {
                return HttpNotFound();
            }
            return View(session);
        }

        // POST: Sessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest);
            }
            Session session = SessionManager.Get(id.Value);
            if (session == null)
            {
                return HttpNotFound();
            }
            try
            {
                SessionManager.RemoveSession(id.Value);
                DataContext.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (CinematicException ex)
            {
                var viewModel = new SessionsDeleteConfirmedViewModel() { Session = session, Exception = ex };
                return View("DeleteConfirmed", viewModel);
            }
        }
    }
}

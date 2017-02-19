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
            var viewModel = new SessionsIndexViewModel();
            viewModel.PageCount = Math.Ceiling((double)DataContext.Sessions.Count() / 10);

            if (page.HasValue)
            {
                viewModel.HasPrevious = page.Value > 1 ? true : false;
                viewModel.HasNext = page.Value < viewModel.PageCount ? true : false;
                viewModel.Sessions = DataContext.Sessions.OrderByDescending(s => s.TimeAndDate).Skip((page.Value - 1) * 10).Take(10);
                viewModel.Page = page.Value;
            }
            else
            {
                viewModel.HasPrevious = false;
                viewModel.HasNext = viewModel.PageCount > 1 ? true : false;
                viewModel.Sessions = DataContext.Sessions.OrderByDescending(s => s.TimeAndDate).Skip(0).Take(10);
                viewModel.Page = 1;
            }

            return View(viewModel);

        }

        // GET: Sessions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Session session = DataContext.Find<Session>(id);
            if (session == null)
            {
                return HttpNotFound();
            }
            return View(session);
        }

        // GET: Sessions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Sessions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TimeAndDate,Status")] Session session)
        {
            if (ModelState.IsValid)
            {
                DataContext.Add(session);
                DataContext.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(session);
        }

        // GET: Sessions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Session session = DataContext.Find<Session>(id);
            if (session == null)
            {
                return HttpNotFound();
            }
            return View(session);
        }

        // POST: Sessions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TimeAndDate,Status")] Session session)
        {
            if (ModelState.IsValid)
            {
                DataContext.Update(session);
                DataContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(session);
        }

        // GET: Sessions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Session session = DataContext.Find<Session>(id);
            if (session == null)
            {
                return HttpNotFound();
            }
            return View(session);
        }

        // POST: Sessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Session session = DataContext.Find<Session>(id);
            if (session == null)
            {
                return HttpNotFound();
            }
            try
            {
                SessionManager.RemoveSession(session);
                DataContext.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (CinematicException ex)
            {
                var viewModel = new SessionsDeleteConfirmedViewModel() { Session = session, Exception = ex };
                return View("DeleteConfirmed", viewModel);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DataContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

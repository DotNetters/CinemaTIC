using NUnit.Framework;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Cinematic.Contracts;
using Cinematic.Resources;
using Cinematic.Infrastructure;

namespace Cinematic.Tests
{
    [TestFixture]
    [TestOf(typeof(SessionManager))]
    [Category("Cinematic.SessionManager")]
    public class SessionManagerTests
    {
        #region Initialization

        List<Session> Sessions { get; set; }

        [SetUp]
        public void PrepareTests()
        {
            Sessions = new List<Session>()
            {
                new Session() { Id=1, TimeAndDate=new DateTime(2016, 03, 21, 17, 0, 0) },
                new Session() { Id=2, TimeAndDate=new DateTime(2016, 03, 21, 19, 0, 0) },
                new Session() { Id=3, TimeAndDate=new DateTime(2016, 03, 21, 21, 0, 0) },

                new Session() { Id=4, TimeAndDate=new DateTime(2016, 03, 19, 17, 0, 0) },
                new Session() { Id=5, TimeAndDate=new DateTime(2016, 03, 19, 19, 0, 0) },
                new Session() { Id=6, TimeAndDate=new DateTime(2016, 03, 19, 21, 0, 0) },

                new Session() { Id=7, TimeAndDate=new DateTime(2016, 03, 20, 17, 0, 0) },
                new Session() { Id=8, TimeAndDate=new DateTime(2016, 03, 20, 19, 0, 0) },
                new Session() { Id=9, TimeAndDate=new DateTime(2016, 03, 20, 21, 0, 0) },
            };

            Sessions[3].Close();
            Sessions[4].Close();
            Sessions[5].Close();

            Sessions[6].Cancel();
            Sessions[7].Cancel();
            Sessions[8].Cancel();
        }

        public void AddNewSessions(int howMany, int pageSize, DateTime startDate, SessionStatus status = SessionStatus.Open)
        {
            var count = Sessions.Count;
            for (int i = count + 1; i <= count + howMany; i++)
            {
                var sss = new Session() { Id = i, TimeAndDate = startDate.AddDays(i - pageSize) };

                switch (status)
                {
                    case SessionStatus.Closed:
                        sss.Close();
                        break;
                    case SessionStatus.Cancelled:
                        sss.Cancel();
                        break;
                    default:
                        break;
                }

                Sessions.Add(sss);
            }
        }

        #endregion

        #region Ctor tests

        [Test]
        public void SessionManager_Ctor_Right()
        {
            //Arrange
            var dataContext = Mock.Of<IDataContext>();

            //Act
            var target = new SessionManager(dataContext);

            //Assert
            target.Should().NotBeNull();
        }

        [Test]
        public void SessionManager_Ctor_NullDataContextParam()
        {
            //Arrange
            //Act
            Action action = () => { var target = new SessionManager(null); };

            //Assert
            action.ShouldThrow<ArgumentNullException>(new ArgumentNullException("dataContext").Message);
        }

        #endregion

        #region GetAvailableSessions test

        [Test]
        public void SessionManager_GetAvailableSessions_Right()
        {
            //Arrange
            var dataContextMock = new Mock<IDataContext>();

            dataContextMock.Setup(m => m.Sessions).Returns(Sessions);

            var target = new SessionManager(dataContextMock.Object);

            //Act
            var result = target.GetAvailableSessions();

            //Assert
            result.Count().Should().Be(3);
            var resultArray = result.ToArray();

            resultArray[0].Id.Should().Be(1);
            resultArray[0].Status.Should().Be(SessionStatus.Open);
            resultArray[0].TimeAndDate.ShouldBeEquivalentTo(new DateTime(2016, 03, 21, 17, 0, 0));

            resultArray[1].Id.Should().Be(2);
            resultArray[1].Status.Should().Be(SessionStatus.Open);
            resultArray[1].TimeAndDate.ShouldBeEquivalentTo(new DateTime(2016, 03, 21, 19, 0, 0));

            resultArray[2].Id.Should().Be(3);
            resultArray[2].Status.Should().Be(SessionStatus.Open);
            resultArray[2].TimeAndDate.ShouldBeEquivalentTo(new DateTime(2016, 03, 21, 21, 0, 0));
        }

        [Test]
        public void SessionManager_GetAvailableSessions_Empty()
        {
            //Arrange
            var dataContextMock = new Mock<IDataContext>();

            dataContextMock.Setup(m => m.Sessions)
                .Returns(() =>
                {
                    var retVal = new List<Session>()
                    {
                        new Session() { Id = 1, TimeAndDate = new DateTime(2016, 03, 19, 17, 0, 0) }
                    };
                    retVal[0].Close();

                    return retVal;
                });

            var target = new SessionManager(dataContextMock.Object);

            //Act
            var result = target.GetAvailableSessions();

            //Assert
            result.Count().Should().Be(0);
        }

        #endregion

        #region CreateSession tests

        [Test]
        public void SessionManager_CreateSession_Right()
        {
            //Arrange
            var dataContextMock = new Mock<IDataContext>();

            dataContextMock.Setup(m => m.Sessions).Returns(Sessions);

            dataContextMock.Setup(m => m.Add(It.IsAny<Session>())).Callback<Session>((session) =>
            {
                Sessions.Add(session);
            });

            var target = new SessionManager(dataContextMock.Object);

            //Act
            var result = target.CreateSession(new DateTime(2016, 03, 22, 17, 0, 0));

            //Assert
            var avSessions = target.GetAvailableSessions();
            avSessions.Count().Should().Be(4);

            result.TimeAndDate.ShouldBeEquivalentTo(new DateTime(2016, 03, 22, 17, 0, 0));
            result.Status.Should().Be(SessionStatus.Open);
        }

        [Test]
        public void SessionManager_CreateSession_DateTimeParamMinValue()
        {
            //Arrange
            var dataContextMock = new Mock<IDataContext>();

            dataContextMock.Setup(m => m.Sessions).Returns(Sessions);

            dataContextMock.Setup(m => m.Add(It.IsAny<Session>())).Callback<Session>((session) =>
            {
                Sessions.Add(session);
            });

            var target = new SessionManager(dataContextMock.Object);

            //Act
            var result = target.CreateSession(DateTime.MinValue);

            //Assert
            var avSessions = target.GetAvailableSessions();
            avSessions.Count().Should().Be(4);

            result.TimeAndDate.ShouldBeEquivalentTo(DateTime.MinValue);
            result.Status.Should().Be(SessionStatus.Open);
        }

        [Test]
        public void SessionManager_CreateSession_DateTimeParamMaxValue()
        {
            //Arrange
            var dataContextMock = new Mock<IDataContext>();

            dataContextMock.Setup(m => m.Sessions).Returns(Sessions);

            dataContextMock.Setup(m => m.Add(It.IsAny<Session>())).Callback<Session>((session) =>
            {
                Sessions.Add(session);
            });

            var target = new SessionManager(dataContextMock.Object);

            //Act
            var result = target.CreateSession(DateTime.MaxValue);

            //Assert
            var avSessions = target.GetAvailableSessions();
            avSessions.Count().Should().Be(4);

            result.TimeAndDate.ShouldBeEquivalentTo(DateTime.MaxValue);
            result.Status.Should().Be(SessionStatus.Open);
        }

        [Test]
        public void SessionManager_CreateSession_Duped()
        {
            //Arrange
            var dataContextMock = new Mock<IDataContext>();

            dataContextMock.Setup(m => m.Sessions).Returns(Sessions);

            var target = new SessionManager(dataContextMock.Object);

            //Act
            Action action = () =>
            {
                target.CreateSession(Sessions[0].TimeAndDate);
            };

            //Assert
            action.ShouldThrow<CinematicException>().WithMessage(Messages.SessionCannotBeCreatedBecauseIsDupe);
        }

        #endregion

        #region RemoveSession tests

        [Test]
        [TestCase(SessionStatus.Open)]
        [TestCase(SessionStatus.Closed)]
        [TestCase(SessionStatus.Cancelled)]
        public void SessionManager_RemoveSession_Right(SessionStatus status)
        {
            //Arrange
            var sessionToDelete = Sessions.Where(s => s.Status == status).FirstOrDefault();
            var expectedSessions = Sessions.Where(s => s.Id != sessionToDelete.Id).ToArray();

            var dataContextMock = new Mock<IDataContext>();

            dataContextMock.Setup(m => m.Find<Session>(It.IsAny<int>()))
                .Returns<int>((sessionId) => {
                    return Sessions.Where(s => s.Id == sessionId).FirstOrDefault();
                });

            dataContextMock.Setup(m => m.Remove(It.IsAny<Session>()))
                .Callback<Session>((s) => { Sessions.Remove(s); });

            var target = new SessionManager(dataContextMock.Object);

            //Act
            target.RemoveSession(sessionToDelete.Id);

            //Assert
            Sessions.ShouldAllBeEquivalentTo(expectedSessions);
        }

        [Test]
        public void SessionManager_RemoveSession_NotFound()
        {
            //Arrange
            var dataContextMock = new Mock<IDataContext>();

            dataContextMock.Setup(m => m.Find<Session>(It.IsAny<int>()))
                .Returns<int>((sessionId) => {
                    return Sessions.Where(s => s.Id == sessionId).FirstOrDefault();
                });

            var target = new SessionManager(dataContextMock.Object);

            //Act
            Action action = () =>
            {
                target.RemoveSession(-1);
            };

            //Assert
            action.ShouldThrow<CinematicException>().WithMessage(Messages.SessionNotAvailableOrNotFound);
        }

        [Test]
        [TestCase(SessionStatus.Open)]
        [TestCase(SessionStatus.Closed)]
        [TestCase(SessionStatus.Cancelled)]
        public void SessionManager_RemoveSession_HasTickets(SessionStatus status)
        {
            //Arrange
            var sessionToDelete = Sessions.Where(s => s.Status == status).FirstOrDefault();
            var expectedSessions = Sessions.Where(s => s.Id != sessionToDelete.Id).ToArray();

            var dataContextMock = new Mock<IDataContext>();

            dataContextMock.Setup(m => m.Find<Session>(It.IsAny<int>()))
                .Returns<int>((sessionId) => {
                    return Sessions.Where(s => s.Id == sessionId).FirstOrDefault();
                });

            dataContextMock.Setup(m => m.Tickets).Returns(new Ticket[] { new Ticket() { Seat = new Seat() { Session = sessionToDelete } } });

            var target = new SessionManager(dataContextMock.Object);

            //Act
            Action action = () =>
            {
                target.RemoveSession(sessionToDelete.Id);
            };

            //Assert
            action.ShouldThrow<CinematicException>().WithMessage(string.Format(Messages.SessionCannotBeRemovedBecauseItHasSoldTickets, sessionToDelete.TimeAndDate.ToString("dd/MM/yyyy HH:mm")));
        }

        #endregion

        #region UpdateSessionTimeAndDate tests

        [Test]
        [TestCase(SessionStatus.Open)]
        [TestCase(SessionStatus.Closed)]
        [TestCase(SessionStatus.Cancelled)]
        public void SessionManager_UpdateSessionTimeAndDate_Right(SessionStatus status)
        {
            //Arrange
            var sessionToUpdate = Sessions.Where(s => s.Status == status).FirstOrDefault();

            var dataContextMock = new Mock<IDataContext>();

            dataContextMock.Setup(m => m.Find<Session>(It.IsAny<int>()))
                .Returns<int>((sessionId) => {
                    return Sessions.Where(s => s.Id == sessionId).FirstOrDefault();
                });

            var target = new SessionManager(dataContextMock.Object);

            var timeAndDate = new DateTime(2017, 2, 1, 18, 0, 0);

            //Act
            var result = target.UpdateSessionTimeAndDate(sessionToUpdate.Id, timeAndDate);

            //Assert
            result.Should().BeSameAs(sessionToUpdate);
            result.TimeAndDate.ShouldBeEquivalentTo(timeAndDate);
        }

        [Test]
        public void SessionManager_UpdateSessionTimeAndDate_NotFound()
        {
            //Arrange
            var dataContextMock = new Mock<IDataContext>();

            dataContextMock.Setup(m => m.Find<Session>(It.IsAny<int>()))
                .Returns((Session)null);

            var target = new SessionManager(dataContextMock.Object);

            //Act
            Action action = () =>
            {
                target.UpdateSessionTimeAndDate(-1, new DateTime(2017, 2, 1, 18, 0, 0));
            };

            //Assert
            action.ShouldThrow<CinematicException>()
                .WithMessage(Messages.SessionNotAvailableOrNotFound);
        }

        [Test]
        public void SessionManager_UpdateSessionTimeAndDate_Dupe()
        {
            //Arrange
            var dataContextMock = new Mock<IDataContext>();

            dataContextMock.Setup(m => m.Find<Session>(It.IsAny<int>()))
                .Returns<int>((sessionId) => {
                    var retVal = Sessions.Where(s => s.Id == sessionId).FirstOrDefault();
                    return retVal;
                });

            dataContextMock.Setup(m => m.Sessions).Returns(Sessions);

            var target = new SessionManager(dataContextMock.Object);

            //Act
            Action action = () =>
            {
                // La fecha ya existe en la colección de sesiones de la preparación (es la de la primera sesión abierta)
                // Como identificador de sesión le pasamos la de la segunda abierta
                target.UpdateSessionTimeAndDate(Sessions[1].Id, new DateTime(2016, 03, 21, 17, 0, 0));
            };

            //Assert
            action.ShouldThrow<CinematicException>()
                .WithMessage(Messages.SessionCannotBeUpdatedBecauseDateIsDupe);
        }

        #endregion

        #region Get tests

        [Test]
        [TestCase(SessionStatus.Open)]
        [TestCase(SessionStatus.Closed)]
        [TestCase(SessionStatus.Cancelled)]
        public void SessionManager_Get_Right(SessionStatus status)
        {
            //Arrange
            var expectedSession = Sessions.Where(s => s.Status == status).First();
            var dataContextMock = new Mock<IDataContext>();

            dataContextMock.Setup(m => m.Find<Session>(It.IsAny<int>()))
                .Returns<int>((id) =>
                {
                    return Sessions.Where(s => s.Id == id).FirstOrDefault();
                });

            var target = new SessionManager(dataContextMock.Object);

            //Act
            var result = target.Get(expectedSession.Id);

            //Assert
            result.Should().BeSameAs(expectedSession);
        }

        [Test]
        public void SessionManager_Get_NotFound()
        {
            //Arrange
            var dataContextMock = new Mock<IDataContext>();

            dataContextMock.Setup(m => m.Find<Session>(It.IsAny<int>()))
                .Returns<int>((id) =>
                {
                    return null;
                });

            var target = new SessionManager(dataContextMock.Object);

            //Act
            var result = target.Get(-1);

            //Assert
            result.Should().BeNull();
        }

        [Test]
        public void SessionManager_GetAll_Right()
        {
            //Arrange
            var dataContextMock = new Mock<IDataContext>();

            dataContextMock.Setup(m => m.Sessions).Returns(Sessions);

            var target = new SessionManager(dataContextMock.Object);

            //Act
            var result = target.GetAll();

            //Assert
            result.ShouldAllBeEquivalentTo(Sessions);
        }

        [Test]
        public void SessionManager_GetAll_Paged_FirstPage()
        {
            //Arrange
            AddSessionsForPageTests();

            var dataContextMock = new Mock<IDataContext>();

            dataContextMock.Setup(m => m.Sessions).Returns(Sessions);

            var target = new SessionManager(dataContextMock.Object);

            var expectedSessions = new Session[]
            {
                Sessions[0],
                Sessions[1],
                Sessions[2],
                Sessions[3],
                Sessions[4],
                Sessions[5],
                Sessions[6],
                Sessions[7],
                Sessions[8],
                Sessions[9]
            };

            //Act
            var result = target.GetAll(1, 10);

            //Assert
            result.PageCount.Should().Be(46);
            result.SessionsPage.ShouldAllBeEquivalentTo(expectedSessions);
        }

        [Test]
        public void SessionManager_GetAll_Paged_LastPage()
        {
            //Arrange
            AddSessionsForPageTests();

            var dataContextMock = new Mock<IDataContext>();

            dataContextMock.Setup(m => m.Sessions).Returns(Sessions);

            var target = new SessionManager(dataContextMock.Object);

            var expectedSessions = new Session[]
            {
                Sessions[450],
                Sessions[451],
                Sessions[452],
                Sessions[453],
                Sessions[454],
                Sessions[455],
                Sessions[456],
                Sessions[457],
                Sessions[458]
            };

            //Act
            var result = target.GetAll(46, 10);

            //Assert
            result.PageCount.Should().Be(46);
            result.SessionsPage.ShouldAllBeEquivalentTo(expectedSessions);
        }

        [Test]
        public void SessionManager_GetAll_Paged_Page()
        {
            //Arrange
            AddSessionsForPageTests();

            var dataContextMock = new Mock<IDataContext>();

            dataContextMock.Setup(m => m.Sessions).Returns(Sessions);

            var target = new SessionManager(dataContextMock.Object);

            var expectedSessions = new Session[]
            {
                Sessions[150],
                Sessions[151],
                Sessions[152],
                Sessions[153],
                Sessions[154],
                Sessions[155],
                Sessions[156],
                Sessions[157],
                Sessions[158],
                Sessions[159]
            };

            //Act
            var result = target.GetAll(16, 10);

            //Assert
            result.PageCount.Should().Be(46);
            result.SessionsPage.ShouldAllBeEquivalentTo(expectedSessions);
        }

        private void AddSessionsForPageTests()
        {
            AddNewSessions(150, 10, SystemTime.Now(), SessionStatus.Closed);
            AddNewSessions(150, 10, SystemTime.Now().AddDays(150), SessionStatus.Cancelled);
            AddNewSessions(150, 10, SystemTime.Now().AddDays(150));
        }

        #endregion
    }
}
using NUnit.Framework;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Cinematic.Contracts;

namespace Cinematic.Tests
{
    [TestFixture]
    [Category("Cinematic.SessionManager")]
    public class SessionManagerTests
    {
        #region Initialization

        List<Session> _sessions;

        [SetUp]
        public void PrepareTests()
        {
            _sessions = new List<Session>()
            {
                new Session() { Id=1, Status=SessionStatus.Open, TimeAndDate=new DateTime(2016, 03, 21, 17, 0, 0) },
                new Session() { Id=2, Status=SessionStatus.Open, TimeAndDate=new DateTime(2016, 03, 21, 19, 0, 0) },
                new Session() { Id=3, Status=SessionStatus.Open, TimeAndDate=new DateTime(2016, 03, 21, 21, 0, 0) },

                new Session() { Id=4, Status=SessionStatus.Closed, TimeAndDate=new DateTime(2016, 03, 19, 17, 0, 0) },
                new Session() { Id=5, Status=SessionStatus.Closed, TimeAndDate=new DateTime(2016, 03, 19, 19, 0, 0) },
                new Session() { Id=6, Status=SessionStatus.Closed, TimeAndDate=new DateTime(2016, 03, 19, 21, 0, 0) },

                new Session() { Id=7, Status=SessionStatus.Cancelled, TimeAndDate=new DateTime(2016, 03, 20, 17, 0, 0) },
                new Session() { Id=8, Status=SessionStatus.Cancelled, TimeAndDate=new DateTime(2016, 03, 20, 19, 0, 0) },
                new Session() { Id=9, Status=SessionStatus.Cancelled, TimeAndDate=new DateTime(2016, 03, 20, 21, 0, 0) },
            };
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

            dataContextMock.Setup(m => m.Sessions).Returns(_sessions);

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
                .Returns(
                    new List<Session>()
                    {
                        new Session() { Id = 1, Status = SessionStatus.Closed, TimeAndDate = new DateTime(2016, 03, 19, 17, 0, 0) }
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

            dataContextMock.Setup(m => m.Sessions).Returns(_sessions);

            dataContextMock.Setup(m => m.Add(It.IsAny<Session>())).Callback<Session>((session) =>
            {
                _sessions.Add(session);
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

            dataContextMock.Setup(m => m.Sessions).Returns(_sessions);

            dataContextMock.Setup(m => m.Add(It.IsAny<Session>())).Callback<Session>((session) =>
            {
                _sessions.Add(session);
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

            dataContextMock.Setup(m => m.Sessions).Returns(_sessions);

            dataContextMock.Setup(m => m.Add(It.IsAny<Session>())).Callback<Session>((session) =>
            {
                _sessions.Add(session);
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

        #endregion

        #region CloseSession tests

        [Test]
        public void SessionManager_CloseSession_Right()
        {
            //Arrange
            var dataContext = Mock.Of<IDataContext>();

            var target = new SessionManager(dataContext);

            var session = _sessions.Where(s => s.Status == SessionStatus.Open).FirstOrDefault();

            //Act
            var result = target.CloseSession(session);

            //Assert
            result.Id.Should().Be(session.Id);
            result.TimeAndDate.ShouldBeEquivalentTo(session.TimeAndDate);
            result.Status.Should().Be(SessionStatus.Closed);
            session.Status.Should().Be(SessionStatus.Closed);
        }

        [Test]
        public void SessionManager_CloseSession_NullSessionParam()
        {
            //Arrange
            var dataContext = Mock.Of<IDataContext>();
            var target = new SessionManager(dataContext);

            //Act
            Action action = () => { var result = target.CloseSession(null); };

            //Assert
            action.ShouldThrow<ArgumentNullException>().WithMessage(new ArgumentNullException("session").Message);
        }

        #endregion

        #region CancelSession tests

        [Test]
        public void SessionManager_CancelSession_Right()
        {
            //Arrange
            var dataContext = Mock.Of<IDataContext>();

            var target = new SessionManager(dataContext);

            var session = _sessions.Where(s => s.Status == SessionStatus.Open).FirstOrDefault();

            //Act
            var result = target.CancelSession(session);

            //Assert
            result.Id.Should().Be(session.Id);
            result.TimeAndDate.ShouldBeEquivalentTo(session.TimeAndDate);
            result.Status.Should().Be(SessionStatus.Cancelled);
            session.Status.Should().Be(SessionStatus.Cancelled);
        }

        [Test]
        public void SessionManager_CancelSession_NullSessionParam()
        {
            //Arrange
            var dataContext = Mock.Of<IDataContext>();
            var target = new SessionManager(dataContext);

            //Act
            Action action = () => { var result = target.CancelSession(null); };

            //Assert
            action.ShouldThrow<ArgumentNullException>().WithMessage(new ArgumentNullException("session").Message);
        }

        #endregion
    }
}

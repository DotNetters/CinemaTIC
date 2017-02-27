using Cinematic.Infrastructure;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using FluentAssertions;
using Cinematic.Resources;

namespace Cinematic.Core.Tests
{
    [TestFixture]
    [TestOf(typeof(Session))]
    [Category("Cinematic.Session")]
    public class SessionTests
    {
        public Session Session { get; set; }

        [OneTimeSetUp]
        public void PrepareTests()
        {
            var fixedDate = new DateTime(2017, 1, 1, 18, 0, 0);
            SystemTime.Now = () => fixedDate;
        }

        [SetUp]
        public void SetUpTest()
        {
            Session = new Session();
            Session.Id = 1;
            Session.TimeAndDate = SystemTime.Now();
        }

        #region CloseSession tests

        [Test]
        public void Session_Close_Right()
        {
            //Arrange

            //Act
            Session.Close();

            //Assert
            Session.Id.Should().Be(1);
            Session.TimeAndDate.ShouldBeEquivalentTo(SystemTime.Now());
            Session.Status.Should().Be(SessionStatus.Closed);
        }

        [Test]
        public void Session_Close_Cancelled()
        {
            //Arrange
            Session.Cancel();

            //Act
            Action action = () =>
            {
                Session.Close();
            };

            //Assert
            action.ShouldThrow<CinematicException>().WithMessage(Messages.CannotCloseSessionBecauseIsCancelledOrClosed);
        }

        [Test]
        public void Session_Close_Closed()
        {
            //Arrange
            Session.Close();

            //Act
            Action action = () =>
            {
                Session.Close();
            };

            //Assert
            action.ShouldThrow<CinematicException>().WithMessage(Messages.CannotCloseSessionBecauseIsCancelledOrClosed);
        }

        #endregion

        #region CancelSession tests

        [Test]
        public void Session_Cancel_Right()
        {
            //Arrange

            //Act
            Session.Cancel();

            //Assert
            Session.Id.Should().Be(1);
            Session.TimeAndDate.ShouldBeEquivalentTo(SystemTime.Now());
            Session.Status.Should().Be(SessionStatus.Cancelled);
        }

        [Test]
        public void Session_Cancel_Closed()
        {
            //Arrange
            Session.Close();

            //Act
            Action action = () =>
            {
                Session.Cancel();
            };

            //Assert
            action.ShouldThrow<CinematicException>().WithMessage(Messages.CannotCancelSessionBecauseIsClosedOrCancelled);
        }

        [Test]
        public void Session_Cancel_Cancelled()
        {
            //Arrange
            Session.Cancel();

            //Act
            Action action = () =>
            {
                Session.Cancel();
            };

            //Assert
            action.ShouldThrow<CinematicException>().WithMessage(Messages.CannotCancelSessionBecauseIsClosedOrCancelled);
        }

        #endregion

        #region ReopenSession tests

        [Test]
        public void Session_Reopen_Right()
        {
            //Arrange
            Session.Close();

            //Act
            Session.Reopen();

            //Assert
            Session.Id.Should().Be(1);
            Session.TimeAndDate.ShouldBeEquivalentTo(SystemTime.Now());
            Session.Status.Should().Be(SessionStatus.Open);
        }

        [Test]
        public void Session_Reopen_Open()
        {
            //Arrange

            //Act
            Action action = () =>
            {
                Session.Reopen();
            };

            //Assert
            action.ShouldThrow<CinematicException>().WithMessage(Messages.CannotReopenSessionBecauseIsOpen);
        }

        #endregion
    }
}

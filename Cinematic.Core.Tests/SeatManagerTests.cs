using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using NUnit.Framework;
using Moq;
using Cinematic.Contracts;
using FluentAssertions;
using Cinematic.Resources;

namespace Cinematic.Tests
{
    [TestFixture]
    [TestOf(typeof(SeatManager))]
    [Category("Cinematic.SeatManager")]
    public class SeatManagerTests
    {
        #region Initialization

        Session _openSession = null;
        Session _closedSession = null;
        Session _cancelledSession = null;

        IEnumerable<Seat> _seats = null;

        [SetUp]
        public void PrepareTests()
        {
            _closedSession = new Session()
            {
                Id = 1,
                // Lunes 27 de octubre de 2014, 16:00
                TimeAndDate = new DateTime(2014, 10, 27, 16, 00, 0)
            };
            _closedSession.Close();
            _openSession = new Session()
            {
                Id = 2,
                // Lunes 27 de octubre de 2014, 18:30
                TimeAndDate = new DateTime(2014, 10, 27, 18, 30, 0)
            };
            _cancelledSession = new Session()
            {
                Id = 3,
                // Lunes 27 de octubre de 2014, 21:00
                TimeAndDate = new DateTime(2014, 10, 27, 21, 00, 0)
            };
            _cancelledSession.Cancel();

            _seats = new List<Seat>()
            {
                new Seat() { Id = 1, Session = _closedSession, Row = 10, SeatNumber = 5, Reserved = true },
                new Seat() { Id = 2, Session = _closedSession, Row = 9, SeatNumber = 1, Reserved = true },
                new Seat() { Id = 3, Session = _closedSession, Row = 9, SeatNumber = 2, Reserved = true },
                new Seat() { Id = 4, Session = _closedSession, Row = 15, SeatNumber = 9, Reserved = true },

                new Seat() { Id = 5, Session = _cancelledSession, Row = 10, SeatNumber = 5, Reserved = true },
                new Seat() { Id = 6, Session = _cancelledSession, Row = 9, SeatNumber = 1, Reserved = true },
                new Seat() { Id = 7, Session = _cancelledSession, Row = 9, SeatNumber = 2, Reserved = true },
                new Seat() { Id = 8, Session = _cancelledSession, Row = 15, SeatNumber = 9, Reserved = true },

                new Seat() { Id = 9, Session = _openSession, Row = 10, SeatNumber = 5, Reserved = true },
                new Seat() { Id = 10, Session = _openSession, Row = 9, SeatNumber = 1, Reserved = true },
                new Seat() { Id = 11, Session = _openSession, Row = 9, SeatNumber = 2, Reserved = true },
                new Seat() { Id = 12, Session = _openSession, Row = 15, SeatNumber = 9, Reserved = true }
            };

        }

        #endregion

        #region Common Mocking

        private SeatManager GetSeatManagerWithSeats(out Mock<IDataContext> dataContextMock)
        {
            dataContextMock = new Mock<IDataContext>();
            dataContextMock.Setup(m => m.Seats).Returns(_seats);
            var target = new SeatManager(dataContextMock.Object);

            return target;
        }

        #endregion

        #region Ctor tests

        [Test]
        public void SeatManager_Ctor_Right()
        {
            //Arrange
            var dataContext = Mock.Of<IDataContext>();

            //Act
            var target = new SeatManager(dataContext);

            //Assert
            target.Should().NotBeNull();
        }

        [Test]
        public void SeatManager_Ctor_DataContextParamNull()
        {
            //Arrange
            //Act
            Action action = () => { var target = new SeatManager(null); };

            //Assert
            action.ShouldThrow<ArgumentNullException>().WithMessage(new ArgumentNullException("dataContext").Message);
        }

        #endregion

        #region GetSeat tests

        [Test]
        public void SeatManager_GetSeatRightReservedTest()
        {
            //Arrange
            Mock<IDataContext> dataContextMock;
            var target = GetSeatManagerWithSeats(out dataContextMock);

            //Act
            var result = target.GetSeat(_openSession, 9, 2);

            //Assert
            result.Session.ShouldBeEquivalentTo(_openSession);
            result.Row.Should().Be(9);
            result.SeatNumber.Should().Be(2);
            result.Reserved.Should().BeTrue();
        }

        [Test]
        public void SeatManager_GetSeatRightNotReservedTest()
        {
            //Arrange
            Mock<IDataContext> dataContextMock;
            var target = GetSeatManagerWithSeats(out dataContextMock);

            //Act
            var result = target.GetSeat(_openSession, 11, 12);

            //Assert
            result.Session.ShouldBeEquivalentTo(_openSession);
            result.Row.Should().Be(11);
            result.SeatNumber.Should().Be(12);
            result.Reserved.Should().BeFalse();
        }

        [Test]
        public void SeatManager_GetSeatRowAboveMaxTest()
        {
            //Arrange
            Mock<IDataContext> dataContextMock;
            var target = GetSeatManagerWithSeats(out dataContextMock);

            //Act
            Action action = () =>
            {
                target.GetSeat(_openSession, Session.NUMBER_OF_ROWS + 1, 1);
            };

            //Assert
            action.ShouldThrow<CinematicException>()
                .WithMessage(Messages.RowNumberIsAboveMaxAllowed);
        }

        [Test]
        public void SeatManager_GetSeatRowBelowMinTest()
        {
            //Arrange
            Mock<IDataContext> dataContextMock;
            var target = GetSeatManagerWithSeats(out dataContextMock);

            //Act
            Action action = () =>
            {
                target.GetSeat(_openSession, 0, 1);
            };

            //Assert
            action.ShouldThrow<CinematicException>()
                .WithMessage(Messages.RowNumberIsBelowMinAllowed);
        }

        [Test]
        public void SeatManager_GetSeatSeatNumberAboveMaxTest()
        {
            //Arrange
            Mock<IDataContext> dataContextMock;
            var target = GetSeatManagerWithSeats(out dataContextMock);

            //Act
            Action action = () =>
            {
                target.GetSeat(_openSession, 1, Session.NUMBER_OF_SEATS + 1);
            };

            //Assert
            action.ShouldThrow<CinematicException>()
                .WithMessage(Messages.SeatNumberIsAboveMaxAllowed);
        }

        [Test]
        public void SeatManager_GetSeatSeatNumberBelowMinTest()
        {
            //Arrange
            Mock<IDataContext> dataContextMock;
            var target = GetSeatManagerWithSeats(out dataContextMock);

            //Act
            Action action = () =>
            {
                target.GetSeat(_openSession, 1, 0);
            };

            //Assert
            action.ShouldThrow<CinematicException>()
                .WithMessage(Messages.SeatNumberIsBelowMinAllowed);
        }

        [Test]
        public void SeatManager_GetSeatSessionParamNullExceptionTest()
        {
            //Arrange
            var dataContext = Mock.Of<IDataContext>();
            var target = new SeatManager(dataContext);

            //Act
            Action action = () =>
            {
                target.GetSeat(null, 1, 1);
            };

            //Assert
            action.ShouldThrow<ArgumentNullException>()
                .WithMessage(new ArgumentNullException("session").Message);
        }

        #endregion

        #region GetAvailableSeats tests

        [Test]
        public void SeatManager_GetAvailableSeats_OpenSession_Right()
        {
            //Arrange
            Mock<IDataContext> dataContextMock;
            var target = GetSeatManagerWithSeats(out dataContextMock);

            //Act
            var result = target.GetAvailableSeats(_openSession);

            //Assert
            var openSessionSeats = _seats.Where(s => s.Session == _openSession);

            result.Where(s => s.Reserved == false).Count()
                .Should()
                .Be((Session.NUMBER_OF_ROWS * Session.NUMBER_OF_SEATS) - openSessionSeats.Count());


            result.Where(s => s.Reserved == true).Count().Should().Be(openSessionSeats.Count());

            result.Where(s => s.Reserved == true).Should().BeEquivalentTo(openSessionSeats);
        }

        [Test]
        public void SeatManager_GetAvailableSeats_ClosedSession_Right()
        {
            //Arrange
            Mock<IDataContext> dataContextMock;
            var target = GetSeatManagerWithSeats(out dataContextMock);

            //Act
            var result = target.GetAvailableSeats(_closedSession);

            //Assert
            result.Count().Should().Be(0);
        }

        [Test]
        public void SeatManager_GetAvailableSeats_CancelledSession_Right()
        {
            //Arrange
            Mock<IDataContext> dataContextMock;
            var target = GetSeatManagerWithSeats(out dataContextMock);

            //Act
            var result = target.GetAvailableSeats(_cancelledSession);

            //Assert
            result.Count().Should().Be(0);
        }

        [Test]
        public void SeatManager_GetAvailableSeats_SessionParamNullException()
        {
            //Arrange
            var dataContext = Mock.Of<IDataContext>();
            var target = new SeatManager(dataContext);

            //Act
            Action action = () =>
            {
                target.GetAvailableSeats(null);
            };

            //Assert
            action.ShouldThrow<ArgumentNullException>().WithMessage(new ArgumentNullException("session").Message);
        }

        #endregion

        #region AllocateSeat tests

        [Test]
        public void SeatManager_AllocateSeat_Right()
        {
            //Arrange
            Mock<IDataContext> dataContextMock;
            var target = GetSeatManagerWithSeats(out dataContextMock);

            dataContextMock.Setup(m => m.Add(It.IsAny<Seat>())).Callback<Seat>((theSeat) =>
            {
                (_seats as IList).Add(theSeat);
            });

            var seatsCount = _seats.Count();

            var seat = new Seat() { Session = _openSession, Row = 2, SeatNumber = 1, Reserved = false };

            //Act
            var result = target.AllocateSeat(seat);

            //Assert
            result.Row.Should().Be(2);
            result.SeatNumber.Should().Be(1);
            result.Reserved.Should().BeTrue();
            _seats.Count().Should().Be(seatsCount + 1);
        }

        [Test]
        public void SeatManager_AllocateSeat_ReservedSeat()
        {
            //Arrange
            Mock<IDataContext> dataContextMock;
            var target = GetSeatManagerWithSeats(out dataContextMock);

            var seatsCount = _seats.Count();

            var seat = new Seat() { Session = _openSession, Row = 9, SeatNumber = 1, Reserved = false };

            //Act
            Action action = () =>
            {
                var result = target.AllocateSeat(seat);
            };

            //Assert
            action.ShouldThrow<CinematicException>().WithMessage(Messages.SeatIsPreviouslyReserved);
        }

        [Test]
        public void SeatManager_AllocateSeat_SeatParamNullException()
        {
            //Arrange
            Mock<IDataContext> dataContextMock;
            var target = GetSeatManagerWithSeats(out dataContextMock);

            //Act
            Action action = () =>
            {
                target.AllocateSeat(null);
            };

            //Assert
            action.ShouldThrow<ArgumentNullException>().WithMessage(new ArgumentNullException("seat").Message);
        }

        #endregion

        #region DeAllocateSeat tests

        [Test]
        public void SeatManager_DeallocateSeat_Right()
        {
            //Arrange
            Mock<IDataContext> dataContextMock;
            var target = GetSeatManagerWithSeats(out dataContextMock);

            dataContextMock.Setup(m => m.Remove(It.IsAny<Seat>())).Callback<Seat>((theSeat) =>
            {
                (_seats as IList).Remove(theSeat);
            });

            var seatsCount = _seats.Count();

            var seat = _seats.Where(s => s.Id == 9).FirstOrDefault();

            //Act
            var result = target.DeallocateSeat(seat);

            //Assert
            result.Row.Should().Be(10);
            result.SeatNumber.Should().Be(5);
            result.Reserved.Should().BeFalse();
            _seats.Count().Should().Be(seatsCount - 1);
        }

        [Test]
        public void SeatManager_DeallocateSeat_NotReservedSeat()
        {
            //Arrange
            Mock<IDataContext> dataContextMock;
            var target = GetSeatManagerWithSeats(out dataContextMock);

            var seatsCount = _seats.Count();

            var seat = new Seat() { Session = _openSession, Row = 1, SeatNumber = 1, Reserved = false };

            //Act
            Action action = () =>
            {
                var result = target.DeallocateSeat(seat);
            };

            //Assert
            action.ShouldThrow<CinematicException>().WithMessage(Messages.SeatIsNotReserved);
        }

        [Test]
        public void SeatManager_DeallocateSeat_SeatParamNullException()
        {
            //Arrange
            Mock<IDataContext> dataContextMock;
            var target = GetSeatManagerWithSeats(out dataContextMock);

            //Act
            Action action = () =>
            {
                target.DeallocateSeat(null);
            };

            //Assert
            action.ShouldThrow<ArgumentNullException>().WithMessage(new ArgumentNullException("seat").Message);
        }

        #endregion
    }
}
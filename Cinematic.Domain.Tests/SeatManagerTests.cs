using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using NUnit.Framework;
using Moq;
using Cinematic.Domain.Contracts;
using FluentAssertions;
using Cinematic.Resources;

namespace Cinematic.Domain.Tests
{
    [TestFixture]
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
                Status = SessionStatus.Closed,
                // Lunes 27 de octubre de 2014, 16:00
                TimeAndDate = new DateTime(2014, 10, 27, 16, 00, 0)
            };
            _openSession = new Session()
            {
                Id = 2,
                Status = SessionStatus.Open,
                // Lunes 27 de octubre de 2014, 18:30
                TimeAndDate = new DateTime(2014, 10, 27, 18, 30, 0)
            };
            _cancelledSession = new Session()
            {
                Id = 3,
                Status = SessionStatus.Cancelled,
                // Lunes 27 de octubre de 2014, 21:00
                TimeAndDate = new DateTime(2014, 10, 27, 21, 00, 0)
            };

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

        private SeatManager GetSeatManagerWithSeats()
        {
            var dataContextMock = new Mock<IDataContext>();
            dataContextMock.Setup(m => m.Seats).Returns(_seats);
            var target = new SeatManager(dataContextMock.Object);

            return target;
        }

        #endregion

        #region GetSeat tests

        [Test]
        public void SeatManager_GetSeatRightReservedTest()
        {
            //Arrange
            var target = GetSeatManagerWithSeats();

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
            var target = GetSeatManagerWithSeats();

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
            var target = GetSeatManagerWithSeats();

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
            var target = GetSeatManagerWithSeats();

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
            var target = GetSeatManagerWithSeats();

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
            var target = GetSeatManagerWithSeats();

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

        #region GetAvailableSeats

        //[Test]
        //public void SeatManager_GetAvailableSeatsOpenSessionRightTest()
        //{
        //    //Arrange
        //    var target = GetSeatManagerWithSeats();

        //    //Act
        //    var result = target.GetAvailableSeats(_openSession);

        //    //Assert
        //    result.Count()
        //        .Should()
        //        .Be(
        //            (Session.NUMBER_OF_ROWS * Session.NUMBER_OF_SEATS) -
        //            (_seats.Where(s => s.Session == _openSession).Count()));

        //    //result.Should().BeEquivalentTo(_seats);
        //}

        //[Test]
        //public void SeatManager_GetAvailableSeatsClosedSessionRightTest()
        //{
        //    //Arrange
        //    var dataContext = new StubIDataContext()
        //    {
        //        SeatsGet = () => { return _seats; }
        //    };

        //    var target = new SeatManager(dataContext);

        //    //Act
        //    var result = target.GetAvailableSeats(_closedSession);

        //    //Assert
        //    Assert.AreEqual(0, (result as ICollection<Seat>).Count);
        //}

        //[Test]
        //public void SeatManager_GetAvailableSeatsCancelledSessionRightTest()
        //{
        //    //Arrange
        //    var dataContext = new StubIDataContext()
        //    {
        //        SeatsGet = () => { return _seats; }
        //    };

        //    var target = new SeatManager(dataContext);

        //    //Act
        //    var result = target.GetAvailableSeats(_cancelledSession);

        //    //Assert
        //    Assert.AreEqual(0, (result as ICollection<Seat>).Count);
        //}

        //[Test]
        //[ExpectedException(typeof(ArgumentNullException))]
        //public void SeatManager_GetAvailableSeatsSessionParamNullExceptionTest()
        //{
        //    //Arrange
        //    var dataContext = new StubIDataContext();
        //    var target = new SeatManager(dataContext);

        //    //Act
        //    var result = target.GetAvailableSeats(null);
        //}

        //#endregion

        //#region AllocateSeat

        //[Test]
        //public void SeatManager_AllocateSeatRightTest()
        //{
        //    //Arrange
        //    var dataContext = new StubIDataContext()
        //    {
        //        SeatsGet = () => { return _seats; },
        //    };
        //    dataContext.AddOf1M0<Seat>((theSeat) => { (_seats as IList).Add(theSeat); });

        //    var seatsCount = _seats.Count();

        //    var target = new SeatManager(dataContext);

        //    var seat = new Seat() { Session = _openSession, Row = 2, SeatNumber = 1, Reserved = false };

        //    //Act
        //    var result = target.AllocateSeat(seat);

        //    //Assert
        //    Assert.AreEqual(2, result.Row);
        //    Assert.AreEqual(1, result.SeatNumber);
        //    Assert.AreEqual(true, result.Reserved);
        //    Assert.AreEqual(seatsCount + 1, _seats.Count());
        //}

        //[Test]
        //[ExpectedException(typeof(TicketManCoreException))]
        //public void SeatManager_AllocateSeatReservedSeatTest()
        //{
        //    //Arrange
        //    var dataContext = new StubIDataContext()
        //    {
        //        SeatsGet = () => { return _seats; },
        //    };

        //    var seatsCount = _seats.Count();

        //    var target = new SeatManager(dataContext);

        //    var seat = new Seat() { Session = _openSession, Row = 9, SeatNumber = 1, Reserved = false };

        //    //Act
        //    var result = target.AllocateSeat(seat);
        //}

        //[Test]
        //[ExpectedException(typeof(ArgumentNullException))]
        //public void SeatManager_AllocateSeatSeatParamNullExceptionTest()
        //{
        //    //Arrange
        //    var dataContext = new StubIDataContext();
        //    var target = new SeatManager(dataContext);

        //    //Act
        //    target.AllocateSeat(null);
        //}

        #endregion
    }
}

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicketMan.Core.Contracts.Fakes;
using System.Collections.Generic;
using System.Collections;

namespace TicketMan.Core.Tests
{
    [TestClass]
    public class SeatManagerTests
    {
        #region Initialization

        Session _openSession = null;
        Session _closedSession = null;
        Session _cancelledSession = null;

        IEnumerable<Seat> _seats = null;

        [TestInitialize]
        public void PrepareTests()
        {
            _closedSession = new Session()
            {
                Status = SessionStatus.Closed,
                // Lunes 27 de octubre de 2014, 16:00
                TimeAndDate = new DateTime(2014, 10, 27, 16, 00, 0)
            };
            _openSession = new Session()
            {
                Status = SessionStatus.Open,
                // Lunes 27 de octubre de 2014, 18:30
                TimeAndDate = new DateTime(2014, 10, 27, 18, 30, 0)
            };
            _cancelledSession = new Session()
            {
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

        #region GetSeat tests

        [TestMethod]
        public void SeatManager_GetSeatRightReservedTest()
        {
            //Arrange
            var dataContext = new StubIDataContext()
            {
                SeatsGet = () => { return _seats; }
            };

            var target = new SeatManager(dataContext);

            //Act
            Seat result = target.GetSeat(_openSession, 9, 2);

            //Assert
            Assert.AreEqual(_openSession, result.Session);
            Assert.AreEqual(9, result.Row);
            Assert.AreEqual(2, result.SeatNumber);
            Assert.IsTrue(result.Reserved);
        }

        [TestMethod]
        public void SeatManager_GetSeatRightNotReservedTest()
        {
            //Arrange
            var dataContext = new StubIDataContext()
            {
                SeatsGet = () => { return _seats; }
            };

            var target = new SeatManager(dataContext);

            //Act
            Seat result = target.GetSeat(_openSession, 11, 12);

            //Assert
            Assert.AreEqual(_openSession, result.Session);
            Assert.AreEqual(11, result.Row);
            Assert.AreEqual(12, result.SeatNumber);
            Assert.IsFalse(result.Reserved);
        }

        [TestMethod]
        [ExpectedException(typeof(TicketManCoreException))]
        public void SeatManager_GetSeatRowAboveMaxTest()
        {
            //Arrange
            var dataContext = new StubIDataContext()
            {
                SeatsGet = () => { return _seats; }
            };

            var target = new SeatManager(dataContext);

            //Act
            Seat result = target.GetSeat(_openSession, Session.NUMBER_OF_ROWS + 1, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(TicketManCoreException))]
        public void SeatManager_GetSeatRowBelowMinTest()
        {
            //Arrange
            var dataContext = new StubIDataContext()
            {
                SeatsGet = () => { return _seats; }
            };

            var target = new SeatManager(dataContext);

            //Act
            Seat result = target.GetSeat(_openSession, 0, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(TicketManCoreException))]
        public void SeatManager_GetSeatSeatNumberAboveMaxTest()
        {
            //Arrange
            var dataContext = new StubIDataContext()
            {
                SeatsGet = () => { return _seats; }
            };

            var target = new SeatManager(dataContext);

            //Act
            Seat result = target.GetSeat(_openSession, 1, Session.NUMBER_OF_SEATS + 1);
        }

        [TestMethod]
        [ExpectedException(typeof(TicketManCoreException))]
        public void SeatManager_GetSeatSeatNumberBelowMinTest()
        {
            //Arrange
            var dataContext = new StubIDataContext()
            {
                SeatsGet = () => { return _seats; }
            };

            var target = new SeatManager(dataContext);

            //Act
            Seat result = target.GetSeat(_openSession, 1, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SeatManager_GetSeatSessionParamNullExceptionTest()
        {
            //Arrange
            var dataContext = new StubIDataContext();
            var target = new SeatManager(dataContext);

            //Act
            target.GetSeat(null, 1, 1);
        } 

        #endregion

        #region GetAvailableSeats

        [TestMethod]
        public void SeatManager_GetAvailableSeatsOpenSessionRightTest()
        {
            //Arrange
            var dataContext = new StubIDataContext()
            {
                SeatsGet = () => { return _seats; }
            };

            var target = new SeatManager(dataContext);

            //Act
            var result = target.GetAvailableSeats(_openSession);

            //Assert
            Assert.AreEqual(
                (Session.NUMBER_OF_ROWS * Session.NUMBER_OF_SEATS) - (_seats.Where(s => s.Session == _openSession).Count()),
                (result as ICollection<Seat>).Count);
            foreach (var seat in result)
            {
                Assert.IsFalse((_seats as ICollection<Seat>).Contains(seat));
            }
        }

        [TestMethod]
        public void SeatManager_GetAvailableSeatsClosedSessionRightTest()
        {
            //Arrange
            var dataContext = new StubIDataContext()
            {
                SeatsGet = () => { return _seats; }
            };

            var target = new SeatManager(dataContext);

            //Act
            var result = target.GetAvailableSeats(_closedSession);

            //Assert
            Assert.AreEqual(0, (result as ICollection<Seat>).Count);
        }

        [TestMethod]
        public void SeatManager_GetAvailableSeatsCancelledSessionRightTest()
        {
            //Arrange
            var dataContext = new StubIDataContext()
            {
                SeatsGet = () => { return _seats; }
            };

            var target = new SeatManager(dataContext);

            //Act
            var result = target.GetAvailableSeats(_cancelledSession);

            //Assert
            Assert.AreEqual(0, (result as ICollection<Seat>).Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SeatManager_GetAvailableSeatsSessionParamNullExceptionTest()
        {
            //Arrange
            var dataContext = new StubIDataContext();
            var target = new SeatManager(dataContext);

            //Act
            var result = target.GetAvailableSeats(null);
        } 

        #endregion

        #region AllocateSeat

        [TestMethod]
        public void SeatManager_AllocateSeatRightTest()
        {
            //Arrange
            var dataContext = new StubIDataContext()
            {
                SeatsGet = () => { return _seats; },
            };
            dataContext.AddOf1M0<Seat>((theSeat) => { (_seats as IList).Add(theSeat); });

            var seatsCount = _seats.Count();

            var target = new SeatManager(dataContext);

            var seat = new Seat() { Session = _openSession, Row = 2, SeatNumber = 1, Reserved = false };

            //Act
            var result = target.AllocateSeat(seat);

            //Assert
            Assert.AreEqual(2, result.Row);
            Assert.AreEqual(1, result.SeatNumber);
            Assert.AreEqual(true, result.Reserved);
            Assert.AreEqual(seatsCount + 1, _seats.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(TicketManCoreException))]
        public void SeatManager_AllocateSeatReservedSeatTest()
        {
            //Arrange
            var dataContext = new StubIDataContext()
            {
                SeatsGet = () => { return _seats; },
            };

            var seatsCount = _seats.Count();

            var target = new SeatManager(dataContext);

            var seat = new Seat() { Session = _openSession, Row = 9, SeatNumber = 1, Reserved = false };

            //Act
            var result = target.AllocateSeat(seat);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SeatManager_AllocateSeatSeatParamNullExceptionTest()
        {
            //Arrange
            var dataContext = new StubIDataContext();
            var target = new SeatManager(dataContext);

            //Act
            target.AllocateSeat(null);
        }

        #endregion
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicketMan.Core.Contracts.Fakes;
using System.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using TicketMan.Core.Contracts;

namespace TicketMan.Core.Tests
{
    [TestClass]
    public class TicketManagerTests
    {
        #region SellTicket tests

        [TestMethod]
        public void TicketManager_SellTicketRightTest()
        {
            using (ShimsContext.Create())
            {
                //Arrange
                var seatManager = new StubISeatManager()
                {
                    AllocateSeatSeat = (s) =>
                    {
                        s.Reserved = true;
                        return s;
                    }
                };
                var priceManager = new StubIPriceManager()
                {
                    GetTicketPriceSessionInt32Int32 = (s, row, seatNumber) =>
                    {
                        return 7;
                    }
                };
                var addCount = 0;
                var dataContext = new StubIDataContext();
                dataContext.AddOf1M0<Ticket>((t) =>
                {
                    t.Id = 1;
                    addCount++;
                });

                var dateFixed = new DateTime(2014, 10, 26, 18, 0, 0);
                ShimDateTime.NowGet = () => { return dateFixed; };

                var session = new Session() { TimeAndDate = new DateTime(2014, 10, 26), Status = SessionStatus.Open };
                var seat = new Seat() { Row = Session.NUMBER_OF_ROWS, SeatNumber = Session.NUMBER_OF_SEATS, Session = session, Reserved = false };
                var target = new TicketManager(seatManager, priceManager, dataContext);

                //Act
                var result = target.SellTicket(seat);

                //Assert
                Assert.AreEqual(seat, result.Seat);
                Assert.AreEqual(7, result.Price);
                Assert.AreEqual(dateFixed, result.TimeAndDate);
                Assert.AreEqual(1, addCount);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(TicketManCoreException))]
        public void TicketManager_SellTicketClosedSessionTest()
        {
            //Arrange
            var seatManager = new StubISeatManager();
            var priceManager = new StubIPriceManager();
            var dataContext = new StubIDataContext();

            var session = new Session() { TimeAndDate = new DateTime(2014, 10, 26), Status = SessionStatus.Closed };
            var seat = new Seat() { Row = Session.NUMBER_OF_ROWS, SeatNumber = Session.NUMBER_OF_SEATS, Session = session, Reserved = false };
            var target = new TicketManager(seatManager, priceManager, dataContext);

            //Act
            var result = target.SellTicket(seat);
        }

        [TestMethod]
        [ExpectedException(typeof(TicketManCoreException))]
        public void TicketManager_SellTicketCancelledSessionTest()
        {
            //Arrange
            var seatManager = new StubISeatManager();
            var priceManager = new StubIPriceManager();
            var dataContext = new StubIDataContext();

            var session = new Session() { TimeAndDate = new DateTime(2014, 10, 26), Status = SessionStatus.Cancelled };
            var seat = new Seat() { Row = Session.NUMBER_OF_ROWS, SeatNumber = Session.NUMBER_OF_SEATS, Session = session, Reserved = false };
            var target = new TicketManager(seatManager, priceManager, dataContext);

            //Act
            var result = target.SellTicket(seat);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TicketManager_SellTicketSeatParamNullExceptionTest()
        {
            //Arrange
            var seatManager = new StubISeatManager();
            var priceManager = new StubIPriceManager();
            var dataContext = new StubIDataContext();

            var target = new TicketManager(seatManager, priceManager, dataContext);

            //Act
            target.SellTicket(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TicketManager_SellTicketSeatSessionParamNullExceptionTest()
        {
            //Arrange
            var seatManager = new StubISeatManager();
            var priceManager = new StubIPriceManager();
            var dataContext = new StubIDataContext();

            var target = new TicketManager(seatManager, priceManager, dataContext);

            //Act
            target.SellTicket(new Seat() { Session = null });
        } 

        #endregion

        #region CancelTicket tests

        [TestMethod]
        public void TicketManager_CancelTicketRightTest()
        {
            using (ShimsContext.Create())
            {
                //Arrange
                var seatManager = new StubISeatManager()
                {
                    DeAllocateSeatSeat = (s) =>
                    {
                        s.Reserved = false;
                        return s;
                    }
                };
                var priceManager = new StubIPriceManager();

                var removeCount = 0;
                var dataContext = new StubIDataContext();
                dataContext.RemoveOf1M0<Ticket>((t) =>
                {
                    t.Id = 1;
                    removeCount++;
                });

                var session = new Session() { TimeAndDate = new DateTime(2014, 10, 26), Status = SessionStatus.Open };
                var seat = new Seat() { Row = Session.NUMBER_OF_ROWS, SeatNumber = Session.NUMBER_OF_SEATS, Session = session, Reserved = true };
                var ticket = new Ticket() { Price = 7, Seat = seat, TimeAndDate = DateTime.Now };

                var target = new TicketManager(seatManager, priceManager, dataContext);

                //Act
                target.CancelTicket(ticket);

                //Assert
                Assert.IsFalse(ticket.Seat.Reserved);
                Assert.AreEqual(1, removeCount);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(TicketManCoreException))]
        public void TicketManager_CancelTicketClosedSessionTest()
        {
            //Arrange
            var seatManager = new StubISeatManager();
            var priceManager = new StubIPriceManager();
            var dataContext = new StubIDataContext();

            var session = new Session() { TimeAndDate = new DateTime(2014, 10, 26), Status = SessionStatus.Closed };
            var seat = new Seat() { Row = Session.NUMBER_OF_ROWS, SeatNumber = Session.NUMBER_OF_SEATS, Session = session, Reserved = true };
            var ticket = new Ticket() { Price = 7, Seat = seat, TimeAndDate = DateTime.Now };

            var target = new TicketManager(seatManager, priceManager, dataContext);

            //Act
            target.CancelTicket(ticket);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TicketManager_CancelTicketTicketParamNullExceptionTest()
        {
            //Arrange
            var seatManager = new StubISeatManager();
            var priceManager = new StubIPriceManager();
            var dataContext = new StubIDataContext();

            var target = new TicketManager(seatManager, priceManager, dataContext);

            //Act
            target.CancelTicket(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TicketManager_CancelTicketTicketSeatParamNullExceptionTest()
        {
            //Arrange
            var seatManager = new StubISeatManager();
            var priceManager = new StubIPriceManager();
            var dataContext = new StubIDataContext();

            var target = new TicketManager(seatManager, priceManager, dataContext);

            //Act
            target.CancelTicket(new Ticket() { Seat = null });
        } 

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TicketManager_CancelTicketTicketSeatSessionParamNullExceptionTest()
        {
            //Arrange
            var seatManager = new StubISeatManager();
            var priceManager = new StubIPriceManager();
            var dataContext = new StubIDataContext();

            var target = new TicketManager(seatManager, priceManager, dataContext);

            //Act
            target.CancelTicket(new Ticket() { Seat = new Seat() { Session = null } });
        } 

        #endregion

        #region Constructor tests

        [TestMethod]
        public void TicketManager_ConstructorRightTest()
        {
            //Arrange
            var seatManager = new StubISeatManager();
            var priceManager = new StubIPriceManager();
            var dataContext = new StubIDataContext();

            //Act
            var target = new TicketManager(seatManager, priceManager, dataContext);

            //Assert
            PrivateObject po = new PrivateObject(target);

            Assert.IsInstanceOfType(po.GetField("_seatManager"), typeof(ISeatManager));
            Assert.IsNotNull(po.GetField("_seatManager"));
            Assert.IsInstanceOfType(po.GetField("_priceManager"), typeof(IPriceManager));
            Assert.IsNotNull(po.GetField("_priceManager"));
            Assert.IsInstanceOfType(po.GetField("_dataContext"), typeof(IDataContext));
            Assert.IsNotNull(po.GetField("_dataContext"));

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TicketManager_ConstructorSeatManagerParamNullExceptionTest()
        {
            //Arrange
            var seatManager = (ISeatManager)null;
            var priceManager = new StubIPriceManager();
            var dataContext = new StubIDataContext();

            //Act
            var target = new TicketManager(seatManager, priceManager, dataContext);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TicketManager_ConstructorPriceManagerParamNullExceptionTest()
        {
            //Arrange
            var seatManager = new StubISeatManager();
            var priceManager = (IPriceManager)null;
            var dataContext = new StubIDataContext();

            //Act
            var target = new TicketManager(seatManager, priceManager, dataContext);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TicketManager_ConstructorDataContextParamNullExceptionTest()
        {
            //Arrange
            var seatManager = new StubISeatManager();
            var priceManager = new StubIPriceManager();
            var dataContext = (IDataContext)null;

            //Act
            var target = new TicketManager(seatManager, priceManager, dataContext);
        } 

        #endregion
    }
}

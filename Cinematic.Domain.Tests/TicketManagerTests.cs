using NUnit.Framework;
using FluentAssertions;
using System;
using Cinematic.Domain.Contracts;
using Moq;
using Cinematic.Resources;

namespace Cinematic.Domain.Tests
{
    [TestFixture]
    [Category("Cinematic.Domain.TicketManager")]
    public class TicketManagerTests
    {
        #region Common mocking

        #endregion

        #region SellTicket tests

        [Test]
        public void TicketManager_SellTicket_Right()
        {
            //Arrange
            var seatManagerMock = new Mock<ISeatManager>();
            var priceManagerMock = new Mock<IPriceManager>();
            var dataContextMock = new Mock<IDataContext>();

            seatManagerMock.Setup(m => m.AllocateSeat(It.IsAny<Seat>())).Returns<Seat>((s) =>
            {
                s.Reserved = true;
                return s;
            });

            priceManagerMock.Setup(m => m.GetTicketPrice(It.IsAny<Session>(), It.IsAny<int>(), It.IsAny<int>())).Returns(7);

            dataContextMock.Setup(m => m.Add(It.IsAny<Ticket>())).Callback<Ticket>((t) =>
            {
                t.Id = 1;
            });

            var fixedDate = new DateTime(2014, 10, 26, 18, 0, 0); ;

            SystemTime.Now = () => fixedDate;

            var session = new Session() { TimeAndDate = new DateTime(2014, 10, 26), Status = SessionStatus.Open };
            var seat = new Seat() { Row = Session.NUMBER_OF_ROWS, SeatNumber = Session.NUMBER_OF_SEATS, Session = session, Reserved = false };
            var target = new TicketManager(seatManagerMock.Object, priceManagerMock.Object, dataContextMock.Object);

            //Act
            var result = target.SellTicket(seat);

            //Assert
            result.Seat.ShouldBeEquivalentTo(seat);
            result.Price.Should().Be(7);
            result.TimeAndDate.ShouldBeEquivalentTo(fixedDate);
        }

        [Test]
        public void TicketManager_SellTicket_ClosedSession()
        {
            //Arrange
            var seatManager = Mock.Of<ISeatManager>();
            var priceManager = Mock.Of<IPriceManager>();
            var dataContext = Mock.Of<IDataContext>();

            var session = new Session() { TimeAndDate = new DateTime(2014, 10, 26), Status = SessionStatus.Closed };
            var seat = new Seat() { Row = Session.NUMBER_OF_ROWS, SeatNumber = Session.NUMBER_OF_SEATS, Session = session, Reserved = false };
            var target = new TicketManager(seatManager, priceManager, dataContext);

            //Act
            Action action = () => { var result = target.SellTicket(seat); };

            //Assert
            action.ShouldThrow<CinematicException>().WithMessage(Messages.SessionIsClosedNoTicketsAvailable);
        }

        [Test]
        public void TicketManager_SellTicket_CancelledSession()
        {
            //Arrange
            var seatManager = Mock.Of<ISeatManager>();
            var priceManager = Mock.Of<IPriceManager>();
            var dataContext = Mock.Of<IDataContext>();

            var session = new Session() { TimeAndDate = new DateTime(2014, 10, 26), Status = SessionStatus.Cancelled };
            var seat = new Seat() { Row = Session.NUMBER_OF_ROWS, SeatNumber = Session.NUMBER_OF_SEATS, Session = session, Reserved = false };
            var target = new TicketManager(seatManager, priceManager, dataContext);

            //Act
            Action action = () => { var result = target.SellTicket(seat); };

            //Assert
            action.ShouldThrow<CinematicException>().WithMessage(Messages.SessionIsCancelledNoTicketsAvailable);
        }

        [Test]
        public void TicketManager_SellTicket_SeatParamNullException()
        {
            //Arrange
            var seatManager = Mock.Of<ISeatManager>();
            var priceManager = Mock.Of<IPriceManager>();
            var dataContext = Mock.Of<IDataContext>();

            var target = new TicketManager(seatManager, priceManager, dataContext);

            //Act
            Action action = () => { target.SellTicket(null); };

            //Assert
            action.ShouldThrow<ArgumentNullException>().WithMessage(new ArgumentNullException("seat").Message);
        }

        [Test]
        public void TicketManager_SellTicket_SeatSessionParamNullException()
        {
            //Arrange
            var seatManager = Mock.Of<ISeatManager>();
            var priceManager = Mock.Of<IPriceManager>();
            var dataContext = Mock.Of<IDataContext>();

            var target = new TicketManager(seatManager, priceManager, dataContext);

            //Act
            Action action = () => { target.SellTicket(new Seat() { Session = null }); };

            //Assert
            action.ShouldThrow<ArgumentNullException>().WithMessage(new ArgumentNullException("seat.Session").Message);
        }

        #endregion

        #region CancelTicket tests

        [Test]
        public void TicketManager_CancelTicket_Right()
        {
            //Arrange
            var seatManagerMock = new Mock<ISeatManager>();
            var priceManagerMock = new Mock<IPriceManager>();
            var dataContextMock = new Mock<IDataContext>();

            seatManagerMock.Setup(m => m.DeallocateSeat(It.IsAny<Seat>())).Returns<Seat>((s) =>
            {
                s.Reserved = false;
                return s;
            });

            dataContextMock.Setup(m => m.Remove(It.IsAny<Ticket>())).Callback<Ticket>((t) =>
            {
                t.Id = 1;
            });

            var session = new Session() { TimeAndDate = new DateTime(2014, 10, 26), Status = SessionStatus.Open };
            var seat = new Seat() { Row = Session.NUMBER_OF_ROWS, SeatNumber = Session.NUMBER_OF_SEATS, Session = session, Reserved = true };
            var ticket = new Ticket() { Price = 7, Seat = seat, TimeAndDate = DateTime.Now };

            var target = new TicketManager(seatManagerMock.Object, priceManagerMock.Object, dataContextMock.Object);

            //Act
            target.CancelTicket(ticket);

            //Assert
            ticket.Seat.Reserved.Should().BeFalse();
            dataContextMock.Verify(m => m.Remove(It.IsAny<Ticket>()), Times.Once);
        }

        [Test]
        public void TicketManager_CancelTicket_ClosedSession()
        {
            //Arrange
            var seatManager = Mock.Of<ISeatManager>();
            var priceManager = Mock.Of<IPriceManager>();
            var dataContext = Mock.Of<IDataContext>();

            var session = new Session() { TimeAndDate = new DateTime(2014, 10, 26), Status = SessionStatus.Closed };
            var seat = new Seat() { Row = Session.NUMBER_OF_ROWS, SeatNumber = Session.NUMBER_OF_SEATS, Session = session, Reserved = true };
            var ticket = new Ticket() { Price = 7, Seat = seat, TimeAndDate = DateTime.Now };

            var target = new TicketManager(seatManager, priceManager, dataContext);

            //Act
            Action action = () => { target.CancelTicket(ticket); };

            //Assert
            action.ShouldThrow<CinematicException>().WithMessage(Messages.SessionIsClosedCannotReturnTickets);
        }

        [Test]
        public void TicketManager_CancelTicket_TicketParamNullException()
        {
            //Arrange
            var seatManager = Mock.Of<ISeatManager>();
            var priceManager = Mock.Of<IPriceManager>();
            var dataContext = Mock.Of<IDataContext>();

            var target = new TicketManager(seatManager, priceManager, dataContext);

            //Act
            Action action = () => { target.CancelTicket(null); };

            //Assert
            action.ShouldThrow<ArgumentNullException>().WithMessage(new ArgumentNullException("ticket").Message);
        }

        [Test]
        public void TicketManager_CancelTicket_TicketSeatParamNullException()
        {
            //Arrange
            var seatManager = Mock.Of<ISeatManager>();
            var priceManager = Mock.Of<IPriceManager>();
            var dataContext = Mock.Of<IDataContext>();

            var target = new TicketManager(seatManager, priceManager, dataContext);

            //Act
            Action action = () => { target.CancelTicket(new Ticket() { Seat = null }); };

            //Assert
            action.ShouldThrow<ArgumentNullException>().WithMessage(new ArgumentNullException("ticket.Seat").Message);
        }

        //[TestMethod]
        //[ExpectedException(typeof(ArgumentNullException))]
        //public void TicketManager_CancelTicketTicketSeatSessionParamNullExceptionTest()
        //{
        //    //Arrange
        //    var seatManager = new StubISeatManager();
        //    var priceManager = new StubIPriceManager();
        //    var dataContext = new StubIDataContext();

        //    var target = new TicketManager(seatManager, priceManager, dataContext);

        //    //Act
        //    target.CancelTicket(new Ticket() { Seat = new Seat() { Session = null } });
        //}

        #endregion

        //#region Constructor tests

        //[TestMethod]
        //public void TicketManager_ConstructorRightTest()
        //{
        //    //Arrange
        //    var seatManager = new StubISeatManager();
        //    var priceManager = new StubIPriceManager();
        //    var dataContext = new StubIDataContext();

        //    //Act
        //    var target = new TicketManager(seatManager, priceManager, dataContext);

        //    //Assert
        //    PrivateObject po = new PrivateObject(target);

        //    Assert.IsInstanceOfType(po.GetField("_seatManager"), typeof(ISeatManager));
        //    Assert.IsNotNull(po.GetField("_seatManager"));
        //    Assert.IsInstanceOfType(po.GetField("_priceManager"), typeof(IPriceManager));
        //    Assert.IsNotNull(po.GetField("_priceManager"));
        //    Assert.IsInstanceOfType(po.GetField("_dataContext"), typeof(IDataContext));
        //    Assert.IsNotNull(po.GetField("_dataContext"));

        //}

        //[TestMethod]
        //[ExpectedException(typeof(ArgumentNullException))]
        //public void TicketManager_ConstructorSeatManagerParamNullExceptionTest()
        //{
        //    //Arrange
        //    var seatManager = (ISeatManager)null;
        //    var priceManager = new StubIPriceManager();
        //    var dataContext = new StubIDataContext();

        //    //Act
        //    var target = new TicketManager(seatManager, priceManager, dataContext);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(ArgumentNullException))]
        //public void TicketManager_ConstructorPriceManagerParamNullExceptionTest()
        //{
        //    //Arrange
        //    var seatManager = new StubISeatManager();
        //    var priceManager = (IPriceManager)null;
        //    var dataContext = new StubIDataContext();

        //    //Act
        //    var target = new TicketManager(seatManager, priceManager, dataContext);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(ArgumentNullException))]
        //public void TicketManager_ConstructorDataContextParamNullExceptionTest()
        //{
        //    //Arrange
        //    var seatManager = new StubISeatManager();
        //    var priceManager = new StubIPriceManager();
        //    var dataContext = (IDataContext)null;

        //    //Act
        //    var target = new TicketManager(seatManager, priceManager, dataContext);
        //}

        //#endregion
    }
}

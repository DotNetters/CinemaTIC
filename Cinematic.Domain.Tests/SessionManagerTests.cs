using NUnit.Framework;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Cinematic.Domain.Contracts;

namespace Cinematic.Domain.Tests
{
    [TestFixture]
    [Category("Cinematic.Domain.SessionManager")]
    public class SessionManagerTests
    {
        #region Initialization

        List<Session> _sessions;

        [SetUp]
        public void PrepareTests()
        {
            _sessions = new List<Session>()
            {
                new Session() { Id=1, Status=SessionStatus.Open, TimeAndDate=new DateTime(2016, 03, 21) }
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

        public void SessionManager_GetAvailableSessions_Right()
        {
            //Arrange
            var dataContext = Mock.Of<IDataContext>();

            var target = new SessionManager(dataContext);

            //Act
            var result = target.GetAvailableSessions();

            //Assert

        }

        #endregion
    }
}

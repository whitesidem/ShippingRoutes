using System.Collections.Generic;
using ShippingRoutes.BusinessObjects.Abstract;
using ShippingRoutes.BusinessObjects.Concrete;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using ShippingRoutes.DAL.Abstract;
using ShippingRoutes.Entities;
using ShippingRoutes.Models;

namespace TestShippingRoutes
{


    /// <summary>
    ///This is a UNIT test class for RouteProcessorTest and is intended
    ///to contain all RouteProcessorTest Unit Tests based on direct routes
    ///</summary>
    [TestClass()]
    public class RouteProcessorDirectTest
    {
        private IRouteProcessor _targetProcessor;
        private Mock<IRouteDataRepository> _mockRouteDataRepository;

        //Equivilent of [Setup] in NUnit
        [TestInitialize()]
        public void MyTestInitialize()
        {
            _mockRouteDataRepository = new Mock<IRouteDataRepository>();
            _targetProcessor = new RouteProcessor(_mockRouteDataRepository.Object);
        }


        /// <summary>
        /// Test Can NOT journey from  A to nowhere
        ///</summary>
        [TestMethod()]
        public void Retrieve_Valid_Direct_Journey_With_No_Stops()
        {
            //Arrange

            //Act - return invalid journey
            JourneyDetails journeyDetails = _targetProcessor.CalcJourneyDetailsForDirectRouteSet("A");

            //Assert
            Assert.IsNotNull(journeyDetails);
            Assert.IsFalse(journeyDetails.IsValidJourney);

        }



        /// <summary>
        /// Test Can journey from  A to C in one stop
        ///</summary>
        [TestMethod()]
        public void Retrieve_Valid_Direct_Journey_With_One_Stop()
        {
            //Arrange
            //Route A to C
            _mockRouteDataRepository.Setup(m => m.RetrieveDirectShippingRouteBetweenPorts("A", "C")).Returns(
                new ShippingRoute("A", "C", 10));

            //Act - return valid single stop journey
            JourneyDetails journeyDetails = _targetProcessor.CalcJourneyDetailsForDirectRouteSet("A", "C");

            //Assert
            Assert.IsNotNull(journeyDetails);
            Assert.IsTrue(journeyDetails.IsValidJourney);
            Assert.AreEqual(1, journeyDetails.TotalStops);

        }


        /// <summary>
        /// Test Can journey from  A to C in one stop
        ///</summary>
        [TestMethod()]
        public void Retrieve_Invalid_Direct_Journey_With_One_Stop()
        {
            //Arrange
            //Route A to C does not exist
            _mockRouteDataRepository.Setup(m => m.RetrieveDirectShippingRouteBetweenPorts("A", "C")).Returns(
                (ShippingRoute)null);

            //Act - attempt to fnd journey that does not exist in one stop
            JourneyDetails journeyDetails = _targetProcessor.CalcJourneyDetailsForDirectRouteSet("A", "C");

            //Assert
            Assert.IsNotNull(journeyDetails);
            Assert.IsFalse(journeyDetails.IsValidJourney);

        }


        /// <summary>
        /// Test Can journey from  A to B to C in two stops
        ///</summary>
        [TestMethod()]
        public void Retrieve_Valid_Direct_Journey_With_Two_Stops()
        {
            //Arrange
            //Route A to B
            _mockRouteDataRepository.Setup(m => m.RetrieveDirectShippingRouteBetweenPorts("A", "B")).Returns(
                new ShippingRoute("A", "B", 10));
            //Route B to C
            _mockRouteDataRepository.Setup(m => m.RetrieveDirectShippingRouteBetweenPorts("B", "C")).Returns(
                new ShippingRoute("B", "C", 10));

            //Act - journey from A to B to C
            JourneyDetails journeyDetails = _targetProcessor.CalcJourneyDetailsForDirectRouteSet("A", "B", "C");

            //Assert
            Assert.IsNotNull(journeyDetails);
            Assert.IsTrue(journeyDetails.IsValidJourney);
            Assert.AreEqual(2, journeyDetails.TotalStops);

        }


        /// <summary>
        /// Test Can NOT journey from  A to B to C in two stops - as B to C is missing
        ///</summary>
        [TestMethod()]
        public void Retrieve_Invalid_Direct_Journey_With_Two_Stops()
        {
            //Arrange
            //Route A to B
            _mockRouteDataRepository.Setup(m => m.RetrieveDirectShippingRouteBetweenPorts("A", "B")).Returns(
                new ShippingRoute("A", "B", 10));
            //Route B to C does not exist
            _mockRouteDataRepository.Setup(m => m.RetrieveDirectShippingRouteBetweenPorts("B", "C")).Returns(
                (ShippingRoute)null);

            //Act - journey from A to B to C   (no direct route exists)
            JourneyDetails journeyDetails = _targetProcessor.CalcJourneyDetailsForDirectRouteSet("A", "B", "C");

            //Assert
            Assert.IsNotNull(journeyDetails);
            Assert.IsFalse(journeyDetails.IsValidJourney);

        }


        /// <summary>
        /// Test duration of direct stop from  A to C
        ///</summary>
        [TestMethod()]
        public void Retrieve_Direct_Journey_Time_With_One_Stop()
        {
            //Arrange
            //Route A to C
            _mockRouteDataRepository.Setup(m => m.RetrieveDirectShippingRouteBetweenPorts("A", "C")).Returns(
                new ShippingRoute("A", "C", 3));

            //Act - return valid single stop journey
            JourneyDetails journeyDetails = _targetProcessor.CalcJourneyDetailsForDirectRouteSet("A", "C");

            //Assert
            Assert.IsNotNull(journeyDetails);
            Assert.IsTrue(journeyDetails.IsValidJourney);
            Assert.AreEqual(3, journeyDetails.TotalJourneyDays);

        }

        /// <summary>
        /// Test duration of direct stop from  A to B to C to D to E
        ///</summary>
        [TestMethod()]
        public void Retrieve_Direct_Journey_Time_With_Four_Stops()
        {
            //Arrange
            //Route A to B
            _mockRouteDataRepository.Setup(m => m.RetrieveDirectShippingRouteBetweenPorts("A", "B")).Returns(
                new ShippingRoute("A", "B", 3));
            //Route B to C
            _mockRouteDataRepository.Setup(m => m.RetrieveDirectShippingRouteBetweenPorts("B", "C")).Returns(
                new ShippingRoute("B", "C", 1));
            //Route C to D
            _mockRouteDataRepository.Setup(m => m.RetrieveDirectShippingRouteBetweenPorts("C", "D")).Returns(
                new ShippingRoute("C", "D", 10));
            //Route D to E
            _mockRouteDataRepository.Setup(m => m.RetrieveDirectShippingRouteBetweenPorts("D", "E")).Returns(
                new ShippingRoute("D", "E", 2));

            //Act - return valid 3 stop journey
            JourneyDetails journeyDetails = _targetProcessor.CalcJourneyDetailsForDirectRouteSet("A", "B", "C", "D", "E");

            //Assert
            Assert.IsNotNull(journeyDetails);
            Assert.IsTrue(journeyDetails.IsValidJourney);
            Assert.AreEqual(4, journeyDetails.TotalStops);
            Assert.AreEqual(16, journeyDetails.TotalJourneyDays);

        }


    }
}

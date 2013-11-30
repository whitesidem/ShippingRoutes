using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ShippingRoutes.BusinessObjects.Abstract;
using ShippingRoutes.BusinessObjects.Concrete;
using ShippingRoutes.DAL.Concrete;
using ShippingRoutes.Entities;
using ShippingRoutes.Models;

namespace TestShippingRoutes
{
    /// <summary>
    /// Tests integration of business class with data access layer
    /// Covering all BDD scenarios of the shipping test
    /// </summary>
    [TestClass]
    public class IntegrationTestsForGivenScenarios
    {
        private IRouteProcessor _targetProcessor;

        //Equivilent of [Setup] in NUnit
        [TestInitialize()]
        public void MyTestInitialize()
        {
            var routeDataRepository = new RouteDataRepository();

            //Landscape Port Data & Routes
            var portsLandscape = new List<Port>()
                            {
                                new Port("BA", "Buenos Aires")
                                ,
                                new Port("NY", "New York")
                                ,
                                new Port("CB", "Casablanca")
                                ,
                                new Port("CT", "Cape Town")
                                ,
                                new Port("LV", "Liverpool")
                            };
            routeDataRepository.PopulatePorts(portsLandscape);

            var shippingRouteLandscape = new List<ShippingRoute>()
                                                              {
                                                                new ShippingRoute("BA","NY",6)
                                                                ,new ShippingRoute("BA","CB",5)
                                                                ,new ShippingRoute("BA","CT",4)
                                                                ,new ShippingRoute("NY","LV",4)
                                                                ,new ShippingRoute("LV","CB",3)
                                                                ,new ShippingRoute("LV","CT",6)
                                                                ,new ShippingRoute("CB","LV",3)
                                                                ,new ShippingRoute("CB","CT",6)
                                                                ,new ShippingRoute("CT","NY",8)
                                                              };
            routeDataRepository.PopulateShippingRoutes(shippingRouteLandscape);
                
            _targetProcessor = new RouteProcessor(routeDataRepository);
        }


        /// <summary>
        /// What is the total journey time for the following direct routes : Buenos Aires -> New York -> Liverpool
        ///</summary>
        [TestMethod()]
        public void Test_Direct_BA_NY_LV_Journey_Time()
        {
            //Arrange

            //Act - return valid single stop journey
            JourneyDetails journeyDetails = _targetProcessor.CalcJourneyDetailsForDirectRouteSet("BA", "NY", "LV");

            //Assert
            Assert.IsNotNull(journeyDetails);
            Assert.IsTrue(journeyDetails.IsValidJourney);
            Assert.AreEqual(10, journeyDetails.TotalJourneyDays);
        }

        /// <summary>
        /// What is the total journey time for the following direct routes :	Buenos Aires -> Casablanca -> Liverpool
        ///</summary>
        [TestMethod()]
        public void Test_Direct_BA_CB_LV_Journey_Time()
        {
            //Arrange

            //Act - return valid single stop journey
            JourneyDetails journeyDetails = _targetProcessor.CalcJourneyDetailsForDirectRouteSet("BA", "CB", "LV");

            //Assert
            Assert.IsNotNull(journeyDetails);
            Assert.IsTrue(journeyDetails.IsValidJourney);
            Assert.AreEqual(8, journeyDetails.TotalJourneyDays);
        }

        /// <summary>
        /// What is the total journey time for the following direct routes :	Buenos Aires -> New York -> Liverpool -> Cassablanca
        ///</summary>
        [TestMethod()]
        public void Test_Direct_BA_NY_LV_CB_Journey_Time()
        {
            //Arrange

            //Act - return valid single stop journey
            JourneyDetails journeyDetails = _targetProcessor.CalcJourneyDetailsForDirectRouteSet("BA", "NY", "LV", "CB");

            //Assert
            Assert.IsNotNull(journeyDetails);
            Assert.IsTrue(journeyDetails.IsValidJourney);
            Assert.AreEqual(13, journeyDetails.TotalJourneyDays);
        }


        /// <summary>
        /// What is the total journey time for the following direct routes :	Buenos Aires -> Cape Town -> Cassablanca
        ///</summary>
        [TestMethod()]
        public void Test_Direct_BA_CT_CB_Invalid_Route()
        {
            //Arrange

            //Act - return valid single stop journey
            JourneyDetails journeyDetails = _targetProcessor.CalcJourneyDetailsForDirectRouteSet("BA", "CT", "CB");

            //Assert
            Assert.IsNotNull(journeyDetails);
            Assert.IsFalse(journeyDetails.IsValidJourney);
        }


        /// <summary>
        /// Find the shortest journey time for the following routes:	Buenos Aires -> Liverpool
        ///</summary>
        [TestMethod()]
        public void Test_InDirect_BA_LV_Shortest_Time()
        {
            //Arrange

            //Act - return valid single stop journey
            JourneyDetailPermutations journeyDetailPermutations = _targetProcessor.CalcJourneyDetailsForInDirectRoutes("BA", "LV");

            //Assert
            Assert.IsNotNull(journeyDetailPermutations);
            Assert.IsTrue(journeyDetailPermutations.HasValidPermutations);
            Assert.AreEqual(8, journeyDetailPermutations.GetShortestJourney());
        }

        /// <summary>
        /// Find the shortest journey time for the following routes:	New York -> New York
        ///</summary>
        [TestMethod()]
        public void Test_InDirect_NY_NY_Shortest_Time()
        {
            //Arrange

            //Act - return valid single stop journey
            JourneyDetailPermutations journeyDetailPermutations = _targetProcessor.CalcJourneyDetailsForInDirectRoutes("NY", "NY");

            //Assert
            Assert.IsNotNull(journeyDetailPermutations);
            Assert.IsTrue(journeyDetailPermutations.HasValidPermutations);
            Assert.AreEqual(18, journeyDetailPermutations.GetShortestJourney());
        }


        /// <summary>
        /// Find the number of routes from Liverpool to Liverpool with a maximum number of 3 stops.
        ///</summary>
        [TestMethod()]
        public void Test_Total_Routes_LV_LV_With_Max_Stops_3()
        {
            //Arrange

            //Act - return valid single stop journey
            JourneyDetailPermutations journeyDetailPermutations = _targetProcessor.CalcJourneyDetailsForInDirectRoutes("LV", "LV", maxStops: 3);

            //Assert
            Assert.IsNotNull(journeyDetailPermutations);
            Assert.IsTrue(journeyDetailPermutations.HasValidPermutations);
            Assert.AreEqual(2, journeyDetailPermutations.TotalValidJourneys);
        }

        /// <summary>
        /// Find the number of routes from Buenos Aires to Liverpool where exactly 4 stops are made.
        ///</summary>
        [TestMethod()]
        public void Test_Total_Routes_BA_LV_With_Exact_Stops_4()
        {
            //Arrange

            //Act - return valid single stop journey
            JourneyDetailPermutations journeyDetailPermutations = _targetProcessor.CalcJourneyDetailsForInDirectRoutes("BA", "LV", minStops: 4, maxStops: 4);

            //Assert
            Assert.IsNotNull(journeyDetailPermutations);
            Assert.IsTrue(journeyDetailPermutations.HasValidPermutations);
            Assert.AreEqual(1, journeyDetailPermutations.TotalValidJourneys);
        }


        /// <summary>
        /// Find the number of routes from Liverpool to Liverpool where the journey time is less than or equal to 25 days.
        ///</summary>
        [TestMethod()]
        public void Test_Total_Routes_LV_LV_Less_Than_Or_Equal_25_Days()
        {
            //Arrange

            //Act - return valid single stop journey
            JourneyDetailPermutations journeyDetailPermutations = _targetProcessor.CalcJourneyDetailsForInDirectRoutes("LV", "LV", maxJourneyDays: 25);

            //Assert
            Assert.IsNotNull(journeyDetailPermutations);
            Assert.IsTrue(journeyDetailPermutations.HasValidPermutations);
            Assert.AreEqual(3, journeyDetailPermutations.TotalValidJourneys);
        }

        /// <summary>
        /// Find the number of routes from Liverpool to Liverpool where the journey time is less than or equal to 18 days.
        ///</summary>
        [TestMethod()]
        public void Test_Total_Routes_LV_LV_Less_Than_Or_Equal_18_Days()
        {
            //Arrange

            //Act - return valid single stop journey
            JourneyDetailPermutations journeyDetailPermutations = _targetProcessor.CalcJourneyDetailsForInDirectRoutes("LV", "LV", maxJourneyDays: 18);

            //Assert
            Assert.IsNotNull(journeyDetailPermutations);
            Assert.IsTrue(journeyDetailPermutations.HasValidPermutations);
            Assert.AreEqual(2, journeyDetailPermutations.TotalValidJourneys);
        }


    }
}

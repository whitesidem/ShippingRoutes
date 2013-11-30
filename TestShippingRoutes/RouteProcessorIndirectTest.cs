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
    ///to contain all RouteProcessorTest Unit Tests based on Indirect tests
    ///</summary>
    [TestClass()]
    public class RouteProcessorIndirectTest
    {
        private IRouteProcessor _targetProcessor;
        private Mock<IRouteDataRepository> _mockRouteDataRepository;

        //Equivilent of [Setup] in NUnit
        [TestInitialize()]
        public void MyTestInitialize()
        {
            _mockRouteDataRepository = new Mock<IRouteDataRepository>();
            _targetProcessor = new RouteProcessor(_mockRouteDataRepository.Object);

            //  A to B,   
            //       B to C 
            //       B to D  
            //         C to D 
            //         C to E 
            //         C to B
            //             D to C 
            //             D to A
            _mockRouteDataRepository.Setup(m => m.ListAllDirectShippingRoutesForPort("A")).Returns(
                new List<ShippingRoute>()
                    {
                        new ShippingRoute("A", "B", 1)
                    }
                );
            _mockRouteDataRepository.Setup(m => m.ListAllDirectShippingRoutesForPort("B")).Returns(
                new List<ShippingRoute>()
                    {
                        new ShippingRoute("B", "C", 2)
                        ,new ShippingRoute("B", "D", 3)
                    }
                );

            _mockRouteDataRepository.Setup(m => m.ListAllDirectShippingRoutesForPort("C")).Returns(
                new List<ShippingRoute>()
                    {
                        new ShippingRoute("C", "D", 2)
                        ,new ShippingRoute("C", "B", 4)
                        ,new ShippingRoute("C", "E", 1)
                    }
                );

            _mockRouteDataRepository.Setup(m => m.ListAllDirectShippingRoutesForPort("D")).Returns(
                new List<ShippingRoute>()
                    {
                        new ShippingRoute("D", "C", 2)
                        ,new ShippingRoute("D", "A", 3)
                    }
                );

            _mockRouteDataRepository.Setup(m => m.ListAllDirectShippingRoutesForPort("E")).Returns(
                new List<ShippingRoute>()
                );

            _mockRouteDataRepository.Setup(m => m.ListAllDirectShippingRoutesForPort("Z")).Returns(
                new List<ShippingRoute>()
                );

            //Invalid Data
            _mockRouteDataRepository.Setup(m => m.ListAllDirectShippingRoutesForPort("X")).Returns(
                new List<ShippingRoute>()
                    {
                  new ShippingRoute("X", "X", 2)
                    }
                );


        }

        [TestMethod()]
        public void Retrieve_Valid_InDirect_Journey_Only_1_Permutation()
        {
            //Arrange

            //Act - return valid 1 stop journey
            JourneyDetailPermutations journeyDetailPermutations = _targetProcessor.CalcJourneyDetailsForInDirectRoutes("A", "B");

            //Assert
            Assert.IsNotNull(journeyDetailPermutations);
            Assert.IsTrue(journeyDetailPermutations.HasValidPermutations);
            Assert.AreEqual(1,journeyDetailPermutations.TotalValidJourneys);
        }

        [TestMethod()]
        public void Retrieve_Valid_InDirect_Journey_2_Permutations()
        {
            //Arrange

            //Act - return two valid journeys - A-B-C-D &  A-B-D  
            JourneyDetailPermutations journeyDetailPermutations = _targetProcessor.CalcJourneyDetailsForInDirectRoutes("A", "D");

            //Assert
            Assert.IsNotNull(journeyDetailPermutations);
            Assert.IsTrue(journeyDetailPermutations.HasValidPermutations);
            Assert.AreEqual(2, journeyDetailPermutations.TotalValidJourneys);

        }


        [TestMethod()]
        public void Retrieve_InDirect_Journey_0_Permutations_To_End()
        {
            //Arrange

            //Act - return no valid journeys   
            JourneyDetailPermutations journeyDetailPermutations = _targetProcessor.CalcJourneyDetailsForInDirectRoutes("A", "Z");

            //Assert
            Assert.IsNotNull(journeyDetailPermutations);
            Assert.IsFalse(journeyDetailPermutations.HasValidPermutations);

        }

        [TestMethod()]
        public void Retrieve_InDirect_Journey_0_Permutations_From_Start()
        {
            //Arrange

            //Act - return no valid journeys   
            JourneyDetailPermutations journeyDetailPermutations = _targetProcessor.CalcJourneyDetailsForInDirectRoutes("Z", "A");

            //Assert
            Assert.IsNotNull(journeyDetailPermutations);
            Assert.IsFalse(journeyDetailPermutations.HasValidPermutations);

        }

        [TestMethod()]
        public void Retrieve_InDirect_Journey_Same_Start_End()
        {
            //Arrange

            //Act - return no valid journeys   
            JourneyDetailPermutations journeyDetailPermutations = _targetProcessor.CalcJourneyDetailsForInDirectRoutes("X", "X");

            //Assert
            Assert.IsNotNull(journeyDetailPermutations);
            Assert.IsFalse(journeyDetailPermutations.HasValidPermutations);

        }


        [TestMethod()]
        public void Retrieve_Valid_Full_Return_Journey_A()
        {
            //Arrange

            //Act - return two valid journeys - A-B-C-D-A &  A-B-D-A  
            JourneyDetailPermutations journeyDetailPermutations = _targetProcessor.CalcJourneyDetailsForInDirectRoutes("A", "A");

            //Assert
            Assert.IsNotNull(journeyDetailPermutations);
            Assert.IsTrue(journeyDetailPermutations.HasValidPermutations);
            Assert.AreEqual(2, journeyDetailPermutations.TotalValidJourneys);

        }

        [TestMethod()]
        public void Retrieve_Valid_Full_Return_Journey_B()
        {
            //Arrange

            //Act - return two valid journeys - B-C-D-A-B &  B-D-A-B  &  B-C-B &  B-D-C-B
            JourneyDetailPermutations journeyDetailPermutations = _targetProcessor.CalcJourneyDetailsForInDirectRoutes("B", "B");

            //Assert
            Assert.IsNotNull(journeyDetailPermutations);
            Assert.IsTrue(journeyDetailPermutations.HasValidPermutations);
            Assert.AreEqual(4, journeyDetailPermutations.TotalValidJourneys);

        }


        [TestMethod()]
        public void Retrieve_Shortest_Journey_From_Multiple_Permutations()
        {
            //Arrange

            //Act - return two valid journeys - B-C-D-A-B = 8   &  B-D-A-B = 7 &   B-C-B = 6  &   B-D-C-B = 9
            JourneyDetailPermutations journeyDetailPermutations = _targetProcessor.CalcJourneyDetailsForInDirectRoutes("B", "B");

            //Assert
            Assert.IsNotNull(journeyDetailPermutations);
            Assert.IsTrue(journeyDetailPermutations.HasValidPermutations);
            Assert.AreEqual(4, journeyDetailPermutations.TotalValidJourneys);
            Assert.AreEqual(6,journeyDetailPermutations.GetShortestJourney());

        }


        [TestMethod()]
        public void Retrieve_Total_Journeys_Restricted_By_Max_Stops()
        {
            //Arrange

            //Act - return two valid journeys - B-C-D-A-B    &  B-D-A-B   &   B-C-B   &   B-D-C-B 
            JourneyDetailPermutations journeyDetailPermutations = _targetProcessor.CalcJourneyDetailsForInDirectRoutes("B", "B", maxStops: 3);

            //Assert
            Assert.IsNotNull(journeyDetailPermutations);
            Assert.IsTrue(journeyDetailPermutations.HasValidPermutations);
            Assert.AreEqual(3, journeyDetailPermutations.TotalValidJourneys);

        }

        [TestMethod()]
        public void Retrieve_Total_Journeys_Restricted_By_Exact_Stops()
        {
            //Arrange

            //Act - return two valid journeys - B-C-D-A-B    &  B-D-A-B   &   B-C-B   &   B-D-C-B 
            JourneyDetailPermutations journeyDetailPermutations = _targetProcessor.CalcJourneyDetailsForInDirectRoutes("B", "B", minStops: 3, maxStops: 3);

            //Assert
            Assert.IsNotNull(journeyDetailPermutations);
            Assert.IsTrue(journeyDetailPermutations.HasValidPermutations);
            Assert.AreEqual(2, journeyDetailPermutations.TotalValidJourneys);

        }

        [TestMethod()]
        public void Retrieve_Total_Journeys_Restricted_By_Max_Days()
        {
            //Arrange

            //Act - return two valid journeys - B-C-D-A-B = 8   &  B-D-A-B =  & 7  B-C-B =  & 6  B-D-C-B = 9
            JourneyDetailPermutations journeyDetailPermutations = _targetProcessor.CalcJourneyDetailsForInDirectRoutes("B", "B", maxJourneyDays: 8);

            //Assert
            Assert.IsNotNull(journeyDetailPermutations);
            Assert.IsTrue(journeyDetailPermutations.HasValidPermutations);
            Assert.AreEqual(3, journeyDetailPermutations.TotalValidJourneys);

        }

        [TestMethod()]
        public void Retrieve_Total_Journeys_Restricted_By_Max_Days_Inc_None()
        {
            //Arrange

            //Act - return two valid journeys - B-C-D-A-B = 8   &  B-D-A-B =  & 7  B-C-B =  & 6  B-D-C-B = 9
            JourneyDetailPermutations journeyDetailPermutations = _targetProcessor.CalcJourneyDetailsForInDirectRoutes("B", "B", maxJourneyDays: 5);

            //Assert
            Assert.IsNotNull(journeyDetailPermutations);
            Assert.IsFalse(journeyDetailPermutations.HasValidPermutations);

        }


        [TestMethod()]
        public void Retrieve_Total_Journeys_Restricted_By_Max_Days_Inc_All()
        {
            //Arrange

            //Act - return two valid journeys - B-C-D-A-B = 8   &  B-D-A-B =  & 7  B-C-B =  & 6  B-D-C-B = 9
            JourneyDetailPermutations journeyDetailPermutations = _targetProcessor.CalcJourneyDetailsForInDirectRoutes("B", "B", maxJourneyDays: 9);

            //Assert
            Assert.IsNotNull(journeyDetailPermutations);
            Assert.IsTrue(journeyDetailPermutations.HasValidPermutations);
            Assert.AreEqual(4, journeyDetailPermutations.TotalValidJourneys);

        }

        [TestMethod()]
        public void Retrieve_Total_Journeys_Restricted_By_Max_Days_And_Stops()
        {
            //Arrange

            //Act - return two valid journeys - B-C-D-A-B = 8   &  B-D-A-B =  & 7  B-C-B =  & 6  B-D-C-B = 9
            JourneyDetailPermutations journeyDetailPermutations = _targetProcessor.CalcJourneyDetailsForInDirectRoutes("B", "B", minStops: 3, maxStops: 3, maxJourneyDays: 9);

            //Assert
            Assert.IsNotNull(journeyDetailPermutations);
            Assert.IsTrue(journeyDetailPermutations.HasValidPermutations);
            Assert.AreEqual(2, journeyDetailPermutations.TotalValidJourneys);
            Assert.AreEqual(7, journeyDetailPermutations.GetShortestJourney());

        }


    }
}

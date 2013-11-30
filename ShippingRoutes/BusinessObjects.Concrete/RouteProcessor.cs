using System.Linq;
using System.Threading.Tasks;
using ShippingRoutes.BusinessObjects.Abstract;
using ShippingRoutes.DAL.Abstract;
using ShippingRoutes.Entities;
using ShippingRoutes.Models;

namespace ShippingRoutes.BusinessObjects.Concrete
{

    /// <summary>
    /// Implementation of Business Logic Processor for cacluating possible shipping routes between destinations
    /// </summary>
    public class RouteProcessor : IRouteProcessor
    {
        private readonly IRouteDataRepository _routeDataRepository;
        private JourneyDetailPermutations _journeyDetailsPermutations;
        private int _minStops;
        private int _maxStops;
        private int _maxJourneyDays;
        private string _portCodeTo;

        /// <summary>
        /// Complete list of all valid permutations of journeys
        /// Filled by call to CalcJourneyDetailsForInDirectRoutes
        /// This allows any analysis of any of the journey permutations that passed the filters.
        /// </summary>
        public JourneyDetailPermutations JourneyDetailsPermutations
        {
            get { return _journeyDetailsPermutations; }
        }


        /// <summary>
        /// Dependency Injection for data repository (can be set manually or via MEF or Ninject)
        /// </summary>
        /// <param name="routeDataRepository"></param>
        public RouteProcessor(IRouteDataRepository routeDataRepository)
        {
            _routeDataRepository = routeDataRepository;
        }

        #region DIRECT route methods

        /// <summary>
        /// Calc set of journey stop details from passed in set of direct stops
        /// </summary>
        /// <param name="portCodeFrom">Source Port Code</param>
        /// <param name="destinationPortStops">Set of Port code stops</param>
        /// <returns>JourneyDetails instance - which has an IsValidJourney property to check</returns>
        public JourneyDetails CalcJourneyDetailsForDirectRouteSet(string portCodeFrom, params string[] destinationPortStops)
        {
            var journeyDetails = new JourneyDetails {IsValidJourney = false};

            //If no stops in journey - return as invalid journey
            if(!destinationPortStops.Any())
            {
                return journeyDetails;
            }

            string currPortFrom = portCodeFrom;

            //Iterate through the passed set of destinations in order
            foreach(string destinationPort in destinationPortStops)
            {
                var shippingRoute = _routeDataRepository.RetrieveDirectShippingRouteBetweenPorts(currPortFrom,
                                                                                                 destinationPort);
                if(shippingRoute==null)
                {
                    //No direct route between the two ports
                    //Return invalid journey details.
                    return journeyDetails;
                }

                journeyDetails.AddJourneyDetail(shippingRoute);
                currPortFrom = shippingRoute.PortCodeTo;            //Set port from to next stop, so we can check next leg of journey
            }

            journeyDetails.IsValidJourney = true;
            return journeyDetails;
        }

        #endregion DIRECT route methods


        #region INDIRECT route methods

        /// <summary>
        /// Calc set of all permutations of journeys based on source port to destination port.
        /// The permutations calculated can be restricted to the min/max stops and max days optional parameters
        /// Note: Recommended that max stops is always set to avoid possible large number of permutations
        /// </summary>
        /// <param name="portCodeFrom"></param>
        /// <param name="portCodeTo"></param>
        /// <param name="minStops"></param>
        /// <param name="maxStops"></param>
        /// <param name="maxJourneyDays"></param>
        /// <returns>JourneyDetailsPermutations instance which has a collection of all valid journeys passing the filters
        ///  Check HasValidPermutations property to check if valid permutations exist.
        ///  </returns>
        public JourneyDetailPermutations CalcJourneyDetailsForInDirectRoutes(string portCodeFrom, string portCodeTo, int minStops = -1, int maxStops = -1, int maxJourneyDays = -1)
        {
            //Start with empty set of permutations - which equates to invalid set.
            _journeyDetailsPermutations = new JourneyDetailPermutations();
            _minStops = minStops;
            _maxStops = maxStops;
            _maxJourneyDays = maxJourneyDays;
            _portCodeTo = portCodeTo;

            RecursiveAddIndirectJourneyRoutesFromPort(portCodeFrom, new JourneyDetails(), null);

            return _journeyDetailsPermutations;
        }

        /// <summary>
        /// Using recursion to go down all possible tree levels of routes.
        /// Code in here exists to Ensuring no infinite looping
        /// Also does checking against the possible filters for min/max stops and time - stopping recursion down tree branch immediately when exceeding any filters.
        /// </summary>
        /// <param name="portCodeFrom"></param>
        /// <param name="journeyDetails"></param>
        /// <param name="nextShippingRoute"></param>
        private void RecursiveAddIndirectJourneyRoutesFromPort(string portCodeFrom, JourneyDetails journeyDetails, ShippingRoute nextShippingRoute)
        {
            //First call should not add shipping route
            if (nextShippingRoute != null)
            {
                journeyDetails.AddJourneyDetail(nextShippingRoute);
            }

            var shippingRouteColl = _routeDataRepository.ListAllDirectShippingRoutesForPort(portCodeFrom);
            //Using Parallel processing for fast processing of all permutations
            //Process through the current branch  
            Parallel.ForEach(shippingRouteColl,
                             shippingRoute => ProcessIndirectRoute(portCodeFrom, journeyDetails, shippingRoute));
        }

        private void ProcessIndirectRoute(string portCodeFrom, JourneyDetails journeyDetails, ShippingRoute shippingRoute)
        {
            string shipRouteTo = shippingRoute.PortCodeTo;

            //Validate for completely invalid scenario - ship from and to the same
            if (shipRouteTo.Equals(portCodeFrom))
            {
                return;
            }

            //Check if this route exceeds any set maximum days - if so do not continue down this route
            if (_maxJourneyDays != -1 &&
                journeyDetails.TotalJourneyDays + shippingRoute.TravelDays > _maxJourneyDays)
            {
                return;
            }

            //Check if this route exceeds any set maximum stops - if so do not continue down this route
            if (_maxStops != -1 && journeyDetails.TotalStops + 1 > _maxStops)
            {
                return;
            }

            //Check if reached final destination port
            if (shipRouteTo.Equals(_portCodeTo))
            {
                //Performs the ONLY if exact number of stops ...
                //Only add as valid permutation if number of stops ge minimum stops or there is no filter for minimum stops
                if (_minStops == -1 || journeyDetails.TotalStops + 1 >= _minStops)
                {
                    var completeJourneyDetail = journeyDetails.Clone() as JourneyDetails;
                    completeJourneyDetail.AddJourneyDetail(shippingRoute);
                    _journeyDetailsPermutations.AddValidJourneyDetails(completeJourneyDetail);
                }
                return;
            }

            //Ensure no looping back to previous port.
            if (journeyDetails.ShippingRoutes.Exists(s => s.PortCodeFrom == shipRouteTo))
            {
                return;
            }

            //Recursive call to next level of tree of routes
            //Current collection of Routes are cloned so that we get seperate collections per possible permutation
            RecursiveAddIndirectJourneyRoutesFromPort(shipRouteTo,
                                                      journeyDetails.Clone() as JourneyDetails,
                                                      shippingRoute);
        }

        #endregion INDIRECT route methods


    }
}

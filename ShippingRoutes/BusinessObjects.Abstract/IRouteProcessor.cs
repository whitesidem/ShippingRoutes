using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingRoutes.Models;

namespace ShippingRoutes.BusinessObjects.Abstract
{
    public interface IRouteProcessor
    {

        /// <summary>
        /// Calc set of journey stop details from passed in set of direct stops
        /// </summary>
        /// <param name="portCodeFrom">Source Port Code</param>
        /// <param name="destinationPortStops">Set of Port code stops</param>
        /// <returns>JourneyDetails instance - which has an IsValidJourney property to check</returns>
        JourneyDetails CalcJourneyDetailsForDirectRouteSet(string portCodeFrom, params string[] destinationPortStops);

        /// <summary>
        /// Calc set of all permutations of journeys based on source port to destination port.
        /// The permutations calculated can be restricted to the min/max stops and max days optional parameters
        /// </summary>
        /// <param name="portCodeFrom"></param>
        /// <param name="portCodeTo"></param>
        /// <param name="minStops"></param>
        /// <param name="maxStops"></param>
        /// <param name="maxJourneyDays"></param>
        /// <returns>JourneyDetailsPermutations instance which has a collection of all valid journeys passing the filters- or null where no valid journeys exist</returns>
        JourneyDetailPermutations CalcJourneyDetailsForInDirectRoutes(string portCodeFrom, string portCodeTo, int minStops = -1, int maxStops = -1, int maxJourneyDays = -1);


    }
}

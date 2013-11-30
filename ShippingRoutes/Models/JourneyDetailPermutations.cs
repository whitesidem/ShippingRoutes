using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingRoutes.Extensions;

namespace ShippingRoutes.Models
{
    /// <summary>
    /// Controls list of all valid permutations of indirect journeys.
    /// </summary>
    public class JourneyDetailPermutations
    {

        private List<JourneyDetails> _validJourneyDetailsCollections = new List<JourneyDetails>();

        public List<JourneyDetails> ValidJourneyDetailsCollections
        {
            get
            {
                return _validJourneyDetailsCollections;
            }
        }

        public bool HasValidPermutations
        {
            get { return _validJourneyDetailsCollections.Count > 0; }
        }


        public void AddValidJourneyDetails(JourneyDetails journeyDetails)
        {
            _validJourneyDetailsCollections.Add(journeyDetails);
        }

        public int TotalValidJourneys
        {
            get { return _validJourneyDetailsCollections.Count; }
        }

        /// <summary>
        /// Return the shortest journey time from all the permutations of journeys
        /// </summary>
        /// <returns>the shortest journey time or 0 if no journeys exist</returns>
        public int GetShortestJourney()
        {
            if (_validJourneyDetailsCollections.Count == 0) return 0;
            var journeyDetails = _validJourneyDetailsCollections.MinBy(j => j.TotalJourneyDays);             
            return journeyDetails.TotalJourneyDays;
        }


    }
}

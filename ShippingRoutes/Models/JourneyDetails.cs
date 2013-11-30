using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingRoutes.Entities;

namespace ShippingRoutes.Models
{
    /// <summary>
    /// Stores details of all routes (stops) involved in journey for one possible permutation of A to B.
    /// Therefore allowing any data analysis on stops in journey and times of journey
    /// </summary>
    public class JourneyDetails : ICloneable
    {
        private List<ShippingRoute> _shippingRoutes = new List<ShippingRoute>();

        public bool IsValidJourney { get; set; }

        public List<ShippingRoute> ShippingRoutes
        {
            get { return _shippingRoutes; }
            private set { _shippingRoutes = value; }
        }

        public void AddJourneyDetail(ShippingRoute shippingRoute)
        {
            _shippingRoutes.Add(shippingRoute);
        }

        public int TotalStops
        {
            get { return _shippingRoutes.Count; }
        }

        public int TotalJourneyDays
        {
            get { return _shippingRoutes.Sum(s => s.TravelDays); }
        }

        /// <summary>
        /// Shallow clone so we have a unique list of shipping routes
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            JourneyDetails clone = new JourneyDetails();
            foreach(var shipRoute in this.ShippingRoutes)
            {
                clone.AddJourneyDetail(shipRoute);   
            }
            return clone;
        }
    }
}

namespace ShippingRoutes.Entities
{

    /// <summary>
    /// Route information containing details of a single journey time 
    /// from one port to another
    /// </summary>
    public class ShippingRoute
    {

        public string PortCodeFrom { get; set; }
        public string PortCodeTo { get; set; }

        /// <summary>
        /// Number of days jopurney will take.
        /// Assumption: All journeys take whole number of days.
        /// </summary>
        public int TravelDays { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="portCodeFrom"></param>
        /// <param name="portCodeTo"></param>
        /// <param name="travelDays"></param>
        public ShippingRoute(string portCodeFrom, string portCodeTo, int travelDays)
        {
            PortCodeFrom = portCodeFrom;
            PortCodeTo = portCodeTo;
            TravelDays = travelDays;
        }


    }


}

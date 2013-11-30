namespace ShippingRoutes.Entities
{
    /// <summary>
    /// Port Information
    /// </summary>
    public class Port
    {

        /// <summary>
        /// Unique identifier key for a Port  
        /// </summary>
        public string PortCode { get; set; }

        /// <summary>
        /// Full Port Name
        /// </summary>
        public string PortName { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="portCode"></param>
        /// <param name="portName"></param>
        public Port(string portCode, string portName)
        {
            PortCode = portCode;
            PortName = portName;
        }


    }
}

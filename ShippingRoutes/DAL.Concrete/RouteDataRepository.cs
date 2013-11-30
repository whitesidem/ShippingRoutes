using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingRoutes.DAL.Abstract;
using ShippingRoutes.Entities;

namespace ShippingRoutes.DAL.Concrete
{


    /// <summary>
    /// Concrete data repository 
    /// reading from in memory
    /// </summary>
    public class RouteDataRepository : IRouteDataRepository
    {
        #region Member variables
        private List<Port> _portCollection;
        private List<ShippingRoute> _shippingRouteCollection;
        #endregion Member variables

        #region Catalogue Population

        public void PopulatePorts(List<Port> portCollection)
        {
            _portCollection = portCollection;
        }


        public void PopulateShippingRoutes(List<ShippingRoute> shippingRouteCollection)
        {
            _shippingRouteCollection = shippingRouteCollection;
        }

        #endregion Catalogue Population


        public List<Port> ListPorts()
        {
            return _portCollection;
        }

        public List<ShippingRoute> ListAllDirectShippingRoutesForPort(string portCodeFrom)
        {
            return _shippingRouteCollection.Where(s => s.PortCodeFrom.Equals(portCodeFrom)).Select(s => s).ToList();
        }

        public ShippingRoute RetrieveDirectShippingRouteBetweenPorts(string portCodeFrom, string portCodeTo)
        {
            return _shippingRouteCollection.Where(s => s.PortCodeFrom.Equals(portCodeFrom) && s.PortCodeTo.Equals(portCodeTo)).Select(s => s).FirstOrDefault();
        }

    }
}

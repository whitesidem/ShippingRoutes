using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingRoutes.Entities;

namespace ShippingRoutes.DAL.Abstract
{

    /// <summary>
    /// Data repository contract for shipping route information
    /// </summary>
    public interface IRouteDataRepository
    {
        #region Catalogue Population
        /// <summary>
        /// Setup catalogue of port information
        /// </summary>
        /// <param name="portCollection"></param>
        void PopulatePorts(List<Port> portCollection);

        /// <summary>
        /// Setup catalogue of Shipping Route Information
        /// </summary>
        /// <param name="shippingRouteCollection"></param>
        void PopulateShippingRoutes(List<ShippingRoute> shippingRouteCollection);

        #endregion Catalogue Population

        /// <summary>
        /// Retrieve list of port entities
        /// </summary>
        /// <returns>List of Port Entities</returns>
        List<Port> ListPorts();


        /// <summary>
        /// Retrieve list of all direct shippingRoutes originating from passed port code. 
        /// </summary>
        /// <param name="portCodeFrom">port code shipping from</param>
        /// <returns>List of ShippingRoute entities</returns>
        List<ShippingRoute> ListAllDirectShippingRoutesForPort(string portCodeFrom);

        /// <summary>
        /// Retrieve single direct shipping route for passed in port from and to.
        /// </summary>
        /// <param name="portCodeFrom"></param>
        /// <param name="portCodeTo"></param>
        /// <returns>Valid shipping route if one exists, else null</returns>
        ShippingRoute RetrieveDirectShippingRouteBetweenPorts(string portCodeFrom, string portCodeTo);


    }
}

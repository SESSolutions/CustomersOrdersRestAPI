using CustomersOrdersDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomersOrdersServices.Models
{
    public class CustomerAndOrders
    {
        public Customer Customer { get; set; }
        public IList<CustomerOrder> Orders { get; set; }
    }
}
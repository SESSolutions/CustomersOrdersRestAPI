using CustomersOrdersDataAccess;
using CustomersOrdersServices.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace CustomersOrdersServices.Controllers
{
        public interface ICustomersAndOrdersController
        {       
            Task<IEnumerable<Customer>> GetCustomersAsync();
            Task<IHttpActionResult> GetCustomerAndOrdersAsync(int id);
            Task<IHttpActionResult> PostCustomerAsync(Customer customer);
            Task<IHttpActionResult> PostOrderAsync(CustomerOrder order);
        }
        
        [RoutePrefix("api")]
        public class CustomersController : ApiController, ICustomersAndOrdersController
        {
            private IDataSourceEntities DataSourceEntities { get; set; }

            public CustomersController() { }

            public CustomersController(IDataSourceEntities dataSourceEntities)
            {
                this.DataSourceEntities = dataSourceEntities;
            }

            [HttpGet]
            [Route("customers")] // http://localhost:50200/api/customers
            public Task<IEnumerable<Customer>> GetCustomersAsync()
            {
                IEnumerable<Customer> Customers = GetCustomers();
                return Task.FromResult(Customers);
            }

            [HttpGet]
            [Route("customer/orders/{id}")] //http://localhost:50200/api/customer/orders{i} i = 1,2,3...
            public Task<IHttpActionResult> GetCustomerAndOrdersAsync(int id)
            {
                IHttpActionResult result = GetCustomerAndOrders(id);
                return Task.FromResult(result);
            }

            [HttpPost]
            [Route("post/customer")] //http://localhost:50200/api/post/customer
            public Task<IHttpActionResult> PostCustomerAsync(Customer newCustomer)
            {
                IHttpActionResult result = PostCustomer(newCustomer);
                return Task.FromResult(result);
            }

            [HttpPost]
            [Route("post/order")]  //http://localhost:50200/api/post/order
            public Task<IHttpActionResult> PostOrderAsync(CustomerOrder order)
            {
                IHttpActionResult result = PostOrder(order);
                return Task.FromResult(result);
            }



           // Private methods used in the task-based methods

            // Method for getting all the customers.
            private IEnumerable<Customer> GetCustomers()
            {
                using (var dataSourceEntities = DataSourceEntities)
                {
                    return dataSourceEntities.Customers.ToList();
                }
            }

            // Method for getting a customer and his orders.
            private IHttpActionResult GetCustomerAndOrders(int id)
            {
                Customer customer;
                IList<CustomerOrder> orders;

                using (var dataSourceEntities = DataSourceEntities)
                {
                    customer = dataSourceEntities.Customers.Where(p => p.id == id).SingleOrDefault();
                    orders = dataSourceEntities.CustomerOrders.Where(p => p.CustomerId == id).ToList();
                }
                CustomerAndOrders customerAndOrders = new CustomerAndOrders
                {
                    Customer = customer,
                    Orders = orders
                };
                if (customerAndOrders.Customer == null || customerAndOrders.Orders == null)
                {
                    return NotFound();
                }
                return Ok(customerAndOrders);
            }

            // Method for posting a customer.
            private IHttpActionResult PostCustomer(Customer newCustomer)
            {
                Customer customer;
                using (var dataSourceEntities = DataSourceEntities)
                {  // Checking whether a customer already exists. If it exists already post returns bad request.
                    customer = dataSourceEntities.Customers.Where(p => p.Email == newCustomer.Email).FirstOrDefault();
                    if (customer != null)
                    {
                        return BadRequest();
                    }
                    dataSourceEntities.Customers.Add(newCustomer);
                    dataSourceEntities.SaveChanges();
                    customer = dataSourceEntities.Customers.Where(p => p.Email == newCustomer.Email).FirstOrDefault(); // Look for it again after saving it
                }
                return Content(HttpStatusCode.Created, customer);
            }

            // Method for posting an order.
            private IHttpActionResult PostOrder(CustomerOrder newOrder)
            {
                CustomerOrder order;
                using (var dataSourceEntities = DataSourceEntities)
                {     // Checking to know whether the order already exist; it returns 'bad request' if it already exits.
                    order = dataSourceEntities.CustomerOrders
                     .Where(p => p.Price == newOrder.Price && p.CreatedDate == newOrder.CreatedDate && p.CustomerId == newOrder.CustomerId)
                     .FirstOrDefault();
                    if (order != null)
                    {
                        return BadRequest();
                    }
                    else
                    {  // Assumption: Every customer has an order; it returns 'bad request' if order is not linked with a exiting customer.
                        CustomerOrder custorder = dataSourceEntities.CustomerOrders.Where(p => p.CustomerId == newOrder.CustomerId).FirstOrDefault();
                        if (custorder != null)
                        {
                            dataSourceEntities.CustomerOrders.Add(newOrder);
                            dataSourceEntities.SaveChanges();
                            order = dataSourceEntities.CustomerOrders.Where(p => p.CreatedDate == newOrder.CreatedDate).FirstOrDefault(); // Look for it again after addition
                        }
                        else return BadRequest();
                    }
                }
                return Content(HttpStatusCode.Created, order);
            }

        }
    
}

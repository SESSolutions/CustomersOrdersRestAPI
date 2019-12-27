using CustomersOrdersDataAccess;
using CustomersOrdersServices.Controllers;
using CustomersOrdersServices.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;

namespace CustomersOrdersServices.Tests.Controllers
{
    [TestClass]
    public class UnitTestCustomersOrderController
    {

        [TestMethod]
        public void GetCustomers_ShouldReturnAllCustomers()
        {
            //Arrange
            var dataSourceEntities = new UnitTestDataSourceEntities();
            dataSourceEntities.Customers.Add(new Customer { id = 1, FirstName = "Sunday", LastName = "Eze", Email = "John@sss.com" });
            dataSourceEntities.Customers.Add(new Customer { id = 2, FirstName = "John", LastName = "Singn", Email = "sign@no.com" });
            dataSourceEntities.Customers.Add(new Customer { id = 3, FirstName = "Samson", LastName = "King", Email = "king@gg.com" });
            dataSourceEntities.Customers.Add(new Customer { id = 4, FirstName = "Olav", LastName = "Hundson", Email = "Olav@sss.com" });
            var controller = new CustomersController(dataSourceEntities);

            // Act
            List<Customer> customers = controller.GetCustomersAsync().Result.ToList();

            //Assert
            Assert.IsNotNull(customers);          // Should not be null
            Assert.AreEqual(4, customers.Count); // Should return all the 4 customers that are contained in the data sources
        }


        [TestMethod]
        public void GetCustomerAndOrders_ShouldReturnCustomerAndAllHisOrders()
        {
            // Arrange
            var dataSourceEntities = new UnitTestDataSourceEntities();
            dataSourceEntities.Customers.Add(new Customer { id = 1, FirstName = "Olav", LastName = "Hundson", Email = "Olav@sss.com" });
            dataSourceEntities.CustomerOrders.Add(new CustomerOrder { id = 1, Price = 1, CreatedDate = Convert.ToDateTime("5/6/2016"), CustomerId = 1 });
            dataSourceEntities.CustomerOrders.Add(new CustomerOrder { id = 2, Price = 2, CreatedDate = Convert.ToDateTime("5/7/2017"), CustomerId = 1 });
            dataSourceEntities.CustomerOrders.Add(new CustomerOrder { id = 3, Price = 3, CreatedDate = Convert.ToDateTime("5/8/2018"), CustomerId = 1 });
            dataSourceEntities.CustomerOrders.Add(new CustomerOrder { id = 4, Price = 4, CreatedDate = Convert.ToDateTime("5/9/2019"), CustomerId = 1 });
            var controller = new CustomersController(dataSourceEntities);

            // Act
            var actionResult = controller.GetCustomerAndOrdersAsync(1).Result;
            var contentResult = actionResult as OkNegotiatedContentResult<CustomerAndOrders>;
            var content = contentResult.Content;

            // Assert
            Assert.IsNotNull(content);    // Should not be null
            Assert.AreEqual(4, content.Orders.Count); // Should return all the 4 orders that belong to a customer
            Assert.AreEqual("Olav", content.Customer.FirstName); // The customer's name should match the returned name 
        }


        [TestMethod]
        public void GetCustomerAndOrders_ShouldReturnNotFoundForCustomerThatDoesNotExist()
        {
            // Arrange
            var dataSourceEntities = new UnitTestDataSourceEntities();
            dataSourceEntities.Customers.Add(new Customer { id = 1, FirstName = "Olav", LastName = "Hundson", Email = "Olav@sss.com" });
            dataSourceEntities.CustomerOrders.Add(new CustomerOrder { id = 1, Price = 1, CreatedDate = Convert.ToDateTime("5/6/2016"), CustomerId = 1 });
            dataSourceEntities.CustomerOrders.Add(new CustomerOrder { id = 2, Price = 2, CreatedDate = Convert.ToDateTime("5/7/2017"), CustomerId = 1 });
            dataSourceEntities.CustomerOrders.Add(new CustomerOrder { id = 3, Price = 3, CreatedDate = Convert.ToDateTime("5/8/2018"), CustomerId = 1 });
            dataSourceEntities.CustomerOrders.Add(new CustomerOrder { id = 4, Price = 4, CreatedDate = Convert.ToDateTime("5/9/2019"), CustomerId = 1 });
            var controller = new CustomersController(dataSourceEntities);

            // Act
            var actionResult = controller.GetCustomerAndOrdersAsync(4).Result;

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult)); // Should return 'not found' for a customer that does not exist
        }


        [TestMethod]
        public void PostCustomer_ShouldReturnThePostedCustomerOnSuccess()
        {
            // Arrange
            var dataSourceEntities = new UnitTestDataSourceEntities();
            var controller = new CustomersController(dataSourceEntities);

            // Act
            var task_actionResult = controller.PostCustomerAsync(new Customer { id = 4, FirstName = "Ya", LastName = "Is", Email = "Is@g,ail.com" });
            var actionResult = task_actionResult.Result;
            var content = actionResult as NegotiatedContentResult<Customer>;

            // Act
            Assert.IsNotNull(content);    // Should post and return a posted content
            Assert.AreEqual("Ya", content.Content.FirstName); // The posted customer's field values should match with those of a returned values
        }


        [TestMethod]
        public void PostCustomer_ShouldReturnBadRequestWhenPostedCustomerAlreadyExists()
        {
            // Arrange
            var dataSourceEntities = new UnitTestDataSourceEntities();
            dataSourceEntities.Customers.Add(new Customer { id = 4, FirstName = "Ya", LastName = "Is", Email = "Is@g,ail.com" });
            var controller = new CustomersController(dataSourceEntities);

            //Act
            var task_actionResult = controller.PostCustomerAsync(new Customer { id = 4, FirstName = "Ya", LastName = "Is", Email = "Is@g,ail.com" });
            var result = task_actionResult.Result;

            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestResult)); // Returned bad request for a customer that already exists
        }



        [TestMethod]
        public void PostOrder_ShouldReturnThePostedOrderOnSuccess()
        {
            //Arrange
            var dataSourceEntities = new UnitTestDataSourceEntities();
            dataSourceEntities.CustomerOrders.Add(new CustomerOrder { id = 1, Price = 100, CreatedDate = Convert.ToDateTime("10/7/2017"), CustomerId = 2 });
            var controller = new CustomersController(dataSourceEntities);

            //Arrange
            var task_actionResult = controller.PostOrderAsync(new CustomerOrder { id = 2, Price = 200, CreatedDate = Convert.ToDateTime("5/6/2016"), CustomerId = 2 });
            var actionResult = task_actionResult.Result;
            var content = actionResult as NegotiatedContentResult<CustomerOrder>;

            //Assert
            Assert.IsNotNull(content);  // Should not be null for a successful post
            Assert.AreEqual(Convert.ToDateTime("5/6/2016"), content.Content.CreatedDate); // The created date of a returned content should match with one posted
            Assert.AreEqual(200, content.Content.Price); // The price of returned content should match with one posted
        }


        [TestMethod]
        public void PostOrder_ShouldReturnBadRequestForPostedOrderThatExistsAlready()
        {
            // Arrange
            var dataSourceEntities = new UnitTestDataSourceEntities();
            dataSourceEntities.CustomerOrders.Add(new CustomerOrder { id = 1, Price = 200, CreatedDate = Convert.ToDateTime("10/7/2017"), CustomerId = 2 });
            var controller = new CustomersController(dataSourceEntities);

            // Act
            var task_actionResult = controller.PostOrderAsync(new CustomerOrder { id = 1, Price = 200, CreatedDate = Convert.ToDateTime("10/7/2017"), CustomerId = 2 });
            var result = task_actionResult.Result;

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestResult)); // Returned bad request because order already exists.
        }

        [TestMethod]
        public void PostOrder_ShouldReturnBadRequestForCustomerThatDoesNotExist()
        {
            // Arrange
            var dataSourceEntities = new UnitTestDataSourceEntities();
            dataSourceEntities.CustomerOrders.Add(new CustomerOrder { id = 1, Price = 200, CreatedDate = Convert.ToDateTime("10/7/2017"), CustomerId = 2 });
            var controller = new CustomersController(dataSourceEntities);

            // Act
            var task_actionResult = controller.PostOrderAsync(new CustomerOrder { id = 1, Price = 200, CreatedDate = Convert.ToDateTime("10/7/2017"), CustomerId = 5 });

            // Assert
            Assert.IsInstanceOfType(task_actionResult.Result, typeof(BadRequestResult)); // Cannot post order for a customer that does not exist
        }
    }
}

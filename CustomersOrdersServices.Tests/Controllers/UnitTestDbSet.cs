using CustomersOrdersDataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;


namespace CustomersOrdersServices.Tests.Controllers
{
    public class TestCustomerDbSet : UnitTestDbSet<Customer>
    {
        public override Customer Find(params object[] keyValues)
        {
            return this.SingleOrDefault(customer => customer.id == (int)keyValues.Single());
        }
    }


    public class TestOrderDbSet : UnitTestDbSet<CustomerOrder>
    {
        public override CustomerOrder Find(params object[] keyValues)
        {
            return this.SingleOrDefault(order => order.id == (int)keyValues.Single());
        }
    }


    public class UnitTestDataSourceEntities : IDataSourceEntities
    {
        public UnitTestDataSourceEntities()
        {
            Customers = new TestCustomerDbSet();
            CustomerOrders = new TestOrderDbSet();
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerOrder> CustomerOrders { get; set; }

        public int SaveChanges() { return 0; }

        public void Dispose() { }
    }


    public class UnitTestDbSet<T> : DbSet<T>, IQueryable, IEnumerable<T>
            where T : class
    {
        ObservableCollection<T> _data;
        IQueryable _query;

        public UnitTestDbSet()
        {
            _data = new ObservableCollection<T>();
            _query = _data.AsQueryable();
        }

        public override T Add(T item)
        {
            _data.Add(item);
            return item;
        }

        public override T Remove(T item)
        {
            _data.Remove(item);
            return item;
        }

        public override T Attach(T item)
        {
            _data.Add(item);
            return item;
        }

        public override T Create()
        {
            return Activator.CreateInstance<T>();
        }

        public override TDerivedEntity Create<TDerivedEntity>()
        {
            return Activator.CreateInstance<TDerivedEntity>();
        }

        public override ObservableCollection<T> Local
        {
            get { return new ObservableCollection<T>(_data); }
        }

        Type IQueryable.ElementType
        {
            get { return _query.ElementType; }
        }

        System.Linq.Expressions.Expression IQueryable.Expression
        {
            get { return _query.Expression; }
        }

        IQueryProvider IQueryable.Provider
        {
            get { return _query.Provider; }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _data.GetEnumerator();
        }
    }
}

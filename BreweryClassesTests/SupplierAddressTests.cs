using System.Collections.Generic;
using System.Linq;
using System;

using NUnit.Framework;
using BreweryClasses.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NUnit.Framework.Internal;

namespace BreweryClassesTests

{
    [TestFixture]
    public class SupplierAddressTests
    {
        BitsContext dbContext;
        IDbContextTransaction transaction;
        SupplierAddress? s;
        List<SupplierAddress> supplierAddresses;

        [SetUp]
        public void Setup() 
        { 
            dbContext = new BitsContext();
            transaction = dbContext.Database.BeginTransaction();
        }

        [TearDown]
        public void Teardown() 
        { 
            transaction.Rollback();
            transaction.Dispose();
            dbContext.Dispose();
        }

        [Test]
        public void GetAllTest()
        {
            supplierAddresses = dbContext.SupplierAddresses.OrderBy(s => s.SupplierId).ToList();
            Assert.That(supplierAddresses.Count, Is.EqualTo(12));
            Assert.That(supplierAddresses[0].SupplierId, Is.EqualTo(1));
            PrintAll(supplierAddresses);
        }

        [Test]
        public void GetByPrimaryKeyTest()
        {
            s = dbContext.SupplierAddresses.Find(4, 5, 1);
            Assert.IsNotNull(s);
            Assert.That(s.SupplierId, Is.EqualTo(4));
            Assert.That(s.AddressTypeId, Is.EqualTo(1));
            Assert.That(s.AddressId, Is.EqualTo(5));
            Console.WriteLine(s);

            s = dbContext.SupplierAddresses.Find(4, 4, 2);
            Assert.IsNotNull(s);
            Assert.That(s.SupplierId, Is.EqualTo(4));
            Assert.That(s.AddressTypeId, Is.EqualTo(2));
            Assert.That(s.AddressId, Is.EqualTo(4));
            Console.WriteLine(s);
        }

        [Test]
        public void GetUsingWhere()
        {
            // get a list of all of the supplier addresses with address type id = 2
            supplierAddresses = dbContext.SupplierAddresses.Where(s => s.AddressTypeId.Equals(2)).OrderBy(s => s.SupplierId).ToList();
            Assert.That(supplierAddresses.Count, Is.EqualTo(6));
            Assert.That(supplierAddresses[5].AddressId, Is.EqualTo(7)); 
            PrintAll(supplierAddresses);
        }

        /*[Test]
        public void GetWith???Test()
        {
        }*/

        [Test]
        public void GetWithJoinTest()
        {   
           // list of objects that includes supplier id, address id, address type id, name, phone, email
                
            var supplierAddresses = dbContext.SupplierAddresses.Join(
               dbContext.Suppliers,
               sa => sa.SupplierId,
               s => s.SupplierId,
               (sa, s) => new { sa.AddressTypeId, sa.SupplierId, sa.AddressId, s.Name, s.Phone, s.Website}).OrderBy(r => r.AddressTypeId).ToList();
            Assert.That(supplierAddresses.Count, Is.EqualTo(12));
            // print objects
            foreach (var s in supplierAddresses)
            {
                Console.WriteLine(s);
            }
        }
        /*supplierId, AddressId, AddressTypeId */

        [Test]
        public void CreateTest()
        {
            //Supplier
            Supplier? supplier;
            supplier = new Supplier();
            supplier.Name = "Test Supplier1";
            dbContext.Suppliers.Add(supplier);
            dbContext.SaveChanges();
            
            //Address
            Address? address;
            address = new Address();
            address.StreetLine1 = "Street line 1 TEST";
            address.City = "Test City";
            address.State = "WA";
            address.Country = "USA";
            dbContext.Addresses.Add(address);
            dbContext.SaveChanges();

            //addresstype
            AddressType? addressType;
            addressType = new AddressType();
            addressType.AddressTypeId = 4;
            addressType.Name = "type test";
            dbContext.AddressTypes.Add(addressType);
            dbContext.SaveChanges();

            //create with above values
            s = new SupplierAddress();
            s.SupplierId = supplier.SupplierId;
            s.AddressId = address.AddressId;
            s.AddressTypeId = addressType.AddressTypeId;
            dbContext.SupplierAddresses.Add(s);
            dbContext.SaveChanges();
            Assert.That(s.SupplierId, Is.EqualTo(supplier.SupplierId));
            Assert.That(s.AddressId, Is.EqualTo(address.AddressId));
            Assert.That(s.AddressTypeId, Is.EqualTo(4));
            Assert.IsNotNull(dbContext.SupplierAddresses.Find(s.SupplierId, s.AddressId, s.AddressTypeId));
        }

        [Test]
        public void UpdateTest()
        {
            s = dbContext.SupplierAddresses.Find(4, 5, 1);

            //remove old values
            dbContext.SupplierAddresses.Remove(s);
            dbContext.SaveChanges();

            //Address
            Address? address;
            address = new Address();
            address.StreetLine1 = "Street line 1 TEST";
            address.City = "Test City";
            address.State = "WA";
            address.Country = "USA";
            dbContext.Addresses.Add(address);
            dbContext.SaveChanges();

            //addresstype
            AddressType? addressType;
            addressType = new AddressType();
            addressType.AddressTypeId = 4;
            addressType.Name = "type test";
            dbContext.AddressTypes.Add(addressType);
            dbContext.SaveChanges();

            //update with new values above
            s.AddressId = address.AddressId;
            s.AddressTypeId = addressType.AddressTypeId; 
            dbContext.SupplierAddresses.Add(s);
            dbContext.SaveChanges();
            Assert.That(s.AddressId, Is.EqualTo(address.AddressId));
            Assert.That(s.AddressTypeId, Is.EqualTo(4));
            Assert.IsNotNull(dbContext.SupplierAddresses.Find(s.SupplierId, s.AddressId, s.AddressTypeId));
        }

        [Test]
        public void DeleteTest()
        {
            //Supplier
            Supplier? supplier;
            supplier = new Supplier();
            supplier.Name = "Test Supplier1";
            dbContext.Suppliers.Add(supplier);
            dbContext.SaveChanges();

            //Address
            Address? address;
            address = new Address();
            address.StreetLine1 = "Street line 1 TEST";
            address.City = "Test City";
            address.State = "WA";
            address.Country = "USA";
            dbContext.Addresses.Add(address);
            dbContext.SaveChanges();

            //addresstype
            AddressType? addressType;
            addressType = new AddressType();
            addressType.AddressTypeId = 4;
            addressType.Name = "type test";
            dbContext.AddressTypes.Add(addressType);
            dbContext.SaveChanges();

            //create with above values
            s = new SupplierAddress();
            s.SupplierId = supplier.SupplierId;
            s.AddressId = address.AddressId;
            s.AddressTypeId = addressType.AddressTypeId;
            dbContext.SupplierAddresses.Add(s);
            dbContext.SaveChanges();
            Assert.That(s.SupplierId, Is.EqualTo(supplier.SupplierId));
            Assert.That(s.AddressId, Is.EqualTo(address.AddressId));
            Assert.That(s.AddressTypeId, Is.EqualTo(4));
            Assert.IsNotNull(dbContext.SupplierAddresses.Find(s.SupplierId, s.AddressId, s.AddressTypeId));

            //delete
            s = dbContext.SupplierAddresses.Find(s.SupplierId, s.AddressId, s.AddressTypeId);
            dbContext.SupplierAddresses.Remove(s);
            dbContext.SaveChanges();
            Assert.IsNull(dbContext.SupplierAddresses.Find(s.SupplierId, s.AddressId, s.AddressTypeId));
        }

        //I'm sure there are more simple ways to do the CrUD tests but clearly I could not figure them out

        /*AddressId, StreetLine1, StreetLine2, City, State, Zipcode , Country */

        public void PrintAll(List<SupplierAddress> supplierAddresses)
        {
            foreach (SupplierAddress s in supplierAddresses)
            {
                Console.WriteLine(s);
            }
        }
    }
}
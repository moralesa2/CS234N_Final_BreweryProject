using System.Collections.Generic;
using System.Linq;
using System;

using NUnit.Framework;
using BreweryClasses.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BreweryClassesTests

{
    [TestFixture]
    public class AddressTests
    {
        BitsContext dbContext;
        IDbContextTransaction transaction;
        Address? a;
        List<Address> addresses;

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
            addresses = dbContext.Addresses.OrderBy(a => a.AddressId).ToList();
            Assert.That(addresses.Count, Is.EqualTo(7));
            Assert.That(addresses[0].StreetLine1, Is.EqualTo("800 West 1st Ave"));
            PrintAll(addresses);
        }

        [Test]
        public void GetByPrimaryKeyTest()
        {
            a = dbContext.Addresses.Find(4);
            Assert.IsNotNull(a);
            Assert.That(a.StreetLine1, Is.EqualTo("1 West Washington Avenue"));
            Assert.That(a.AddressId, Is.EqualTo(4));
            Console.WriteLine(a);
        }

        [Test]
        public void GetUsingWhere()
        {
            // get a list of all of the Washingtion addresses
            addresses = dbContext.Addresses.Where(a => a.State.StartsWith("WA")).OrderBy(a => a.AddressId).ToList();
            Assert.That(addresses.Count, Is.EqualTo(2));
            Assert.That(addresses[0].StreetLine1, Is.EqualTo("2501 Kotobuki Way")); 
            PrintAll(addresses);
        }

        [Test]
        public void GetWithSupplierAddressesTest()
        {
            a = dbContext.Addresses.Include("SupplierAddresses").Where(a => a.AddressId == 1).SingleOrDefault();
            Assert.IsNotNull(a);
            Assert.That(a.StreetLine1, Is.EqualTo("800 West 1st Ave"));
            Assert.That(a.SupplierAddresses.Count, Is.EqualTo(2));
            Console.WriteLine(a);
        }

        [Test]
        public void GetWithJoinTest()
        {   
                // list of objects that includes addressId, streetLine1, supplier id, addres type id
                
            var addresses = dbContext.Addresses.Join(
               dbContext.SupplierAddresses,
               a => a.AddressId,
               sa => sa.AddressId,
               (a, sa) => new { sa.SupplierId, a.AddressId, a.StreetLine1, sa.AddressTypeId}).OrderBy(r => r.SupplierId).ToList();
            Assert.That(addresses.Count, Is.EqualTo(12));
            // print objects
            foreach (var a in addresses)
            {
                Console.WriteLine(a);
            }
        }

        [Test]
        public void CreateTest()
        {
            a = new Address();
            a.StreetLine1 = "Street line 1 TEST";
            a.StreetLine2 = "Street line 2 TeST";
            a.City = "Test City";
            a.State = "WA";
            a.Zipcode = "10001";
            a.Country = "USA";
            dbContext.Addresses.Add(a);
            dbContext.SaveChanges();
            Assert.IsNotNull(dbContext.Addresses.Find(a.AddressId));
        }

        [Test]
        public void UpdateTest()
        {
            a = new Address();
            a.StreetLine1 = "Street line 1 TEST2";
            string oldStreet1 = a.StreetLine1;
            a.StreetLine2 = "Street line 2 TEST2";
            a.City = "Test City3";
            a.State = "WA";
            a.Zipcode = "20002";
            a.Country = "USA";
            dbContext.Addresses.Add(a);
            dbContext.SaveChanges();
            Assert.IsNotNull(dbContext.Addresses.Find(a.AddressId));

            a = dbContext.Addresses.Find(a.AddressId);
            a.StreetLine1 = "Street line 1 TESTUPDATE";
            a.StreetLine2 = "Street line 2 TESTUPDATE";
            a.City = "Test CityUD";
            a.State = "WA";
            a.Zipcode = "30003";
            a.Country = "USA";
            dbContext.Addresses.Update(a);
            dbContext.SaveChanges();
            dbContext.Addresses.Find(a.AddressId);
            Assert.That(a.StreetLine1, Is.Not.EqualTo(oldStreet1));
            Assert.That(a.StreetLine1, Is.EqualTo("Street line 1 TESTUPDATE"));
        }

        [Test]
        public void DeleteTest()
        {
            a = new Address();
            a.StreetLine1 = "Street line 1 TESTDELETE";
            a.StreetLine2 = "Street line 2 TESTDELETE";
            a.City = "Test CityDEL";
            a.State = "WA";
            a.Zipcode = "40004";
            a.Country = "USA";
            dbContext.Addresses.Add(a);
            dbContext.SaveChanges();
            Assert.IsNotNull(dbContext.Addresses.Find(a.AddressId));

            a = dbContext.Addresses.Find(a.AddressId);
            Console.WriteLine(a);
            dbContext.Addresses.Remove(a);
            dbContext.SaveChanges();
            Assert.IsNull(dbContext.Addresses.Find(a.AddressId));
        }

        /*AddressId, StreetLine1, StreetLine2, City, State, Zipcode , Country */

        public void PrintAll(List<Address> addresses)
        {
            foreach (Address a in addresses)
            {
                Console.WriteLine(a);
            }
        }
    }
}
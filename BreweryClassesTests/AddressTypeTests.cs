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
    public class AddressTypeTests
    {
        BitsContext dbContext;
        IDbContextTransaction transaction;
        AddressType? a;
        List<AddressType> addressTypes;

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
            addressTypes = dbContext.AddressTypes.OrderBy(a => a.AddressTypeId).ToList();
            Assert.That(addressTypes.Count, Is.EqualTo(3));
            Assert.That(addressTypes[0].Name, Is.EqualTo("billing"));
            PrintAll(addressTypes);
        }

        [Test]
        public void GetByPrimaryKeyTest()
        {
            a = dbContext.AddressTypes.Find(3);
            Assert.IsNotNull(a);
            Assert.That(a.AddressTypeId, Is.EqualTo(3));
            Assert.That(a.Name, Is.EqualTo("shipping"));
            Console.WriteLine(a);
        }

        [Test]
        public void GetUsingWhere()
        {
            // get a list of all of the billing AddressTypes
            addressTypes = dbContext.AddressTypes.Where(a => a.Name.StartsWith("bi")).OrderBy(a => a.AddressTypeId).ToList();
            Assert.That(addressTypes.Count, Is.EqualTo(1));
            Assert.That(addressTypes[0].Name, Is.EqualTo("billing")); 
            PrintAll(addressTypes);
        }

        [Test]
        public void GetWithSupplierAddressesTest()
        {
            a = dbContext.AddressTypes.Include("SupplierAddresses").Where(a => a.AddressTypeId == 2).SingleOrDefault();
            Assert.IsNotNull(a);
            Assert.That(a.Name, Is.EqualTo("mailing"));
            Assert.That(a.SupplierAddresses.Count, Is.EqualTo(6));
            Console.WriteLine(a);
        }

        [Test]
        public void GetWithJoinTest()
        {   
                // list of objects that includes addressTypeId, Name, supplier id, addressId
                
            var addressTypes = dbContext.AddressTypes.Join(
               dbContext.SupplierAddresses,
               a => a.AddressTypeId,
               sa => sa.AddressTypeId,
               (a, sa) => new { a.AddressTypeId, a.Name, sa.SupplierId, sa.AddressId}).OrderBy(r => r.SupplierId).ToList();
            Assert.That(addressTypes.Count, Is.EqualTo(12));
            // print objects
            foreach (var a in addressTypes)
            {
                Console.WriteLine(a);
            }
        }

        [Test]
        public void CreateTest()
        {
            a = new AddressType();
            a.AddressTypeId = 4;
            a.Name = "type test";
            dbContext.AddressTypes.Add(a);
            dbContext.SaveChanges();
            Assert.IsNotNull(dbContext.AddressTypes.Find(a.AddressTypeId));
            Assert.That(a.Name, Is.EqualTo("type test"));
            Assert.That(a.AddressTypeId, Is.EqualTo(4));    
        }

        [Test]
        public void UpdateTest()
        {
            a = new AddressType();
            a.AddressTypeId = 4;
            a.Name = "test type UD";
            string oldName = a.Name; 
            dbContext.AddressTypes.Add(a);
            dbContext.SaveChanges();
            Assert.IsNotNull(dbContext.AddressTypes.Find(4));

            //update
            a = dbContext.AddressTypes.Find(4);
            a.Name = "test type UD2";
            dbContext.AddressTypes.Update(a);
            dbContext.SaveChanges();
            dbContext.AddressTypes.Find(a.AddressTypeId);
            Assert.That(a.Name, Is.Not.EqualTo(oldName));
            Assert.That(a.AddressTypeId, Is.EqualTo(4));
        }

        [Test]
        public void DeleteTest()
        {
            a = new AddressType();
            a.AddressTypeId = 4;
            a.Name = "test typeDEL";
            dbContext.AddressTypes.Add(a);
            dbContext.SaveChanges();
            Assert.IsNotNull(dbContext.AddressTypes.Find(4));

            //delete
            a = dbContext.AddressTypes.Find(a.AddressTypeId);
            Console.WriteLine(a);
            dbContext.AddressTypes.Remove(a);
            dbContext.SaveChanges();
            Assert.IsNull(dbContext.AddressTypes.Find(4));
        }

        public void PrintAll(List<AddressType> addressTypes)
        {
            foreach (AddressType a in addressTypes)
            {
                Console.WriteLine(a);
            }
        }
    }
}
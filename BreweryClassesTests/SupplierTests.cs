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
    public class SupplierTests
    {
        BitsContext dbContext;
        IDbContextTransaction transaction;
        Supplier? s;
        List<Supplier> suppliers;

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
            suppliers = dbContext.Suppliers.OrderBy(s => s.SupplierId).ToList();
            Assert.That(suppliers.Count, Is.EqualTo(6));
            Assert.That(suppliers[0].Name, Is.EqualTo("BSG Craft Brewing"));
            PrintAll(suppliers);
        }

        [Test]
        public void GetByPrimaryKeyTest()
        {
            s = dbContext.Suppliers.Find(1);
            Assert.IsNotNull(s);
            Assert.That(s.Name, Is.EqualTo("BSG Craft Brewing"));
            Assert.That(s.Email, Is.EqualTo("sales@bsgcraft.com"));
            Assert.That(s.SupplierId, Is.EqualTo(1));
            Console.WriteLine(s);
        }

        [Test]
        public void GetUsingWhere()
        {
            // get a list of all of the suppliers whose phone # starts with 800
            suppliers = dbContext.Suppliers.Where(s => s.Phone.StartsWith("800")).OrderBy(s => s.Name).ToList();
            Assert.That(suppliers.Count, Is.EqualTo(2));
            Assert.That(suppliers[0].Name, Is.EqualTo("Country Malt Group")); 
            PrintAll(suppliers);
        }

        [Test]
        public void GetWithIngredientInventoryAdditionsTest()
        {
            s = dbContext.Suppliers.Include("IngredientInventoryAdditions").Where(s => s.SupplierId == 2).SingleOrDefault();
            Assert.IsNotNull(s);
            Assert.That(s.Name, Is.EqualTo("Malteurop Malting Company"));
            Assert.That(s.IngredientInventoryAdditions.Count, Is.EqualTo(12));
            Console.WriteLine(s);
        }

        [Test]
        public void GetWithSupplierAddressesTest()
        {
            s = dbContext.Suppliers.Include("SupplierAddresses").Where(s => s.SupplierId == 5).SingleOrDefault();
            Assert.IsNotNull(s);
            Assert.That(s.Name, Is.EqualTo("John I. Haas, Inc."));
            Assert.That(s.SupplierAddresses.Count, Is.EqualTo(2));
            Console.WriteLine(s);
        }

        [Test]
        public void GetWithJoinTest()
        {   
           // list of objects that includes supplier id, name, phone, website, ingredient id, order date 
                
            var suppliers = dbContext.Suppliers.Join(
               dbContext.IngredientInventoryAdditions,
               s => s.SupplierId,
               i => i.SupplierId,
               (s, i) => new { s.SupplierId, s.Name, s.Phone, s.Website, i.IngredientId, i.OrderDate}).OrderBy(r => r.SupplierId).ToList();
            Assert.That(suppliers.Count, Is.EqualTo(35));
            // print objects
            foreach (var s in suppliers)
            {
                Console.WriteLine(s);
            }
        }
        /* SupplierId, Name, Phone, Email, Website, ContactFirstName, ContactLastName, ContactPhone, ContactEmail, Note */
        
        [Test]
        public void CreateTest()
        {
            s = new Supplier();
            s.Name = "Test Supplier1";
            s.Phone = "1234567890";
            s.Email = "mail@email.com";
            s.Website = "testsupplier1.com";
            s.ContactFirstName = "John";
            s.ContactLastName = "Doe";
            s.ContactPhone = "2345678901";
            s.ContactEmail = "johndoe@email.com";
            s.Note = "test note";
            dbContext.Suppliers.Add(s);
            dbContext.SaveChanges();
            Assert.IsNotNull(dbContext.Suppliers.Find(s.SupplierId));
        }

        [Test]
        public void UpdateTest()
        {
            s = new Supplier();
            s.Name = "Test Supplier1";
            string oldName = s.Name;
            s.Phone = "1234567890";
            s.Email = "mail@email.com";
            s.Website = "testsupplier1.com";
            s.ContactFirstName = "John";
            string oldFirstName = s.ContactFirstName;
            s.ContactLastName = "Doe";
            s.ContactPhone = "2345678901";
            s.ContactEmail = "johndoe@email.com";
            s.Note = "test note";
            string oldNote = s.Note;
            dbContext.Suppliers.Add(s);
            dbContext.SaveChanges();
            Assert.IsNotNull(dbContext.Suppliers.Find(s.SupplierId));

            s = dbContext.Suppliers.Find(s.SupplierId);
            s.Name = "Test SupplierUD";
            s.Phone = "3456789012";
            s.Email = "mail@email.com";
            s.Website = "testsupplier2.com";
            s.ContactFirstName = "Jane";
            s.ContactLastName = "Doe";
            s.ContactPhone = "4567890123";
            s.ContactEmail = "johndoe@email.com";
            s.Note = "testing update note";
            dbContext.Suppliers.Update(s);
            dbContext.SaveChanges();
            dbContext.Suppliers.Find(s.SupplierId);
            Assert.That(s.Name, Is.Not.EqualTo(oldName));
            Assert.That(s.ContactFirstName, Is.Not.EqualTo(oldFirstName));
            Assert.That(s.Note, Is.Not.EqualTo(oldNote));
            Assert.That(s.Name, Is.EqualTo("Test SupplierUD"));
            Assert.That(s.Phone, Is.EqualTo("3456789012"));
        }

        [Test]
        public void DeleteTest()
        {
            s = new Supplier();
            s.Name = "Test Supplier1";
            s.Phone = "1234567890";
            s.Email = "mail@email.com";
            s.Website = "testsupplier1.com";
            s.ContactFirstName = "John";
            s.ContactLastName = "Doe";
            s.ContactPhone = "2345678901";
            s.ContactEmail = "johndoe@email.com";
            s.Note = "test note";
            dbContext.Suppliers.Add(s);
            dbContext.SaveChanges();
            Assert.IsNotNull(dbContext.Suppliers.Find(s.SupplierId));

            s = dbContext.Suppliers.Find(s.SupplierId);
            Console.WriteLine(s);
            dbContext.Suppliers.Remove(s);
            dbContext.SaveChanges();
            Assert.IsNull(dbContext.Suppliers.Find(s.SupplierId));
        }

        public void PrintAll(List<Supplier> suppliers)
        {
            foreach (Supplier s in suppliers)
            {
                Console.WriteLine(s);
            }
        }
    }
}
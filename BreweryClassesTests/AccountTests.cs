using System.Collections.Generic;
using System.Linq;
using System;

using NUnit.Framework;
using BreweryClasses.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NUnit.Framework.Internal;
using System.Security.Cryptography;

namespace BreweryClassesTests

{
    [TestFixture]
    public class AccountTests
    {
        BitsContext dbContext;
        IDbContextTransaction transaction;
        Account? a;
        List<Account> accounts;

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
            Account a1 = new Account();
            Account a2 = new Account();
            dbContext.Accounts.Add(a1);
            dbContext.Accounts.Add(a2);
            dbContext.SaveChanges();

            accounts = dbContext.Accounts.OrderBy(a => a.AccountId).ToList();
            Assert.That(a1.Name, Is.Null);
            Assert.That(accounts.Count, Is.EqualTo(2));
            PrintAll(accounts);
        }

        [Test]
        public void GetByPrimaryKeyTest()
        {
            a = new Account();
            dbContext.Accounts.Add(a);
            dbContext.SaveChanges();

            a = dbContext.Accounts.Find(a.AccountId);
            int iD = a.AccountId;
            Assert.IsNotNull(a);
            Assert.That(a.Name, Is.Null);
            Assert.That(a.Phone, Is.Null);
            Assert.That(a.AccountId, Is.EqualTo(iD));
            Console.WriteLine(a);
        }

        /*AccountId, Name, Address, City, State, Zipcode, Phone, ContactName, SalesPersonName*/

        [Test]
        public void GetWithInventoryTransactionsTest()
        {
            a = new Account();
            dbContext.Accounts.Add(a);
            dbContext.SaveChanges();
            int iD = a.AccountId;

            a = dbContext.Accounts.Include("InventoryTransactions").Where(a => a.AccountId == iD).SingleOrDefault();
            Assert.IsNotNull(a);
            Assert.That(a.Name, Is.Null);
            Assert.That(a.InventoryTransactions.Count, Is.EqualTo(0));
            Console.WriteLine(a);
        }

        [Test]
        public void CreateTest()
        {
            a = new Account();
            a.Name = "Doe, Jane";
            a.Address = "Test address 1234";
            a.City = "TestCity";
            a.State = "OR";
            a.Zipcode = "1234";
            a.Phone = "1234567890";
            a.ContactName = "Jane contact";
            a.SalesPersonName = "Jane sales";
            dbContext.Accounts.Add(a);
            dbContext.SaveChanges();
            Assert.IsNotNull(dbContext.Accounts.Find(a.AccountId));
        }

        [Test]
        public void UpdateTest()
        {
            a = new Account();
            dbContext.Accounts.Add(a);
            dbContext.SaveChanges();
            Assert.IsNotNull(dbContext.Accounts.Find(a.AccountId));

            a = dbContext.Accounts.Find(a.AccountId);
            a.Name = "Doe, Jane";
            a.Address = "Test address 1234";
            a.City = "TestCity";
            a.State = "OR";
            a.Zipcode = "1234";
            a.Phone = "1234567890";
            a.ContactName = "Jane contact";
            a.SalesPersonName = "Jane sales";
            dbContext.Accounts.Update(a);
            dbContext.SaveChanges();
            dbContext.Suppliers.Find(a.AccountId);
            Assert.That(a.Name, Is.Not.Null);
            Assert.That(a.Address, Is.Not.Null);
            Assert.That(a.City, Is.Not.Null);
            Assert.That(a.Name, Is.EqualTo("Doe, Jane"));
            Assert.That(a.Phone, Is.EqualTo("1234567890"));
        }
        
        [Test]
        public void DeleteTest()
        {
            a = new Account();
            dbContext.Accounts.Add(a);
            dbContext.SaveChanges();
            Assert.IsNotNull(dbContext.Accounts.Find(a.AccountId));

            a = dbContext.Accounts.Find(a.AccountId);
            dbContext.Accounts.Remove(a);
            dbContext.SaveChanges();
            Assert.IsNull(dbContext.Accounts.Find(a.AccountId));
        }

        public void PrintAll(List<Account> accounts)
        {
            foreach (Account a in accounts)
            {
                Console.WriteLine(a);
            }
        }
    }
}
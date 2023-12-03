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
    public class IngedientInventoryAdditionTests
    {
        BitsContext dbContext;
        IDbContextTransaction transaction;
        IngredientInventoryAddition? iia;
        List<IngredientInventoryAddition> ingredientInventoryAdditions;

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
            ingredientInventoryAdditions = dbContext.IngredientInventoryAdditions.OrderBy(iia => iia.IngredientInventoryAdditionId).ToList();
            Assert.That(ingredientInventoryAdditions.Count, Is.EqualTo(35));
            Assert.That(ingredientInventoryAdditions[0].SupplierId, Is.EqualTo(2));
            PrintAll(ingredientInventoryAdditions);
        }

        [Test]
        public void GetByPrimaryKeyTest()
        {
            iia = dbContext.IngredientInventoryAdditions.Find(13);
            Assert.IsNotNull(iia);
            Assert.That(iia.IngredientId, Is.EqualTo(940));
            Assert.That(iia.SupplierId, Is.EqualTo(4));
            Assert.That(iia.Quantity, Is.EqualTo(0.056699));
            Console.WriteLine(iia);
        }

        [Test]
        public void GetUsingWhere()
        {
            // get a list of all of the ingredient inventory additions to supplier with supplierid 4
            ingredientInventoryAdditions = dbContext.IngredientInventoryAdditions.Where(iia => iia.SupplierId.Equals(4)).OrderBy(iia => iia.IngredientInventoryAdditionId).ToList();
            Assert.That(ingredientInventoryAdditions.Count, Is.EqualTo(11));
            Assert.That(ingredientInventoryAdditions[0].IngredientInventoryAdditionId, Is.EqualTo(4)); 
            PrintAll(ingredientInventoryAdditions);
        }

        [Test]
        public void GetWithJoinTest()
        {   
           // list of objects that includes iia ID, ingredient id, supplier id, name (ingredient) 
                
            var ingredientInventoryAdditions = dbContext.IngredientInventoryAdditions.Join(
               dbContext.Ingredients,
               iia => iia.IngredientId,
               ingr => ingr.IngredientId,
               (iia, ingr) => new { iia.IngredientId, iia.IngredientInventoryAdditionId, iia.SupplierId, ingr.Name}).OrderBy(r => r.IngredientId).ToList();
            Assert.That(ingredientInventoryAdditions.Count, Is.EqualTo(35));
            // print objects
            foreach (var i in ingredientInventoryAdditions)
            {
                Console.WriteLine(i);
            }
        }

        /*IngredientInventoryAdditionId, IngredientId, OrderDate, EstimatedDeliveryDate, TransactionDate, SupplierId, Quantity, QuantityRemaining, UnitCost*/


        [Test]
        public void CreateTest()
        {
            Ingredient? ingredient;
            Supplier? supplier;

            ingredient = dbContext.Ingredients.Find(914);
            supplier = dbContext.Suppliers.Find(4);

            //create with above values
            iia = new IngredientInventoryAddition();
            iia.IngredientId = ingredient.IngredientId;
            iia.SupplierId = supplier.SupplierId;
            iia.Quantity = 2;
            iia.UnitCost = 5;
            dbContext.IngredientInventoryAdditions.Add(iia);
            dbContext.SaveChanges();
            Assert.That(iia.IngredientId, Is.EqualTo(ingredient.IngredientId));
            Assert.That(iia.SupplierId, Is.EqualTo(supplier.SupplierId));
            Assert.IsNotNull(dbContext.IngredientInventoryAdditions.Find(iia.IngredientInventoryAdditionId));
        }

        [Test]
        public void UpdateTest()
        {
            iia = dbContext.IngredientInventoryAdditions.Find(1);
            int oldSupplierId = iia.SupplierId;
            int oldIngredientId = iia.IngredientId;
            double oldQuantity = iia.Quantity;

            //remove old values
            dbContext.IngredientInventoryAdditions.Remove(iia);
            dbContext.SaveChanges();

            Ingredient? ingredient;
            Supplier? supplier;
            ingredient = dbContext.Ingredients.Find(914);
            supplier = dbContext.Suppliers.Find(4);

            //update with new values above
            iia.SupplierId = supplier.SupplierId;
            iia.IngredientId = ingredient.IngredientId;
            iia.Quantity = 4;
            dbContext.IngredientInventoryAdditions.Add(iia);
            dbContext.SaveChanges();
            Assert.That(iia.SupplierId, Is.Not.EqualTo(oldSupplierId));
            Assert.That(iia.IngredientId, Is.Not.EqualTo(oldIngredientId));
            Assert.That(iia.Quantity, Is.Not.EqualTo(oldQuantity));
            Assert.IsNotNull(dbContext.IngredientInventoryAdditions.Find(iia.IngredientInventoryAdditionId));
            Console.WriteLine(iia);
        }

        [Test]
        public void DeleteTest()
        {
            Ingredient? ingredient;
            Supplier? supplier;

            ingredient = dbContext.Ingredients.Find(914);
            supplier = dbContext.Suppliers.Find(4);

            //create with above values
            iia = new IngredientInventoryAddition();
            iia.IngredientId = ingredient.IngredientId;
            iia.SupplierId = supplier.SupplierId;
            iia.Quantity = 2;
            iia.UnitCost = 5;
            dbContext.IngredientInventoryAdditions.Add(iia);
            dbContext.SaveChanges();
            Assert.That(iia.IngredientId, Is.EqualTo(ingredient.IngredientId));
            Assert.That(iia.SupplierId, Is.EqualTo(supplier.SupplierId));
            Assert.IsNotNull(dbContext.IngredientInventoryAdditions.Find(iia.IngredientInventoryAdditionId));

            //delete
            iia = dbContext.IngredientInventoryAdditions.Find(iia.IngredientInventoryAdditionId);
            dbContext.IngredientInventoryAdditions.Remove(iia);
            dbContext.SaveChanges();
            var deleted = dbContext.IngredientInventoryAdditions.Find(iia.IngredientInventoryAdditionId);
            Assert.IsNull(deleted);
        }

        public void PrintAll(List<IngredientInventoryAddition> ingredientInventoryAdditions)
        {
            foreach (IngredientInventoryAddition iia in ingredientInventoryAdditions)
            {
                Console.WriteLine(iia);
            }
        }
    }
}
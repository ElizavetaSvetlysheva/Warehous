using System;
using NUnit.Framework;
using WarehouseData.Context;
using WarehouseData.Models;
using WarehouseData.Services;

namespace WarehouseDataTests.Tests
{
    [TestFixture]
    public class ProductServiceTests
    {
        private ProductService _service;
        private Warehouse _warehouse;

        [SetUp]
        public void SetUp()
        {
            ApplicationContext.Organizations.Clear();
            ApplicationContext.Warehouses.Clear();
            ApplicationContext.Products.Clear();
            ApplicationContext.Invoices.Clear();
            ApplicationContext.ResetCounters();

            _service = new ProductService();

            var org = new Organization("Орг") { OrgId = ApplicationContext.NextOrgId() };
            ApplicationContext.Organizations.Add(org);

            _warehouse = new Warehouse("Склад", "Адрес", org.OrgId) { WhId = ApplicationContext.NextWhId() };
            ApplicationContext.Warehouses.Add(_warehouse);
        }

        private Product MakeProduct(string name = "Тестовый товар", decimal price = 100m, int qty = 10)
        {
            return new Product
            {
                Name = name,
                Price = price,
                Quantity = qty,
                Discount = 0,
                WarehouseId = _warehouse.WhId
            };
        }


        [Test]
        public void GetAll_EmptyContext_ReturnsEmptyList()
        {
            Assert.AreEqual(0, _service.GetAll().Count);
        }

        [Test]
        public void GetAll_AfterAddingProducts_ReturnsAll()
        {
            _service.Add(MakeProduct("Товар 1"));
            _service.Add(MakeProduct("Товар 2"));

            Assert.AreEqual(2, _service.GetAll().Count);
        }


        [Test]
        public void GetByWarehouse_ReturnsOnlyProductsOfThatWarehouse()
        {
            var org2 = new Organization("Орг2") { OrgId = ApplicationContext.NextOrgId() };
            ApplicationContext.Organizations.Add(org2);
            var wh2 = new Warehouse("Склад2", "Адрес2", org2.OrgId) { WhId = ApplicationContext.NextWhId() };
            ApplicationContext.Warehouses.Add(wh2);

            _service.Add(MakeProduct("Товар склада 1"));

            var prod2 = new Product { Name = "Товар склада 2", Price = 50m, Quantity = 5, WarehouseId = wh2.WhId };
            _service.Add(prod2);

            var result = _service.GetByWarehouse(_warehouse.WhId);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(_warehouse.WhId, result[0].WarehouseId);
        }

        [Test]
        public void GetByWarehouse_NoMatchingWarehouse_ReturnsEmptyList()
        {
            _service.Add(MakeProduct());

            Assert.AreEqual(0, _service.GetByWarehouse(9999).Count);
        }


        [Test]
        public void Add_ValidProduct_AddsToContext()
        {
            var p = MakeProduct("Ботинки Adidas");

            _service.Add(p);

            Assert.AreEqual(1, ApplicationContext.Products.Count);
            Assert.AreEqual("Ботинки Adidas", ApplicationContext.Products[0].Name);
        }

        [Test]
        public void Add_ValidProduct_AssignsId()
        {
            var p = MakeProduct();

            _service.Add(p);

            Assert.Greater(p.ProdId, 0);
        }

        [Test]
        public void Add_NullProduct_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _service.Add(null));
        }

        [Test]
        public void Add_ProductWithEmptyName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _service.Add(MakeProduct("")));
        }

        [Test]
        public void Add_ProductWithWhitespaceName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _service.Add(MakeProduct("   ")));
        }

        [Test]
        public void Add_ProductWithNegativePrice_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _service.Add(MakeProduct(price: -1m)));
        }

        [Test]
        public void Add_ProductWithZeroPrice_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _service.Add(MakeProduct(price: 0m)));
        }

        [Test]
        public void Add_ProductWithNegativeQuantity_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _service.Add(MakeProduct(qty: -5)));
        }

        [Test]
        public void Add_ProductWithZeroQuantity_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _service.Add(MakeProduct(qty: 0)));
        }


        [Test]
        public void Update_ExistingProduct_ChangesName()
        {
            var p = MakeProduct("Старое название");
            _service.Add(p);

            p.Name = "Новое название";
            _service.Update(p);

            Assert.AreEqual("Новое название", ApplicationContext.Products[0].Name);
        }

        [Test]
        public void Update_ExistingProduct_ChangesPrice()
        {
            var p = MakeProduct(price: 100m);
            _service.Add(p);

            p.Price = 250m;
            _service.Update(p);

            Assert.AreEqual(250m, ApplicationContext.Products[0].Price);
        }

        [Test]
        public void Update_ExistingProduct_ChangesQuantity()
        {
            var p = MakeProduct(qty: 10);
            _service.Add(p);

            p.Quantity = 50;
            _service.Update(p);

            Assert.AreEqual(50, ApplicationContext.Products[0].Quantity);
        }

        [Test]
        public void Update_ExistingProduct_ChangesDiscount()
        {
            var p = MakeProduct();
            _service.Add(p);

            p.Discount = 15;
            _service.Update(p);

            Assert.AreEqual(15, ApplicationContext.Products[0].Discount);
        }

        [Test]
        public void Update_NullProduct_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _service.Update(null));
        }

        [Test]
        public void Update_ProductWithEmptyName_ThrowsArgumentException()
        {
            var p = MakeProduct();
            _service.Add(p);

            p.Name = "";

            Assert.Throws<ArgumentException>(() => _service.Update(p));
        }

        [Test]
        public void Update_ProductWithNegativePrice_ThrowsArgumentException()
        {
            var p = MakeProduct();
            _service.Add(p);

            p.Price = -10m;

            Assert.Throws<ArgumentException>(() => _service.Update(p));
        }

        [Test]
        public void Update_ProductWithNegativeQuantity_ThrowsArgumentException()
        {
            var p = MakeProduct();
            _service.Add(p);

            p.Quantity = -1;

            Assert.Throws<ArgumentException>(() => _service.Update(p));
        }


        [Test]
        public void Delete_ExistingProduct_RemovesFromContext()
        {
            var p = MakeProduct();
            _service.Add(p);

            _service.Delete(p);

            Assert.AreEqual(0, ApplicationContext.Products.Count);
        }

        [Test]
        public void Delete_OneOfMultipleProducts_RemovesCorrectOne()
        {
            var p1 = MakeProduct("Товар 1");
            var p2 = MakeProduct("Товар 2");
            _service.Add(p1);
            _service.Add(p2);

            _service.Delete(p1);

            Assert.AreEqual(1, ApplicationContext.Products.Count);
            Assert.AreEqual("Товар 2", ApplicationContext.Products[0].Name);
        }
    }
}

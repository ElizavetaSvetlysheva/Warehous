using System;
using NUnit.Framework;
using WarehouseData.Context;
using WarehouseData.Models;
using WarehouseData.Services;

namespace WarehouseDataTests.Tests
{
    [TestFixture]
    public class WarehouseServiceTests
    {
        private WarehouseService _service;
        private Organization _org;

        [SetUp]
        public void SetUp()
        {
            ApplicationContext.Organizations.Clear();
            ApplicationContext.Warehouses.Clear();
            ApplicationContext.Products.Clear();
            ApplicationContext.Invoices.Clear();
            ApplicationContext.ResetCounters();

            _service = new WarehouseService();

            _org = new Organization("Тест Орг") { OrgId = ApplicationContext.NextOrgId() };
            ApplicationContext.Organizations.Add(_org);
        }


        [Test]
        public void GetAll_EmptyContext_ReturnsEmptyList()
        {
            var result = _service.GetAll();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void GetAll_AfterAddingWarehouses_ReturnsAll()
        {
            _service.Add(new Warehouse("Склад 1", "Адрес 1", _org.OrgId));
            _service.Add(new Warehouse("Склад 2", "Адрес 2", _org.OrgId));

            Assert.AreEqual(2, _service.GetAll().Count);
        }


        [Test]
        public void GetByOrganization_ReturnsOnlyWarehousesOfThatOrg()
        {
            var org2 = new Organization("Другая Орг") { OrgId = ApplicationContext.NextOrgId() };
            ApplicationContext.Organizations.Add(org2);

            _service.Add(new Warehouse("Склад Орг1", "Ул. 1", _org.OrgId));
            _service.Add(new Warehouse("Склад Орг2", "Ул. 2", org2.OrgId));

            var result = _service.GetByOrganization(_org.OrgId);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(_org.OrgId, result[0].OrgId);
        }

        [Test]
        public void GetByOrganization_NoMatchingOrg_ReturnsEmptyList()
        {
            _service.Add(new Warehouse("Склад", "Адрес", _org.OrgId));

            var result = _service.GetByOrganization(9999);

            Assert.AreEqual(0, result.Count);
        }


        [Test]
        public void Add_ValidWarehouse_AddsToContext()
        {
            var wh = new Warehouse("Главный склад", "Ул. Ленина 1", _org.OrgId);

            _service.Add(wh);

            Assert.AreEqual(1, ApplicationContext.Warehouses.Count);
            Assert.AreEqual("Главный склад", ApplicationContext.Warehouses[0].WhName);
        }

        [Test]
        public void Add_ValidWarehouse_AssignsId()
        {
            var wh = new Warehouse("Склад", "Адрес", _org.OrgId);

            _service.Add(wh);

            Assert.Greater(wh.WhId, 0);
        }

        [Test]
        public void Add_NullWarehouse_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _service.Add(null));
        }

        [Test]
        public void Add_MultipleWarehouses_IdsAreUnique()
        {
            var wh1 = new Warehouse("Склад 1", "Адрес 1", _org.OrgId);
            var wh2 = new Warehouse("Склад 2", "Адрес 2", _org.OrgId);

            _service.Add(wh1);
            _service.Add(wh2);

            Assert.AreNotEqual(wh1.WhId, wh2.WhId);
        }


        [Test]
        public void Update_ExistingWarehouse_ChangesName()
        {
            var wh = new Warehouse("Старое", "Адрес", _org.OrgId);
            _service.Add(wh);

            wh.WhName = "Новое";
            _service.Update(wh);

            Assert.AreEqual("Новое", ApplicationContext.Warehouses[0].WhName);
        }

        [Test]
        public void Update_ExistingWarehouse_ChangesAddress()
        {
            var wh = new Warehouse("Склад", "Старый адрес", _org.OrgId);
            _service.Add(wh);

            wh.WhAddress = "Новый адрес";
            _service.Update(wh);

            Assert.AreEqual("Новый адрес", ApplicationContext.Warehouses[0].WhAddress);
        }

        [Test]
        public void Update_NonExistentWarehouse_DoesNotThrow()
        {
            var wh = new Warehouse("Несуществующий", "Адрес", _org.OrgId) { WhId = 9999 };

            Assert.DoesNotThrow(() => _service.Update(wh));
        }

        [Test]
        public void Delete_WarehouseWithoutProducts_RemovesFromContext()
        {
            var wh = new Warehouse("Удаляемый", "Адрес", _org.OrgId);
            _service.Add(wh);

            _service.Delete(wh);

            Assert.AreEqual(0, ApplicationContext.Warehouses.Count);
        }

        [Test]
        public void Delete_WarehouseWithProducts_ThrowsInvalidOperationException()
        {
            var wh = new Warehouse("Склад с товарами", "Адрес", _org.OrgId);
            _service.Add(wh);

            ApplicationContext.Products.Add(new Product
            {
                ProdId = ApplicationContext.NextProdId(),
                Name = "Товар",
                WarehouseId = wh.WhId
            });

            Assert.Throws<InvalidOperationException>(() => _service.Delete(wh));
        }

        [Test]
        public void Delete_WarehouseWithProducts_WarehouseRemainsInContext()
        {
            var wh = new Warehouse("Защищённый", "Адрес", _org.OrgId);
            _service.Add(wh);

            ApplicationContext.Products.Add(new Product
            {
                ProdId = ApplicationContext.NextProdId(),
                Name = "Товар",
                WarehouseId = wh.WhId
            });

            try { _service.Delete(wh); } catch { }

            Assert.AreEqual(1, ApplicationContext.Warehouses.Count);
        }
    }
}

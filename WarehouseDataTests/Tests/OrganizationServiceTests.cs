using System;
using NUnit.Framework;
using WarehouseData.Context;
using WarehouseData.Models;
using WarehouseData.Services;

namespace WarehouseDataTests.Tests
{
    [TestFixture]
    public class OrganizationServiceTests
    {
        private OrganizationService _service;

        [SetUp]
        public void SetUp()
        {
           
            ApplicationContext.Organizations.Clear();
            ApplicationContext.Warehouses.Clear();
            ApplicationContext.Products.Clear();
            ApplicationContext.Invoices.Clear();
            ApplicationContext.ResetCounters();

            _service = new OrganizationService();
        }


        [Test]
        public void GetAll_EmptyContext_ReturnsEmptyList()
        {
            var result = _service.GetAll();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void GetAll_AfterAddingOrganizations_ReturnsAll()
        {
            _service.Add(new Organization("Org A"));
            _service.Add(new Organization("Org B"));

            var result = _service.GetAll();

            Assert.AreEqual(2, result.Count);
        }

       

        [Test]
        public void Add_ValidOrganization_AddsToContext()
        {
            var org = new Organization("ООО Рога и Копыта");

            _service.Add(org);

            Assert.AreEqual(1, ApplicationContext.Organizations.Count);
            Assert.AreEqual("ООО Рога и Копыта", ApplicationContext.Organizations[0].OrgName);
        }

        [Test]
        public void Add_ValidOrganization_AssignsId()
        {
            var org = new Organization("Тест");

            _service.Add(org);

            Assert.Greater(org.OrgId, 0);
        }

        [Test]
        public void Add_OrganizationWithEmptyName_ThrowsArgumentException()
        {
            var org = new Organization { OrgName = "" };

            Assert.Throws<ArgumentException>(() => _service.Add(org));
        }

        [Test]
        public void Add_OrganizationWithWhitespaceName_ThrowsArgumentException()
        {
            var org = new Organization { OrgName = "   " };

            Assert.Throws<ArgumentException>(() => _service.Add(org));
        }

        [Test]
        public void Add_OrganizationWithNullName_ThrowsArgumentException()
        {
            var org = new Organization { OrgName = null };

            Assert.Throws<ArgumentException>(() => _service.Add(org));
        }

        [Test]
        public void Add_MultipleOrganizations_IdsAreUnique()
        {
            var org1 = new Organization("Первая");
            var org2 = new Organization("Вторая");

            _service.Add(org1);
            _service.Add(org2);

            Assert.AreNotEqual(org1.OrgId, org2.OrgId);
        }


        [Test]
        public void Update_ExistingOrganization_ChangesName()
        {
            var org = new Organization("Старое название");
            _service.Add(org);

            org.OrgName = "Новое название";
            _service.Update(org);

            Assert.AreEqual("Новое название", ApplicationContext.Organizations[0].OrgName);
        }

        [Test]
        public void Update_WithEmptyName_ThrowsArgumentException()
        {
            var org = new Organization("Валидная");
            _service.Add(org);

            org.OrgName = "";

            Assert.Throws<ArgumentException>(() => _service.Update(org));
        }

        [Test]
        public void Update_WithWhitespaceName_ThrowsArgumentException()
        {
            var org = new Organization("Валидная");
            _service.Add(org);

            org.OrgName = "   ";

            Assert.Throws<ArgumentException>(() => _service.Update(org));
        }

        [Test]
        public void Update_NonExistentOrganization_DoesNotThrow()
        {
            var org = new Organization("Несуществующая") { OrgId = 999 };

            // Не должно бросать исключение — просто ничего не делает
            Assert.DoesNotThrow(() => _service.Update(org));
        }


        [Test]
        public void Delete_OrganizationWithoutWarehouses_RemovesFromContext()
        {
            var org = new Organization("Для удаления");
            _service.Add(org);

            _service.Delete(org);

            Assert.AreEqual(0, ApplicationContext.Organizations.Count);
        }

        [Test]
        public void Delete_OrganizationWithWarehouses_ThrowsInvalidOperationException()
        {
            var org = new Organization("С складами");
            _service.Add(org);

            var wh = new WarehouseData.Models.Warehouse("Склад", "Адрес", org.OrgId) { WhId = ApplicationContext.NextWhId() };
            ApplicationContext.Warehouses.Add(wh);

            Assert.Throws<InvalidOperationException>(() => _service.Delete(org));
        }

        [Test]
        public void Delete_OrganizationWithWarehouses_OrganizationRemainsInContext()
        {
            var org = new Organization("Защищённая");
            _service.Add(org);

            ApplicationContext.Warehouses.Add(
                new WarehouseData.Models.Warehouse("Склад", "Адрес", org.OrgId) { WhId = ApplicationContext.NextWhId() });

            try { _service.Delete(org); } catch { /* ожидаемо */ }

            Assert.AreEqual(1, ApplicationContext.Organizations.Count);
        }
    }
}

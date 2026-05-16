using System;
using System.Collections.Generic;
using NUnit.Framework;
using WarehouseData.Context;
using WarehouseData.Models;
using WarehouseData.Services;

namespace WarehouseDataTests.Tests
{
    [TestFixture]
    public class InvoiceServiceTests
    {
        private InvoiceService _invoiceService;
        private ProductService _productService;
        private Warehouse _warehouse;
        private Product _product;

        [SetUp]
        public void SetUp()
        {
            ApplicationContext.Organizations.Clear();
            ApplicationContext.Warehouses.Clear();
            ApplicationContext.Products.Clear();
            ApplicationContext.Invoices.Clear();
            ApplicationContext.ResetCounters();

            _invoiceService = new InvoiceService();
            _productService = new ProductService();

            var org = new Organization("Орг") { OrgId = ApplicationContext.NextOrgId() };
            ApplicationContext.Organizations.Add(org);

            _warehouse = new Warehouse("Склад", "Адрес", org.OrgId) { WhId = ApplicationContext.NextWhId() };
            ApplicationContext.Warehouses.Add(_warehouse);

            _product = new Product
            {
                ProdId = ApplicationContext.NextProdId(),
                Name = "Тестовый товар",
                Price = 100m,
                Quantity = 50,
                WarehouseId = _warehouse.WhId
            };
            ApplicationContext.Products.Add(_product);
        }

        private Invoice MakeInvoice(string type, int productId, int quantity)
        {
            return new Invoice
            {
                Number = $"INV-{Guid.NewGuid():N}".Substring(0, 12),
                Date = DateTime.Today,
                Type = type,
                WarehouseId = _warehouse.WhId,
                Items = new List<InvoiceItem>
                {
                    new InvoiceItem
                    {
                        ProductId = productId,
                        Quantity = quantity,
                        Price = 100m
                    }
                }
            };
        }


        [Test]
        public void GetAll_EmptyContext_ReturnsEmptyList()
        {
            Assert.AreEqual(0, _invoiceService.GetAll().Count);
        }

        [Test]
        public void GetAll_AfterAddingInvoices_ReturnsAll()
        {
            _invoiceService.Add(new Invoice("ИНВ-001", DateTime.Today, "Входящая", _warehouse.WhId));
            _invoiceService.Add(new Invoice("ИНВ-002", DateTime.Today, "Исходящая", _warehouse.WhId));

            Assert.AreEqual(2, _invoiceService.GetAll().Count);
        }


        [Test]
        public void GetByWarehouse_ReturnsOnlyInvoicesOfThatWarehouse()
        {
            var org2 = new Organization("Орг2") { OrgId = ApplicationContext.NextOrgId() };
            ApplicationContext.Organizations.Add(org2);
            var wh2 = new Warehouse("Склад2", "Адрес2", org2.OrgId) { WhId = ApplicationContext.NextWhId() };
            ApplicationContext.Warehouses.Add(wh2);

            _invoiceService.Add(new Invoice("ИНВ-001", DateTime.Today, "Входящая", _warehouse.WhId));
            _invoiceService.Add(new Invoice("ИНВ-002", DateTime.Today, "Входящая", wh2.WhId));

            var result = _invoiceService.GetByWarehouse(_warehouse.WhId);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(_warehouse.WhId, result[0].WarehouseId);
        }


        [Test]
        public void Add_ValidInvoice_AddsToContext()
        {
            var inv = new Invoice("ИНВ-001", DateTime.Today, "Входящая", _warehouse.WhId);

            _invoiceService.Add(inv);

            Assert.AreEqual(1, ApplicationContext.Invoices.Count);
        }

        [Test]
        public void Add_ValidInvoice_AssignsId()
        {
            var inv = new Invoice("ИНВ-001", DateTime.Today, "Входящая", _warehouse.WhId);

            _invoiceService.Add(inv);

            Assert.Greater(inv.Id, 0);
        }

        [Test]
        public void Add_InvoiceWithEmptyNumber_ThrowsArgumentException()
        {
            var inv = new Invoice { Number = "", Date = DateTime.Today, Type = "Входящая", WarehouseId = _warehouse.WhId };

            Assert.Throws<ArgumentException>(() => _invoiceService.Add(inv));
        }

        [Test]
        public void Add_InvoiceWithNullNumber_ThrowsArgumentException()
        {
            var inv = new Invoice { Number = null, Date = DateTime.Today, Type = "Входящая", WarehouseId = _warehouse.WhId };

            Assert.Throws<ArgumentException>(() => _invoiceService.Add(inv));
        }

        [Test]
        public void ProcessInvoice_IncomingInvoice_IncreasesProductQuantity()
        {
            int initialQty = _product.Quantity; // 50
            var inv = MakeInvoice("Входящая", _product.ProdId, 10);

            _invoiceService.ProcessInvoice(inv);

            Assert.AreEqual(initialQty + 10, _product.Quantity);
        }

        [Test]
        public void ProcessInvoice_IncomingInvoice_MultipleItems_IncreasesQuantitiesCorrectly()
        {
            var product2 = new Product
            {
                ProdId = ApplicationContext.NextProdId(),
                Name = "Второй товар",
                Price = 200m,
                Quantity = 20,
                WarehouseId = _warehouse.WhId
            };
            ApplicationContext.Products.Add(product2);

            var inv = new Invoice
            {
                Number = "ИНВ-MULTI",
                Date = DateTime.Today,
                Type = "Входящая",
                WarehouseId = _warehouse.WhId,
                Items = new List<InvoiceItem>
                {
                    new InvoiceItem { ProductId = _product.ProdId, Quantity = 5, Price = 100m },
                    new InvoiceItem { ProductId = product2.ProdId, Quantity = 3, Price = 200m }
                }
            };

            _invoiceService.ProcessInvoice(inv);

            Assert.AreEqual(55, _product.Quantity);
            Assert.AreEqual(23, product2.Quantity);
        }

        
        [Test]
        public void ProcessInvoice_OutgoingInvoice_DecreasesProductQuantity()
        {
            int initialQty = _product.Quantity; // 50
            var inv = MakeInvoice("Исходящая", _product.ProdId, 15);

            _invoiceService.ProcessInvoice(inv);

            Assert.AreEqual(initialQty - 15, _product.Quantity);
        }

        [Test]
        public void ProcessInvoice_OutgoingInvoice_ExactQuantity_SetsQuantityToZero()
        {
            var inv = MakeInvoice("Исходящая", _product.ProdId, _product.Quantity);

            _invoiceService.ProcessInvoice(inv);

            Assert.AreEqual(0, _product.Quantity);
        }

        [Test]
        public void ProcessInvoice_OutgoingInvoice_InsufficientStock_ThrowsInvalidOperationException()
        {
            var inv = MakeInvoice("Исходящая", _product.ProdId, _product.Quantity + 1);

            Assert.Throws<InvalidOperationException>(() => _invoiceService.ProcessInvoice(inv));
        }

        [Test]
        public void ProcessInvoice_OutgoingInvoice_InsufficientStock_QuantityUnchanged()
        {
            int initialQty = _product.Quantity;
            var inv = MakeInvoice("Исходящая", _product.ProdId, initialQty + 100);

            try { _invoiceService.ProcessInvoice(inv); } catch { /* ожидаемо */ }

            Assert.AreEqual(initialQty, _product.Quantity);
        }

        

        [Test]
        public void Delete_IncomingInvoice_ReversesQuantityIncrease()
        {
            var inv = MakeInvoice("Входящая", _product.ProdId, 10);
            _invoiceService.Add(inv);
            _invoiceService.ProcessInvoice(inv);

            int qtyAfterProcess = _product.Quantity; // 60

            _invoiceService.Delete(inv);

            Assert.AreEqual(qtyAfterProcess - 10, _product.Quantity);
        }

        [Test]
        public void Delete_OutgoingInvoice_ReversesQuantityDecrease()
        {
            var inv = MakeInvoice("Исходящая", _product.ProdId, 10);
            _invoiceService.Add(inv);
            _invoiceService.ProcessInvoice(inv);

            int qtyAfterProcess = _product.Quantity; // 40

            _invoiceService.Delete(inv);

            Assert.AreEqual(qtyAfterProcess + 10, _product.Quantity);
        }

        [Test]
        public void Delete_Invoice_RemovesFromContext()
        {
            var inv = new Invoice("ИНВ-DEL", DateTime.Today, "Входящая", _warehouse.WhId);
            _invoiceService.Add(inv);

            _invoiceService.Delete(inv);

            Assert.AreEqual(0, ApplicationContext.Invoices.Count);
        }

       

        [Test]
        public void Update_ChangesInvoiceNumber()
        {
            var inv = new Invoice("СТАРЫЙ-001", DateTime.Today, "Входящая", _warehouse.WhId);
            _invoiceService.Add(inv);

            var updated = new Invoice
            {
                Id = inv.Id,
                Number = "НОВЫЙ-001",
                Date = inv.Date,
                Type = inv.Type,
                WarehouseId = inv.WarehouseId,
                Items = new List<InvoiceItem>()
            };

            _invoiceService.Update(updated);

            Assert.AreEqual("НОВЫЙ-001", ApplicationContext.Invoices[0].Number);
        }

        [Test]
        public void Update_NonExistentInvoice_DoesNotThrow()
        {
            var inv = new Invoice
            {
                Id = 9999,
                Number = "НЕ-СУЩЕСТВУЕТ",
                Date = DateTime.Today,
                Type = "Входящая",
                WarehouseId = _warehouse.WhId,
                Items = new List<InvoiceItem>()
            };

            Assert.DoesNotThrow(() => _invoiceService.Update(inv));
        }
    }
}

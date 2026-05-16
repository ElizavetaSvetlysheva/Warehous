using System.Collections.Generic;
using System.Linq;
using WarehouseData.Context;
using WarehouseData.Models;

namespace WarehouseData.Services
{
    public class InvoiceService : IDataService<Invoice>
    {
        public List<Invoice> GetAll() => ApplicationContext.Invoices;

        public List<Invoice> GetByWarehouse(int whId)
        {
            return ApplicationContext.Invoices.Where(i => i.WarehouseId == whId).ToList();
        }

        public void Add(Invoice item)
        {
            if (string.IsNullOrWhiteSpace(item.Number))
                throw new System.ArgumentException("Номер накладной не может быть пустым");

            item.Id = ApplicationContext.NextInvoiceId();
            ApplicationContext.Invoices.Add(item);
        }

        public void Update(Invoice item)
        {
            var existing = ApplicationContext.Invoices.FirstOrDefault(i => i.Id == item.Id);
            if (existing == null) return;

            string oldType  = existing.Type;
            // Копируем список, чтобы после перезаписи existing.Items откат применился к старым позициям
            var    oldItems = new System.Collections.Generic.List<InvoiceItem>(existing.Items);

            ReverseInvoice(oldType, oldItems);

            existing.Number = item.Number;
            existing.Date   = item.Date;
            existing.Type   = item.Type;
            existing.Items  = item.Items;

            ProcessInvoice(existing);
        }

        private void ReverseInvoice(Invoice invoice)
        {
            ReverseInvoice(invoice.Type, invoice.Items);
        }

        private void ReverseInvoice(string type, System.Collections.Generic.List<InvoiceItem> items)
        {
            var productService = new ProductService();
            foreach (var item in items)
            {
                var product = productService.GetAll().FirstOrDefault(p => p.ProdId == item.ProductId);
                if (product == null) continue;

                if (type == "Входящая")
                    product.Quantity -= item.Quantity;
                else if (type == "Исходящая")
                    product.Quantity += item.Quantity;

                productService.Update(product);
            }
        }

        public void Delete(Invoice item)
        {
            ReverseInvoice(item);
            ApplicationContext.Invoices.Remove(item);
        }

        public void ProcessInvoice(Invoice invoice)
        {
            var productService = new ProductService();

            foreach (var item in invoice.Items)
            {
                var product = productService.GetAll().FirstOrDefault(p => p.ProdId == item.ProductId);
                if (product != null)
                {
                    if (invoice.Type == "Входящая")
                    {
                        product.Quantity += item.Quantity;
                    }
                    else if (invoice.Type == "Исходящая")
                    {
                        if (product.Quantity < item.Quantity)
                            throw new System.InvalidOperationException($"Недостаточно товара {product.Name} на складе");
                        product.Quantity -= item.Quantity;
                    }
                    productService.Update(product);
                }
            }
        }
    }
}

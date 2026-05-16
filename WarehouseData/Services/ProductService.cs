using System.Collections.Generic;
using System.Linq;
using WarehouseData.Context;
using WarehouseData.Models;

namespace WarehouseData.Services
{
    public class ProductService : IDataService<Product>
    {
        public List<Product> GetAll() => ApplicationContext.Products;

        public List<Product> GetByWarehouse(int whId)
        {
            return ApplicationContext.Products.Where(p => p.WarehouseId == whId).ToList();
        }

        public void Add(Product item)
        {
            if (item == null)
                throw new System.ArgumentNullException(nameof(item));
            if (string.IsNullOrWhiteSpace(item.Name))
                throw new System.ArgumentException("Название товара не может быть пустым");
            if (item.Price < 0)
                throw new System.ArgumentException("Цена не может быть отрицательной");
            if (item.Quantity < 0)
                throw new System.ArgumentException("Остаток не может быть отрицательным");

            item.ProdId = ApplicationContext.NextProdId();
            ApplicationContext.Products.Add(item);
        }

        public void Update(Product item)
        {
            if (item == null)
                throw new System.ArgumentNullException(nameof(item));
            if (string.IsNullOrWhiteSpace(item.Name))
                throw new System.ArgumentException("Название товара не может быть пустым");
            if (item.Price < 0)
                throw new System.ArgumentException("Цена не может быть отрицательной");
            if (item.Quantity < 0)
                throw new System.ArgumentException("Остаток не может быть отрицательным");

            var existing = ApplicationContext.Products.FirstOrDefault(p => p.ProdId == item.ProdId);
            if (existing != null)
            {
                existing.Name = item.Name;
                existing.Price = item.Price;
                existing.Quantity = item.Quantity;
                existing.Discount = item.Discount;
                existing.Category = item.Category;
                existing.Manufacturer = item.Manufacturer;
                existing.Supplier = item.Supplier;
                existing.PhotoPath = item.PhotoPath;
            }
        }

        public void Delete(Product item)
        {
            ApplicationContext.Products.Remove(item);
        }
    }
}

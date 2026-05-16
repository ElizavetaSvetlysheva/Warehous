using System.Collections.ObjectModel;
using WarehouseData.Context;
using WarehouseData.Models;
using WarehouseData.Services;

namespace Warehouse.ViewModels
{
    public class ProductViewModel : BaseViewModel
    {
        private readonly ProductService _service = new ProductService();
        private int _currentWhId;

        public ObservableCollection<Product> Products { get; set; }
        public Product SelectedProduct { get; set; }

        public ObservableCollection<Category> Categories { get; set; }
        public ObservableCollection<Manufacturer> Manufacturers { get; set; }
        public ObservableCollection<Supplier> Suppliers { get; set; }

        public ProductViewModel()
        {
            Products = new ObservableCollection<Product>();
            Categories = new ObservableCollection<Category>(ApplicationContext.Categories);
            Manufacturers = new ObservableCollection<Manufacturer>(ApplicationContext.Manufacturers);
            Suppliers = new ObservableCollection<Supplier>(ApplicationContext.Suppliers);
        }

        public void LoadData(int whId)
        {
            _currentWhId = whId;
            Products.Clear();
            foreach (var p in _service.GetByWarehouse(whId))
                Products.Add(p);
        }

        public void AddProduct(Product product)
        {
            product.WarehouseId = _currentWhId;
            _service.Add(product);
            Products.Add(product);
        }

        public void UpdateProduct(Product product)
        {
            _service.Update(product);
            var whId = _currentWhId;
            Products.Clear();
            foreach (var p in _service.GetByWarehouse(whId))
                Products.Add(p);
        }

        public void DeleteProduct(Product product)
        {
            if (product == null) return;
            _service.Delete(product);
            Products.Remove(product);
        }

        
        public void ImportProducts(string filePath)
        {
            
        }
    }
}

using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Warehouse.ViewModels;
using Warehouse.Views;
using WarehouseData.Models;
using WhModel = WarehouseData.Models.Warehouse;

namespace Warehouse
{
    public partial class MainWindow : Window
    {
        public OrganizationViewModel OrgVM { get; set; }
        public WarehouseViewModel WarehouseVM { get; set; }
        public ProductViewModel ProductVM { get; set; }
        public InvoiceViewModel InvoiceVM { get; set; }

        private Organization _selectedOrg;
        private WhModel _selectedWarehouse;

        public MainWindow()
        {
            InitializeComponent();

            OrgVM       = new OrganizationViewModel();
            WarehouseVM = new WarehouseViewModel();
            ProductVM   = new ProductViewModel();
            InvoiceVM   = new InvoiceViewModel();

            this.DataContext = this;

            listWarehouses.ItemsSource = WarehouseVM.Warehouses;
            gridProducts.ItemsSource   = ProductVM.Products;

            UpdateContextBar();
        }

        public void SelectOrganization(Organization org)
        {
            _selectedOrg       = org;
            _selectedWarehouse = null;

            this.Title = org.OrgName;
            WarehouseVM.LoadData(org.OrgId);
            ProductVM.Products.Clear();
            InvoiceVM.Invoices.Clear();

            UpdateContextBar();
        }

        private void UpdateContextBar()
        {
            contextBar.SetOrganization(_selectedOrg?.OrgName ?? "Организация не выбрана");
            contextBar.SetWarehouse(_selectedWarehouse?.WhName ?? "не выбран");
        }

        private void Close_Click(object sender, RoutedEventArgs e) => this.Close();

        private void OpenOrganizations_Click(object sender, RoutedEventArgs e)
        {
            var win = new OrganizationWindow { Owner = this };
            win.ShowDialog();
            OrgVM.LoadData();
        }

        private void listWarehouses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedWarehouse = listWarehouses.SelectedItem as WhModel;
            if (_selectedWarehouse != null)
            {
                ProductVM.LoadData(_selectedWarehouse.WhId);
                InvoiceVM.LoadData(_selectedWarehouse.WhId);
            }
            UpdateContextBar();
            gridProducts.ItemsSource = ProductVM.Products;
        }

        private void AddWarehouse_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedOrg == null)
            {
                MessageBox.Show("Сначала выберите организацию (Справочники → Организации).",
                    "Нет организации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var nameDialog = new InputDialog("Введите название нового склада:")
            {
                RequireNonEmpty = true,
                Owner = this
            };
            if (nameDialog.ShowDialog() != true) return;

            var addrDialog = new InputDialog("Введите адрес склада:") { Owner = this };
            addrDialog.ShowDialog();

            try
            {
                WarehouseVM.AddWarehouse(nameDialog.Result, addrDialog.Result ?? "");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void EditWarehouse_Click(object sender, RoutedEventArgs e)
        {
            var wh = listWarehouses.SelectedItem as WhModel;
            if (wh == null)
            {
                MessageBox.Show("Выберите склад для редактирования.", "Нет выбора",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var nameDialog = new InputDialog("Изменить название склада:", wh.WhName)
            {
                RequireNonEmpty = true,
                Owner = this
            };
            if (nameDialog.ShowDialog() != true) return;

            var addrDialog = new InputDialog("Изменить адрес склада:", wh.WhAddress) { Owner = this };
            addrDialog.ShowDialog();

            try
            {
                WarehouseVM.EditWarehouse(wh, nameDialog.Result ?? wh.WhName, addrDialog.Result ?? wh.WhAddress);
                UpdateContextBar();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteWarehouse_Click(object sender, RoutedEventArgs e)
        {
            var wh = listWarehouses.SelectedItem as WhModel;
            if (wh == null)
            {
                MessageBox.Show("Выберите склад для удаления.", "Нет выбора",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var confirmDialog = new Views.InputDialog(
                $"Для подтверждения удаления введите название склада:\n\"{wh.WhName}\"")
            {
                Owner = this
            };

            if (confirmDialog.ShowDialog() != true) return;

            if (confirmDialog.Result != wh.WhName)
            {
                MessageBox.Show("Название введено неверно. Удаление отменено.",
                    "Ошибка подтверждения", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                WarehouseVM.DeleteWarehouse(wh);
                _selectedWarehouse = null;
                ProductVM.Products.Clear();
                InvoiceVM.Invoices.Clear();
                UpdateContextBar();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedWarehouse == null)
            {
                MessageBox.Show("Сначала выберите склад.", "Нет склада",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var dialog = new ProductDialog(ProductVM.Categories, ProductVM.Manufacturers, ProductVM.Suppliers);
            if (dialog.ShowDialog() == true)
                ProductVM.AddProduct(dialog.Product);
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            var product = gridProducts.SelectedItem as Product;
            if (product == null)
            {
                MessageBox.Show("Выберите товар для редактирования.", "Нет выбора",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var dialog = new ProductDialog(ProductVM.Categories, ProductVM.Manufacturers, ProductVM.Suppliers, product);
            if (dialog.ShowDialog() == true)
                ProductVM.UpdateProduct(dialog.Product);
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            var product = gridProducts.SelectedItem as Product;
            if (product == null)
            {
                MessageBox.Show("Выберите товар для удаления.", "Нет выбора",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (MessageBox.Show($"Удалить товар «{product.Name}»?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                ProductVM.DeleteProduct(product);
        }

        private void ImportProducts_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedWarehouse == null)
            {
                MessageBox.Show("Сначала выберите склад.", "Нет склада",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var ofd = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "CSV файлы|*.csv",
                Title  = "Импорт товаров из CSV"
            };
            if (ofd.ShowDialog() != true) return;
            try
            {
                var lines = File.ReadAllLines(ofd.FileName);
                int imported = 0;
                foreach (var line in lines.Skip(1))
                {
                    var parts = line.Split(';');
                    if (parts.Length < 4) continue;
                    var product = new Product
                    {
                        Name        = parts[0].Trim(),
                        Price       = decimal.TryParse(parts[1].Trim(), out var p) ? p : 0,
                        Quantity    = int.TryParse(parts[2].Trim(), out var q)     ? q : 0,
                        Discount    = int.TryParse(parts[3].Trim(), out var d)     ? d : 0,
                        WarehouseId = _selectedWarehouse.WhId
                    };
                    ProductVM.AddProduct(product);
                    imported++;
                }
                MessageBox.Show($"Импортировано товаров: {imported}", "Импорт завершён",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка импорта: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var query = txtSearch.Text.Trim().ToLower();
            gridProducts.ItemsSource = string.IsNullOrEmpty(query)
                ? (System.Collections.IEnumerable)ProductVM.Products
                : ProductVM.Products.Where(p => p.Name != null && p.Name.ToLower().Contains(query)).ToList();
        }

        private void OpenInvoices_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedWarehouse == null)
            {
                MessageBox.Show("Сначала выберите склад.", "Нет склада",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var win = new InvoiceListWindow(_selectedWarehouse, InvoiceVM, ProductVM) { Owner = this };
            win.ShowDialog();
         
            ProductVM.LoadData(_selectedWarehouse.WhId);
            gridProducts.ItemsSource = ProductVM.Products;
        }

        private void AddInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedWarehouse == null)
            {
                MessageBox.Show("Сначала выберите склад.", "Нет склада",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var dialog = new InvoiceDialog(ProductVM.Products);
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    InvoiceVM.AddInvoice(dialog.InvoiceNumber, dialog.InvoiceType, dialog.Items);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка проводки",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                ProductVM.LoadData(_selectedWarehouse.WhId);
                gridProducts.ItemsSource = ProductVM.Products;
            }
        }

        private void EditInvoice_Click(object sender, RoutedEventArgs e)
        {
            OpenInvoices_Click(sender, e);
        }

        private void DeleteInvoice_Click(object sender, RoutedEventArgs e)
        {
            OpenInvoices_Click(sender, e);
        }
    }
}

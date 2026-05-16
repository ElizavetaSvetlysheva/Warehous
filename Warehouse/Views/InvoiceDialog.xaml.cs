using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using WarehouseData.Models;

namespace Warehouse.Views
{
    public class InvoiceRow
    {
        public Product SelectedProduct { get; set; }
        public string ProductName => SelectedProduct?.Name ?? "(выберите товар)";
        public int Quantity { get; set; } = 1;
        public decimal Price { get; set; }
    }

    public partial class InvoiceDialog : Window
    {
        public string InvoiceNumber { get; private set; }
        public string InvoiceType { get; private set; }
        public ObservableCollection<InvoiceItem> Items { get; private set; }
        public ObservableCollection<Product> AvailableProducts { get; private set; }

        private ObservableCollection<InvoiceRow> _rows = new ObservableCollection<InvoiceRow>();

        public InvoiceDialog(ObservableCollection<Product> products, Invoice existing = null)
        {
            InitializeComponent();
            DataContext = this;
            AvailableProducts = products;
            gridItems.ItemsSource = _rows;

            if (existing != null)
            {
                txtNumber.Text = existing.Number;
                
                foreach (ComboBoxItem item in cmbType.Items)
                    if (item.Content.ToString() == existing.Type)
                        cmbType.SelectedItem = item;

                foreach (var inv in existing.Items)
                {
                    _rows.Add(new InvoiceRow
                    {
                        SelectedProduct = inv.Product,
                        Quantity = inv.Quantity,
                        Price = inv.Price
                    });
                }
            }
        }

        private void AddRow_Click(object sender, RoutedEventArgs e)
        {
            _rows.Add(new InvoiceRow());
        }

        private void RemoveRow_Click(object sender, RoutedEventArgs e)
        {
            if (gridItems.SelectedItem is InvoiceRow row)
                _rows.Remove(row);
        }

        private void ProductCombo_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is ComboBox cmb && gridItems.CurrentItem is InvoiceRow row)
                cmb.SelectedItem = row.SelectedProduct;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNumber.Text))
            {
                MessageBox.Show("Номер накладной обязателен.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            InvoiceNumber = txtNumber.Text.Trim();
            var comboItem = cmbType.SelectedItem as ComboBoxItem;
            InvoiceType = comboItem?.Content.ToString() ?? "Входящая";

            Items = new ObservableCollection<InvoiceItem>();
            foreach (var row in _rows)
            {
                if (row.SelectedProduct == null || row.Quantity <= 0) continue;
                Items.Add(new InvoiceItem
                {
                    Product = row.SelectedProduct,
                    ProductId = row.SelectedProduct.ProdId,
                    Quantity = row.Quantity,
                    Price = row.Price > 0 ? row.Price : row.SelectedProduct.Price
                });
            }

            if (Items.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы один товар в накладную.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}

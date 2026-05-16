using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using WarehouseData.Models;

namespace Warehouse.Views
{
    public partial class ProductDialog : Window
    {
        public Product Product { get; private set; }

        public ProductDialog(ObservableCollection<Category> categories,
                             ObservableCollection<Manufacturer> manufacturers,
                             ObservableCollection<Supplier> suppliers,
                             Product product = null)
        {
            InitializeComponent();

            cmbCategory.ItemsSource = categories;
            cmbManufacturer.ItemsSource = manufacturers;
            cmbSupplier.ItemsSource = suppliers;

            if (product != null)
            {
                Product = product;
                txtName.Text = product.Name;
                txtPrice.Text = product.Price.ToString();
                txtQuantity.Text = product.Quantity.ToString();
                txtDiscount.Text = product.Discount.ToString();
                cmbCategory.SelectedItem = product.Category;
                cmbManufacturer.SelectedItem = product.Manufacturer;
                cmbSupplier.SelectedItem = product.Supplier;
                txtPhoto.Text = product.PhotoPath;
                LoadImage(product.PhotoPath);
            }
            else
            {
                Product = new Product();
            }
        }

        private void SelectPhoto_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp",
                Title = "Выбрать фото товара"
            };
            if (ofd.ShowDialog() == true)
            {
                txtPhoto.Text = ofd.FileName;
                LoadImage(ofd.FileName);
            }
        }

        private void LoadImage(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                imgPreview.Source = null;
                return;
            }

            try
            {
                BitmapImage bitmap;

                if (path.StartsWith("pack://", StringComparison.OrdinalIgnoreCase))
                {
                    bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(path, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                }
                else if (File.Exists(path))
                {
                    bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        bitmap.StreamSource = stream;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                    }
                    bitmap.Freeze();
                }
                else
                {
                    imgPreview.Source = null;
                    return;
                }

                imgPreview.Source = bitmap;
            }
            catch
            {
                imgPreview.Source = null;
            }
        }

        private static readonly System.Windows.Media.Brush _errorBrush =
            new System.Windows.Media.SolidColorBrush(
                System.Windows.Media.Color.FromRgb(0xE8, 0x11, 0x23));  

        private static readonly System.Windows.Media.Brush _clearBrush =
            System.Windows.Media.Brushes.Transparent;

        private bool MarkError(System.Windows.Controls.Border border,
                               System.Windows.Controls.Control field,
                               string tooltip)
        {
            border.BorderBrush = _errorBrush;
            System.Windows.Controls.ToolTipService.SetToolTip(border, tooltip);
            field.Focus();
            return false;
        }

        private void ClearError(System.Windows.Controls.Border border)
        {
            border.BorderBrush = _clearBrush;
            System.Windows.Controls.ToolTipService.SetToolTip(border, null);
        }

        private void ClearAllErrors()
        {
            ClearError(borderName);
            ClearError(borderPrice);
            ClearError(borderQuantity);
            ClearError(borderDiscount);
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            ClearAllErrors();
            bool valid = true;

            if (string.IsNullOrWhiteSpace(txtName.Text))
                valid = MarkError(borderName, txtName, "Наименование не может быть пустым");

            decimal price = 0;
            if (!decimal.TryParse(txtPrice.Text, out price) || price < 0)
                valid = MarkError(borderPrice, txtPrice, "Цена — неотрицательное число");

            int qty = 0;
            if (!int.TryParse(txtQuantity.Text, out qty) || qty < 0)
                valid = MarkError(borderQuantity, txtQuantity, "Остаток — целое неотрицательное число");

            int disc = 0;
            if (!int.TryParse(txtDiscount.Text, out disc) || disc < 0 || disc > 100)
                valid = MarkError(borderDiscount, txtDiscount, "Скидка — число от 0 до 100");

            if (!valid) return;

            Product.Name         = txtName.Text.Trim();
            Product.Price        = price;
            Product.Quantity     = qty;
            Product.Discount     = disc;
            Product.Category     = cmbCategory.SelectedItem as Category;
            Product.Manufacturer = cmbManufacturer.SelectedItem as Manufacturer;
            Product.Supplier     = cmbSupplier.SelectedItem as Supplier;
            Product.PhotoPath    = string.IsNullOrWhiteSpace(txtPhoto.Text) ? null : txtPhoto.Text;

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}

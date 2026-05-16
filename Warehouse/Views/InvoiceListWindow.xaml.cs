using System;
using System.Windows;
using System.Windows.Controls;
using Warehouse.ViewModels;
using WarehouseData.Models;
using WhModel = WarehouseData.Models.Warehouse;

namespace Warehouse.Views
{
    public partial class InvoiceListWindow : Window
    {
        private readonly InvoiceViewModel _invoiceVM;
        private readonly ProductViewModel _productVM;
        private readonly WhModel _warehouse;

        public InvoiceListWindow(WhModel warehouse, InvoiceViewModel invoiceVM, ProductViewModel productVM)
        {
            InitializeComponent();

            _warehouse  = warehouse;
            _invoiceVM  = invoiceVM;
            _productVM  = productVM;

            listInvoices.ItemsSource = _invoiceVM.Invoices;
            txtWarehouseName.Text    = warehouse.WhName;
            this.Title               = $"Накладные — {warehouse.WhName}";
        }

        private void listInvoices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var invoice = listInvoices.SelectedItem as Invoice;
            gridItems.ItemsSource = invoice?.Items;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new InvoiceDialog(_productVM.Products);
            if (dialog.ShowDialog() != true) return;
            try
            {
                _invoiceVM.AddInvoice(dialog.InvoiceNumber, dialog.InvoiceType, dialog.Items);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка проводки",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var invoice = listInvoices.SelectedItem as Invoice;
            if (invoice == null)
            {
                MessageBox.Show("Выберите накладную для редактирования.", "Нет выбора",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var dialog = new InvoiceDialog(_productVM.Products, invoice);
            if (dialog.ShowDialog() == true)
                _invoiceVM.EditInvoice(invoice, dialog.InvoiceNumber, dialog.InvoiceType, dialog.Items);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var invoice = listInvoices.SelectedItem as Invoice;
            if (invoice == null)
            {
                MessageBox.Show("Выберите накладную для удаления.", "Нет выбора",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Удалить накладную «{invoice.Number}»?\nОстатки товаров будут восстановлены.",
                "Подтверждение удаления",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                _invoiceVM.DeleteInvoice(invoice);
                gridItems.ItemsSource = null;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

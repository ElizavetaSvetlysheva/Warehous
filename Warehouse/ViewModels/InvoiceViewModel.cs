using System.Collections.ObjectModel;
using System.Linq;
using WarehouseData.Models;
using WarehouseData.Services;

namespace Warehouse.ViewModels
{
    public class InvoiceViewModel : BaseViewModel
    {
        private readonly InvoiceService _service = new InvoiceService();
        private readonly ProductService _productService = new ProductService();
        private int _currentWhId;

        public ObservableCollection<Invoice> Invoices { get; set; }
        public Invoice SelectedInvoice { get; set; }

        public InvoiceViewModel()
        {
            Invoices = new ObservableCollection<Invoice>();
        }

        public void LoadData(int whId)
        {
            _currentWhId = whId;
            Invoices.Clear();
            foreach (var inv in _service.GetByWarehouse(whId))
                Invoices.Add(inv);
        }

        public void AddInvoice(string number, string type, ObservableCollection<InvoiceItem> items)
        {
            var invoice = new Invoice(number, System.DateTime.Now, type, _currentWhId);
            invoice.Items = items.ToList();

            _service.ProcessInvoice(invoice);

            _service.Add(invoice);
            Invoices.Add(invoice);
        }

        public void EditInvoice(Invoice invoice, string number, string type, ObservableCollection<InvoiceItem> items)
        {
            if (invoice == null) return;
            invoice.Number = number;
            invoice.Type   = type;
            invoice.Items  = items.ToList();
            _service.Update(invoice);

            var index = Invoices.IndexOf(invoice);
            if (index >= 0)
                Invoices[index] = invoice;
        }

        public void DeleteInvoice(Invoice invoice)
        {
            if (invoice == null) return;
            _service.Delete(invoice);
            Invoices.Remove(invoice);
        }
    }
}

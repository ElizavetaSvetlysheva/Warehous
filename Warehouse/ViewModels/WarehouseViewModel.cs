using System.Collections.ObjectModel;
using WarehouseData.Models;
using WarehouseData.Services;
using WhModel = WarehouseData.Models.Warehouse;

namespace Warehouse.ViewModels
{
    public class WarehouseViewModel : BaseViewModel
    {
        private readonly WarehouseService _service = new WarehouseService();
        private int _currentOrgId;

        public ObservableCollection<WhModel> Warehouses { get; set; }
            = new ObservableCollection<WhModel>();

        public WhModel SelectedWarehouse { get; set; }

        public WarehouseViewModel() { }

        public void LoadData(int orgId)
        {
            _currentOrgId = orgId;
            Warehouses.Clear();
            foreach (var wh in _service.GetByOrganization(orgId))
                Warehouses.Add(wh);
        }

        
        public void AddWarehouse(string name, string address)
        {
            var newWh = new WhModel(name, address, _currentOrgId);
            _service.Add(newWh);
            Warehouses.Add(newWh);
        }

        public void EditWarehouse(WhModel wh, string name, string address)
        {
            if (wh == null) return;
            wh.WhName = name;
            wh.WhAddress = address;
            _service.Update(wh);

            var index = Warehouses.IndexOf(wh);
            if (index >= 0)
            {
                Warehouses.RemoveAt(index);
                Warehouses.Insert(index, wh);
            }
        }

        public void DeleteWarehouse(WhModel wh)
        {
            if (wh == null) return;
            _service.Delete(wh);
            Warehouses.Remove(wh);
        }
    }
}

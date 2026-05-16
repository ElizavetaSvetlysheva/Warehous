using System.Collections.ObjectModel;
using WarehouseData.Models;
using WarehouseData.Services;

namespace Warehouse.ViewModels
{
    public class OrganizationViewModel : BaseViewModel
    {
        private readonly OrganizationService _service = new OrganizationService();

        public ObservableCollection<Organization> Organizations { get; set; }
            = new ObservableCollection<Organization>();

        public Organization SelectedOrganization { get; set; }

        public OrganizationViewModel()
        {
            LoadData();
        }

        public void LoadData()
        {
            Organizations.Clear();
            foreach (var org in _service.GetAll())
                Organizations.Add(org);
        }

        public void AddOrganization(string name)
        {
            var newOrg = new Organization(name);
            _service.Add(newOrg);
            Organizations.Add(newOrg);
        }

        public void EditOrganization(Organization org, string newName)
        {
            if (org == null) return;
            org.OrgName = newName;
            _service.Update(org);

            var index = Organizations.IndexOf(org);
            if (index >= 0)
            {
                Organizations.RemoveAt(index);
                Organizations.Insert(index, org);
            }
        }

        public void DeleteOrganization(Organization org)
        {
            if (org == null) return;
            _service.Delete(org);
            Organizations.Remove(org);
        }
    }
}

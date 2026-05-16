using System.Collections.Generic;
using System.Linq;
using WarehouseData.Context;
using WarehouseData.Models;

namespace WarehouseData.Services
{
    public class OrganizationService : IDataService<Organization>
    {
        public List<Organization> GetAll() => ApplicationContext.Organizations;

        public void Add(Organization item)
        {
            if (string.IsNullOrWhiteSpace(item.OrgName))
                throw new System.ArgumentException("Название организации не может быть пустым");

            if (item.OrgId == 0)
                item.OrgId = ApplicationContext.NextOrgId();

            ApplicationContext.Organizations.Add(item);
        }

        public void Update(Organization item)
        {
            if (string.IsNullOrWhiteSpace(item.OrgName))
                throw new System.ArgumentException("Название организации не может быть пустым");

            var existing = ApplicationContext.Organizations.FirstOrDefault(o => o.OrgId == item.OrgId);
            if (existing != null)
                existing.OrgName = item.OrgName;
        }

        public void Delete(Organization item)
        {
            if (ApplicationContext.Warehouses.Any(w => w.OrgId == item.OrgId))
                throw new System.InvalidOperationException("Нельзя удалить организацию с привязанными складами");

            ApplicationContext.Organizations.Remove(item);
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using WarehouseData.Context;
using WarehouseData.Models;

namespace WarehouseData.Services
{
    public class WarehouseService : IDataService<Warehouse>
    {
        public List<Warehouse> GetAll() => ApplicationContext.Warehouses;

        public List<Warehouse> GetByOrganization(int orgId)
        {
            return ApplicationContext.Warehouses.Where(w => w.OrgId == orgId).ToList();
        }

        public void Add(Warehouse item)
        {
            if (item == null)
                throw new System.ArgumentNullException(nameof(item));

            if (item.WhId == 0)
                item.WhId = ApplicationContext.NextWhId();

            ApplicationContext.Warehouses.Add(item);
        }

        public void Update(Warehouse item)
        {
            var existing = ApplicationContext.Warehouses.FirstOrDefault(w => w.WhId == item.WhId);
            if (existing != null)
            {
                existing.WhName = item.WhName;
                existing.WhAddress = item.WhAddress;
            }
        }

        public void Delete(Warehouse item)
        {
            if (ApplicationContext.Products.Any(p => p.WarehouseId == item.WhId))
                throw new System.InvalidOperationException("Нельзя удалить склад с привязанными товарами.");

            ApplicationContext.Warehouses.Remove(item);
        }
    }
}

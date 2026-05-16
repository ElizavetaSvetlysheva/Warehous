using System.Collections.Generic;

namespace WarehouseData.Services
{
    public interface IDataService<T>
    {
        List<T> GetAll();
        void Add(T item);
        void Update(T item);
        void Delete(T item);
    }
}
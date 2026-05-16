using System.Collections.Generic;
using WarehouseData.Models;

namespace WarehouseData.Context
{
    public static class ApplicationContext
    {
        public static List<Organization> Organizations { get; set; } = new List<Organization>();
        public static List<Warehouse> Warehouses { get; set; } = new List<Warehouse>();
        public static List<Product> Products { get; set; } = new List<Product>();
        public static List<Invoice> Invoices { get; set; } = new List<Invoice>();
        public static List<Category> Categories { get; set; } = new List<Category>();
        public static List<Manufacturer> Manufacturers { get; set; } = new List<Manufacturer>();
        public static List<Supplier> Suppliers { get; set; } = new List<Supplier>();

        private static int _orgCounter = 0;
        private static int _whCounter = 0;
        private static int _prodCounter = 0;
        private static int _invoiceCounter = 0;

        public static int NextOrgId() => ++_orgCounter;
        public static int NextWhId() => ++_whCounter;
        public static int NextProdId() => ++_prodCounter;
        public static int NextInvoiceId() => ++_invoiceCounter;

        public static void ResetCounters()
        {
            _orgCounter = 0;
            _whCounter = 0;
            _prodCounter = 0;
            _invoiceCounter = 0;
        }

        public static void Initialize()
        {
            var cat1 = new Category { Id = 1, Name = "Обувь" };
            var cat2 = new Category { Id = 2, Name = "Одежда" };
            var cat3 = new Category { Id = 3, Name = "Электроника" };
            var cat4 = new Category { Id = 4, Name = "Мебель" };
            var cat5 = new Category { Id = 5, Name = "Зоотовары" };
            var cat6 = new Category { Id = 6, Name = "Еда" };
            var cat7 = new Category { Id = 7, Name = "Инструмент" };
            var cat8 = new Category { Id = 8, Name = "Спорт товары" };
            var cat9 = new Category { Id = 9, Name = "Аксессуары" };
            var cat10 = new Category { Id = 10, Name = "Косметика" };
            Categories.Add(cat1); Categories.Add(cat2);
            Categories.Add(cat3); Categories.Add(cat4);
            Categories.Add(cat5); Categories.Add(cat6);
            Categories.Add(cat7); Categories.Add(cat8);
            Categories.Add(cat9); Categories.Add(cat10);

            var man1 = new Manufacturer { Id = 1, Name = "Adidas" };
            var man2 = new Manufacturer { Id = 2, Name = "Nike" };
            var man3 = new Manufacturer { Id = 3, Name = "Bosсh" };
            var man4 = new Manufacturer { Id = 4, Name = "Milka" };
            var man5 = new Manufacturer { Id = 5, Name = "Royal Canin" };
            var man6 = new Manufacturer { Id = 6, Name = "Диваны" };
            var man7 = new Manufacturer { Id = 7, Name = "Marmelato" };
            var man8 = new Manufacturer { Id = 8, Name = "Vivien Sabo" };
            var man9 = new Manufacturer { Id = 9, Name = "Indesit" };
            var man10 = new Manufacturer { Id = 10, Name = "Mixit" };
            Manufacturers.Add(man1); Manufacturers.Add(man2);
            Manufacturers.Add(man3); Manufacturers.Add(man4);
            Manufacturers.Add(man5); Manufacturers.Add(man6);
            Manufacturers.Add(man7); Manufacturers.Add(man8);
            Manufacturers.Add(man9); Manufacturers.Add(man10);

            var sup1 = new Supplier { Id = 1, Name = "ООО Поставщик" };
            var sup2 = new Supplier { Id = 2, Name = "ИП Иванов" };
            Suppliers.Add(sup1); Suppliers.Add(sup2);

            var org1 = new Organization("ООО Рога и копыта") { OrgId = NextOrgId() };
            var org2 = new Organization("Пупкин и сыновья") { OrgId = NextOrgId() };
            Organizations.Add(org1); Organizations.Add(org2);

            var wh1 = new Warehouse("Склад Рогов и Копыт №1", "Ул. Ленина 1", org1.OrgId) { WhId = NextWhId() };
            var wh2 = new Warehouse("Склад Рогов и Копыт №2", "Ул. Ленина 2", org1.OrgId) { WhId = NextWhId() };
            var wh3 = new Warehouse("Склад Пупкина №1", "Ул. Пушкина 10", org2.OrgId) { WhId = NextWhId() };
            Warehouses.Add(wh1); Warehouses.Add(wh2); Warehouses.Add(wh3);

            var prod1 = new Product
            {
                ProdId = NextProdId(),
                Name = "Adidas Sportswear X_PLRPATH",
                Category = cat1,
                Manufacturer = man1,
                Supplier = sup1,
                Price = 15000m,
                Quantity = 10,
                Discount = 5,
                WarehouseId = wh1.WhId,
                PhotoPath = null
            };
            var prod2 = new Product
            {
                ProdId = NextProdId(),
                Name = "Nike 10k 2 in 1",
                Category = cat2,
                Manufacturer = man2,
                Supplier = sup2,
                Price = 5000m,
                Quantity = 20,
                Discount = 10,
                WarehouseId = wh1.WhId,
                PhotoPath = null
            };
            var prod3 = new Product
            {
                ProdId = NextProdId(),
                Name = "Bosch GSR 12V-15",
                Category = cat7,
                Manufacturer = man3,
                Supplier = sup1,
                Price = 8500m,
                Quantity = 15,
                Discount = 0,
                WarehouseId = wh3.WhId,
                PhotoPath = null
            };
            var prod4 = new Product
            {
                ProdId = NextProdId(),
                Name = "Royal Canin Mini Adult",
                Category = cat5,
                Manufacturer = man5,
                Supplier = sup2,
                Price = 2200m,
                Quantity = 30,
                Discount = 0,
                WarehouseId = wh3.WhId,
                PhotoPath = null
            };
            var prod5 = new Product
            {
                ProdId = NextProdId(),
                Name = "Milka Oreo 100g",
                Category = cat6,
                Manufacturer = man4,
                Supplier = sup1,
                Price = 120m,
                Quantity = 200,
                Discount = 3,
                WarehouseId = wh3.WhId,
                PhotoPath = null
            };
            Products.Add(prod1); Products.Add(prod2);
            Products.Add(prod3); Products.Add(prod4); Products.Add(prod5);
        }
    }
}

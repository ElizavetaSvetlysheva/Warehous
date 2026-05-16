using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseData.Models
{
    [Table("products")]
    public class Product
    {
        [Column("id")]
        [Key]
        public int ProdId { get; set; }

        [Column("name")]
        [Required]
        public string Name { get; set; }

        [Column("photo")]
        public string PhotoPath { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("discount")]
        public int Discount { get; set; }

        public Category Category { get; set; }
        public Manufacturer Manufacturer { get; set; }
        public Supplier Supplier { get; set; }

        [Column("warehouseid")]
        public int WarehouseId { get; set; }

        public Product() { }
    }
}

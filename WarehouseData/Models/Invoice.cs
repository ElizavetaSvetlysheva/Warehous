using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseData.Models
{
    [Table("invoices")]
    public class Invoice
    {
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("number")]
        [Required]
        public string Number { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("warehouseid")]
        public int WarehouseId { get; set; }

        [ForeignKey("WarehouseId")]
        public Warehouse Warehouse { get; set; }

        public List<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();

        public Invoice() { }

        public Invoice(string number, DateTime date, string type, int warehouseId)
        {
            Number = number;
            Date = date;
            Type = type;
            WarehouseId = warehouseId;
        }
    }

    [Table("invoiceitems")]
    public class InvoiceItem
    {
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("invoiceid")]
        public int InvoiceId { get; set; }

        [ForeignKey("InvoiceId")]
        public Invoice Invoice { get; set; }

        [Column("productid")]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        public InvoiceItem() { }
    }
}

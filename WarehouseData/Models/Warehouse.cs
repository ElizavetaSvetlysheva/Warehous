using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseData.Models
{
    [Table("warehouses")]
    public class Warehouse
    {
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WhId { get; set; }

        [Column("whname")]
        [Required]
        public string WhName { get; set; }

        [Column("whaddress")]
        public string WhAddress { get; set; }

        [Column("orgid")]
        public int OrgId { get; set; }

        [ForeignKey("OrgId")]
        public Organization Organization { get; set; }

        public List<Product> products { get; set; } = new List<Product>();

        public Warehouse() { }

        public Warehouse(string name, string address, int orgId)
        {
            this.WhName = name;
            this.WhAddress = address;
            this.OrgId = orgId;
        }
    }
}

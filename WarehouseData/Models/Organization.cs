using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseData.Models
{
    [Table("orgs")]
    public class Organization
    {
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrgId { get; set; }

        [Column("orgname")]
        [Required]
        public string OrgName { get; set; }

        public ObservableCollection<Warehouse> warehouses { get; set; }
            = new ObservableCollection<Warehouse>();

        public Organization() { }

        public Organization(string name)
        {
            OrgName = name;
        }
    }
}

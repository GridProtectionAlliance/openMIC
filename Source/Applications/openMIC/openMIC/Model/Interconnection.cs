using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace openMIC.Model
{
    [Table("Interconnection")]
    public class Interconnection
    {
        public int ID
        {
            get;
            set;
        }

        [Required]
        [StringLength(50)]
        public string Acronym
        {
            get;
            set;
        }

        [Required]
        [StringLength(100)]
        public string Name
        {
            get;
            set;
        }

        public int? LoadOrder
        {
            get;
            set;
        }
    }
}
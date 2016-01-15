using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace openMIC.Model
{
    [Table("Protocol")]
    public class Protocol
    {
        public int ID
        {
            get;
            set;
        }

        [Required]
        [StringLength(200)]
        public string Acronym
        {
            get;
            set;
        }

        [Required]
        [StringLength(200)]
        public string Name
        {
            get;
            set;
        }

        [Required]
        [StringLength(200)]
        public string Type
        {
            get;
            set;
        }

        [Required]
        [StringLength(200)]
        public string Category
        {
            get;
            set;
        }

        [Required]
        [StringLength(1024)]
        public string AssemblyName
        {
            get;
            set;
        }

        [Required]
        [StringLength(200)]
        public string TypeName
        {
            get;
            set;
        }

        public int LoadOrder
        {
            get;
            set;
        }
    }
}
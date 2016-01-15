using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace openMIC.Model
{
    [Table("SignalType")]
    public class SignalType
    {
        public int ID
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
        [StringLength(4)]
        public string Acronym
        {
            get;
            set;
        }

        [Required]
        [StringLength(2)]
        public string Suffix
        {
            get;
            set;
        }

        [Required]
        [StringLength(2)]
        public string Abbreviation
        {
            get;
            set;
        }

        [Required]
        [StringLength(200)]
        public string LongAcronym
        {
            get;
            set;
        }

        [Required]
        [StringLength(10)]
        public string Source
        {
            get;
            set;
        }

        [StringLength(10)]
        public string EngineeringUnits
        {
            get;
            set;
        }
    }
}
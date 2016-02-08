using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace openMIC.Model
{
    [Table("Vendor")]
    public class Vendor
    {
        [PrimaryKey(true)]
        public int ID
        {
            get;
            set;
        }

        [Required]
        [StringLength(200)]
        [RegularExpression("^[A-Z0-9\\-!_\\.@#\\$]+$", ErrorMessage = "Only upper case letters, numbers, '!', '-', '@', '#', '_' , '.'and '$' are allowed.")]
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

        [StringLength(200)]
        public string PhoneNumber
        {
            get;
            set;
        }

        [StringLength(200)]
        public string ContactEmail
        {
            get;
            set;
        }

        public string URL
        {
            get;
            set;
        }

        public DateTime CreatedOn
        {
            get;
            set;
        }

        [Required]
        [StringLength(200)]
        public string CreatedBy
        {
            get;
            set;
        }

        public DateTime UpdatedOn
        {
            get;
            set;
        }

        [Required]
        [StringLength(200)]
        public string UpdatedBy
        {
            get;
            set;
        }
    }
}
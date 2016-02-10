using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace openMIC.Model
{
    [Table("ConnectionProfileTask")]
    public class ConnectionProfileTask
    {
        [PrimaryKey(true)]
        public int ID
        {
            get;
            set;
        }

        public int ConnectionProfileID
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

        public string Settings
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
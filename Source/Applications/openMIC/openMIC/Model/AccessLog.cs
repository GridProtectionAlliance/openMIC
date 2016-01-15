using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace openMIC.Model
{
    [Table("AccessLog")]
    public class AccessLog
    {
        public int ID
        {
            get;
            set;
        }

        [Required]
        [StringLength(200)]
        public string UserName
        {
            get;
            set;
        }

        public bool AccessGranted
        {
            get;
            set;
        }

        public DateTime CreatedOn
        {
            get;
            set;
        }
    }
}
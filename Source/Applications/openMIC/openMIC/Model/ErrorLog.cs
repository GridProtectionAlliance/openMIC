using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace openMIC.Model
{
    [Table("ErrorLog")]
    public class ErrorLog
    {
        public int ID
        {
            get;
            set;
        }

        [Required]
        [StringLength(200)]
        public string Source
        {
            get;
            set;
        }

        public string Type
        {
            get;
            set;
        }

        [Required]
        public string Message
        {
            get;
            set;
        }

        public string Detail
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
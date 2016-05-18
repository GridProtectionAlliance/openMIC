using System;
using System.ComponentModel.DataAnnotations;
using GSF.Data.Model;

namespace openMIC.Model
{
    public class VendorDevice
    {
        [PrimaryKey(true)]
        public int ID
        {
            get;
            set;
        }

        public int VendorID
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

        public string Description
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
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace openMIC.Model
{
    [Table("Node")]
    public class Node
    {
        public Guid ID
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

        public int? CompanyID
        {
            get;
            set;
        }

        public decimal? Longitude
        {
            get;
            set;
        }

        public decimal? Latitude
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string ImagePath
        {
            get;
            set;
        }

        public string Settings
        {
            get;
            set;
        }

        [Required]
        [StringLength(200)]
        public string MenuType
        {
            get;
            set;
        }

        [Required]
        public string MenuData
        {
            get;
            set;
        }

        public bool Master
        {
            get;
            set;
        }

        public int LoadOrder
        {
            get;
            set;
        }

        public bool Enabled
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
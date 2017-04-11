using System;
using System.ComponentModel.DataAnnotations;
using GSF.ComponentModel.DataAnnotations;
using GSF.Data.Model;

namespace openMIC.Model
{
    /// <summary>
    /// Model for Node table.
    /// </summary>
    [TableName("Node")]
    [PrimaryLabel("Name")]
    public class Node
    {
        /// <summary>
        /// Unique node ID field.
        /// </summary>
        [PrimaryKey(true)]
        public Guid ID { get; set; }

        /// <summary>
        /// Name field.
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        /// <summary>
        /// Description field.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Enabled field.
        /// </summary>
        [InitialValueScript("true")]
        public bool Enabled { get; set; }

        /// <summary>
        /// Created on field.
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Created by field.
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Updated on field.
        /// </summary>
        public DateTime UpdatedOn { get; set; }

        /// <summary>
        /// Updated by field.
        /// </summary>
        public string UpdatedBy { get; set; }
    }
}
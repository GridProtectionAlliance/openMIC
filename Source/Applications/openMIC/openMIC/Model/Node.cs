using System;
using System.ComponentModel.DataAnnotations;
using GSF.ComponentModel;
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

        [DefaultValueExpression("DateTime.UtcNow")]
        public DateTime CreatedOn
        {
            get;
            set;
        }

        [Required]
        [StringLength(200)]
        [DefaultValueExpression("UserInfo.CurrentUserID")]
        public string CreatedBy
        {
            get;
            set;
        }

        [DefaultValueExpression("this.CreatedOn", EvaluationOrder = 1)]
        [UpdateValueExpression("DateTime.UtcNow")]
        public DateTime UpdatedOn
        {
            get;
            set;
        }

        [Required]
        [StringLength(200)]
        [DefaultValueExpression("this.CreatedBy", EvaluationOrder = 1)]
        [UpdateValueExpression("UserInfo.CurrentUserID")]
        public string UpdatedBy
        {
            get;
            set;
        }
    }
}
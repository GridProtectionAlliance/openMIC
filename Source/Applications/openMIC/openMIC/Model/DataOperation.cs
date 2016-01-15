using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace openMIC.Model
{
    [Table("DataOperation")]
    public class DataOperation
    {
        public Guid? NodeID
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        [Key]
        [Column(Order = 0)]
        public string AssemblyName
        {
            get;
            set;
        }

        [Key]
        [Column(Order = 1)]
        public string TypeName
        {
            get;
            set;
        }

        [Key]
        [Column(Order = 2)]
        [StringLength(200)]
        public string MethodName
        {
            get;
            set;
        }

        public string Arguments
        {
            get;
            set;
        }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int LoadOrder
        {
            get;
            set;
        }

        [Key]
        [Column(Order = 4)]
        public bool Enabled
        {
            get;
            set;
        }
    }
}
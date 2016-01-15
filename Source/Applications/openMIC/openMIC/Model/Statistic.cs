using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace openMIC.Model
{
    [Table("Statistic")]
    public class Statistic
    {
        public int ID
        {
            get;
            set;
        }

        [Required]
        [StringLength(20)]
        public string Source
        {
            get;
            set;
        }

        public int SignalIndex
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

        [Required]
        public string AssemblyName
        {
            get;
            set;
        }

        [Required]
        public string TypeName
        {
            get;
            set;
        }

        [Required]
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

        public bool Enabled
        {
            get;
            set;
        }

        [StringLength(200)]
        public string DataType
        {
            get;
            set;
        }

        [StringLength(200)]
        public string DisplayFormat
        {
            get;
            set;
        }

        public bool IsConnectedState
        {
            get;
            set;
        }

        public int LoadOrder
        {
            get;
            set;
        }
    }
}
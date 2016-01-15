using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace openMIC.Model
{
    [Table("AlarmLog")]
    public class AlarmLog
    {
        public int ID
        {
            get;
            set;
        }

        public Guid SignalID
        {
            get;
            set;
        }

        public int? PreviousState
        {
            get;
            set;
        }

        public int? NewState
        {
            get;
            set;
        }

        public long Ticks
        {
            get;
            set;
        }

        [Column(TypeName = "datetime2")]
        public DateTime Timestamp
        {
            get;
            set;
        }

        public double Value
        {
            get;
            set;
        }
    }
}
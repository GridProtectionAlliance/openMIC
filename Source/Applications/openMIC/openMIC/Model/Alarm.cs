using System;
using System.ComponentModel.DataAnnotations;
using GSF.ComponentModel;

namespace openMIC.Model
{
    public class Alarm
    {
        public Guid NodeID
        {
            get;
            set;
        }

        public int ID
        {
            get;
            set;
        }

        [Required]
        [StringLength(200)]
        public string TagName
        {
            get;
            set;
        }

        public Guid SignalID
        {
            get;
            set;
        }

        public Guid? AssociatedMeasurementID
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public int Severity
        {
            get;
            set;
        }

        public int Operation
        {
            get;
            set;
        }

        public double? SetPoint
        {
            get;
            set;
        }

        public double? Tolerance
        {
            get;
            set;
        }

        public double? Delay
        {
            get;
            set;
        }

        public double? Hysteresis
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
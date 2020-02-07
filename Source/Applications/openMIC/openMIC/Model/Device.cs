using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GSF.ComponentModel;
using GSF.ComponentModel.DataAnnotations;
using GSF.Data.Model;

namespace openMIC.Model
{
    [PrimaryLabel("Acronym")]
    public class Device
    {
        [DefaultValueExpression("Global.NodeID")]
        public Guid NodeID
        {
            get;
            set;
        }

        [Label("Local Device ID")]
        [PrimaryKey(true)]
        public int ID
        {
            get;
            set;
        }

        public int? ParentID
        {
            get;
            set;
        }

        [Label("Unique Device ID")]
        [DefaultValueExpression("Guid.NewGuid()")]
        public Guid UniqueID
        {
            get;
            set;
        }

        [Required]
        [StringLength(200)]
        [AcronymValidation]
        [Searchable]
        public string Acronym
        {
            get;
            set;
        }

        [StringLength(200)]
        public string Name
        {
            get;
            set;
        }

        [Label("Target Folder Name (leave blank to use Acronym)")]
        [StringLength(200)]
        public string OriginalSource
        {
            get;
            set;
        }

        [Label("Is Concentrator")]
        public bool IsConcentrator
        {
            get;
            set;
        }

        [Required]
        [Label("Company")]
        [DefaultValueExpression("Connection.ExecuteScalar(typeof(int), (object)null, 'SELECT ID FROM Company WHERE Acronym = {0}', Global.CompanyAcronym)", Cached = true)]
        public int? CompanyID
        {
            get;
            set;
        }

        [Label("Historian")]
        public int? HistorianID
        {
            get;
            set;
        }

        [Label("Access ID")]
        public int AccessID
        {
            get;
            set;
        }

        [Label("Vendor Device")]
        public int? VendorDeviceID
        {
            get;
            set;
        }

        [Label("Protocol")]
        public int? ProtocolID
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

        [Label("Interconnection")]
        [InitialValueScript("1")]
        public int? InterconnectionID
        {
            get;
            set;
        }

        [Label("Connection String")]
        public string ConnectionString
        {
            get;
            set;
        }

        [StringLength(200)]
        public string TimeZone
        {
            get;
            set;
        }

        [Label("Frames Per Second")]
        [DefaultValue(1)] // For openMIC, we are typically dealing with slower moving data
        public int? FramesPerSecond
        {
            get;
            set;
        }

        public long TimeAdjustmentTicks
        {
            get;
            set;
        }

        public double DataLossInterval
        {
            get;
            set;
        }

        public int AllowedParsingExceptions
        {
            get;
            set;
        }

        public double ParsingExceptionWindow
        {
            get;
            set;
        }

        public double DelayedConnectionInterval
        {
            get;
            set;
        }

        public bool AllowUseOfCachedConfiguration
        {
            get;
            set;
        }

        public bool AutoStartDataParsingSequence
        {
            get;
            set;
        }

        public bool SkipDisableRealTimeData
        {
            get;
            set;
        }

        [DefaultValue(5000)] // For openMIC, we are typically dealing with slower moving data
        public int MeasurementReportingInterval
        {
            get;
            set;
        }

        [Label("Connect On Demand")]
        public bool ConnectOnDemand
        {
            get;
            set;
        }

        [Label("Contacts")]
        public string ContactList
        {
            get;
            set;
        }

        public int? MeasuredLines
        {
            get;
            set;
        }

        public int LoadOrder
        {
            get;
            set;
        }

        [DefaultValue(true)]
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
        [StringLength(50)]
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
        [StringLength(50)]
        [DefaultValueExpression("this.CreatedBy", EvaluationOrder = 1)]
        [UpdateValueExpression("UserInfo.CurrentUserID")]
        public string UpdatedBy
        {
            get;
            set;
        }
    }
}
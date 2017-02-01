using System;
using GSF.Data.Model;

namespace BenDownloader
{
    [PrimaryLabel("Acronym")]
    public class Device
    {
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
        public Guid UniqueID
        {
            get;
            set;
        }

        public string Acronym
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        [Label("Folder Name")]
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

        [Label("Company")]
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
        [InitialValue("1")]
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

        public string TimeZone
        {
            get;
            set;
        }

        [Label("Frames Per Second")]
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

        public string UpdatedBy
        {
            get;
            set;
        }
    }
}
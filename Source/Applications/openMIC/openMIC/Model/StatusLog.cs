using System;
using System.Data;
using GSF.ComponentModel;
using GSF.Data;
using GSF.Data.Model;

namespace openMIC.Model
{
    [TableName("StatusLog")]
    [AmendExpression("WITH (NOLOCK)", DatabaseType.SQLServer)]
    [AmendExpression("TOP 1", DatabaseType.SQLServer,
        StatementTypes = StatementTypes.SelectSet,
        TargetExpression = TargetExpression.FieldList,
        AffixPosition = AffixPosition.Prefix)]
    public class StatusLog
    {
        [PrimaryKey(true)]
        public int ID { get; set; }

        public int DeviceID { get; set; }

        public int? LastDownloadedFileID { get; set; }

        public string LastOutcome { get; set; }

        [FieldDataType(DbType.DateTime2)]
        public DateTime? LastRun { get; set; }

        [FieldDataType(DbType.DateTime2)]
        public DateTime? LastFailure { get; set; }

        public string LastErrorMessage { get; set; }

        [FieldDataType(DbType.DateTime2)]
        public DateTime? LastDownloadStartTime { get; set; }

        [FieldDataType(DbType.DateTime2)]
        public DateTime? LastDownloadEndTime { get; set; }

        public int? LastDownloadFileCount { get; set; }
    }
}
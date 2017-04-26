using System;
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

        [DefaultValueExpression("DateTime.UtcNow")]
        public DateTime? LastSuccess { get; set; }

        [DefaultValueExpression("DateTime.UtcNow")]
        public DateTime? LastFailure { get; set; }

        public string Message { get; set; }

        public string LastFile { get; set; }

        public DateTime? FileDownloadTimestamp { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GSF.Data.Model;

namespace BenDownloader
{
    public class StatusLog
    {
        [PrimaryKey (true)]
        public int ID { get; set; }
        public int DeviceID { get; set; }
        public DateTime? LastSuccess { get; set; }
        public DateTime? LastFailure { get; set; }
        public string Message { get; set; }
        public string LastFile { get; set; }
    }
}
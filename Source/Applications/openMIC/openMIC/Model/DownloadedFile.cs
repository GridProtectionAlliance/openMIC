using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GSF.Data.Model;

namespace openMIC.Model
{
    public class DownloadedFile
    {
        [PrimaryKey(true)]
        public int ID { get; set; }
	    public int DeviceID { get; set; }
        [UseEscapedName]
        public string File { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime CreationTime { get; set; }
        public int FileSize { get; set; }
    }
}
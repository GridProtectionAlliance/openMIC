using System;
using GSF.Data.Model;

namespace openMIC.Model
{
    public class DownloadedFile
    {
        [PrimaryKey(true)]
        public int ID { get; set; }

        public int DeviceID { get; set; }

        public string FilePath { get; set; }

        public DateTime Timestamp { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime LastWriteTime { get; set; }

        public DateTime LastAccessTime { get; set; }

        public int FileSize { get; set; }
    }
}
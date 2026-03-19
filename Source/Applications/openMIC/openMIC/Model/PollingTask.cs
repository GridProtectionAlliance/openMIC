using System;
using GSF.Data.Model;

namespace openMIC.Model;

public class PollingTask
{
    [PrimaryKey(true)]
    public int ID { get; set; }
    public Guid RuntimeID { get; set; }
    public string NodeID { get; set; }
    public string DownloaderName { get; set; }
    public string Task { get; set; }
}
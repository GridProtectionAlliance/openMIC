using System;

namespace openMIC.Model;

public class StatusLight
{
    public string DeviceAcronym { get; set; }
    public DateTime Timestamp { get; set; }
    public bool GoodData { get; set; }
}
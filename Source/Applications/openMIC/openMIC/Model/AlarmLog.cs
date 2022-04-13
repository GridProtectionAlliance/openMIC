using System;
using GSF.ComponentModel;

namespace openMIC.Model;

public class AlarmLog
{
    public int ID
    {
        get;
        set;
    }

    public Guid SignalID
    {
        get;
        set;
    }

    public int? PreviousState
    {
        get;
        set;
    }

    public int? NewState
    {
        get;
        set;
    }

    public long Ticks
    {
        get;
        set;
    }

    [DefaultValueExpression("DateTime.UtcNow")]
    public DateTime Timestamp
    {
        get;
        set;
    }

    public double Value
    {
        get;
        set;
    }
}
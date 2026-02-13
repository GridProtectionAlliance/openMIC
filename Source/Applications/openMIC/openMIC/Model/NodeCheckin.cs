using System;
using System.ComponentModel;
using GSF.Data.Model;

namespace openMIC.Model;

/// <summary>
/// Represents a checkin with a polling node in the openMIC cluster.
/// </summary>
public class NodeCheckin
{
    [PrimaryKey(true)]
    public int ID
    {
        get;
        set;
    }

    public string Url
    {
        get;
        set;
    }

    [FieldDataType(System.Data.DbType.DateTime2)]
    public DateTime LastCheckin
    {
        get;
        set;
    }

    public string FailureReason
    { 
        get;
        set;
    }

    [DefaultValue(0)]
    public int TasksQueued
    {
        get;
        set;
    }
}
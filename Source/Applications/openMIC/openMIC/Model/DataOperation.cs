using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace openMIC.Model;

public class DataOperation
{
    public Guid? NodeID
    {
        get;
        set;
    }

    public string Description
    {
        get;
        set;
    }

    public string AssemblyName
    {
        get;
        set;
    }

    public string TypeName
    {
        get;
        set;
    }

    [StringLength(200)]
    public string MethodName
    {
        get;
        set;
    }

    public string Arguments
    {
        get;
        set;
    }

    public int LoadOrder
    {
        get;
        set;
    }

    [DefaultValue(true)]
    public bool Enabled
    {
        get;
        set;
    }
}
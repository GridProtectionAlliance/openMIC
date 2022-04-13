using System;
using System.ComponentModel;

namespace openMIC.Model;

public class IGridDevice
{
    public int DeviceID
    {
        get;
        set;
    }

    public string Acronym
    {
        get;
        set;
    }

    public string SerialNumber
    {
        get;
        set;
    }

    public string Name
    {
        get;
        set;
    }

    public string Description
    {
        get;
        set;
    }

    public string ModelNumber
    {
        get;
        set;
    }

    public decimal Longitude
    {
        get;
        set;
    }

    public decimal Latitude
    {
        get;
        set;
    }

    public bool Selected
    {
        get;
        set;
    }
}
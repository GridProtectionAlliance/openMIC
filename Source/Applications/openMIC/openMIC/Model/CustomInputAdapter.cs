using System;
using System.ComponentModel.DataAnnotations;
using GSF.ComponentModel;

namespace openMIC.Model;

public class CustomInputAdapter
{
    public Guid NodeID
    {
        get;
        set;
    }

    public int ID
    {
        get;
        set;
    }

    [Required]
    [StringLength(200)]
    public string AdapterName
    {
        get;
        set;
    }

    [Required]
    public string AssemblyName
    {
        get;
        set;
    }

    [Required]
    public string TypeName
    {
        get;
        set;
    }

    public string ConnectionString
    {
        get;
        set;
    }

    public int LoadOrder
    {
        get;
        set;
    }

    public bool Enabled
    {
        get;
        set;
    }

    [DefaultValueExpression("DateTime.UtcNow")]
    public DateTime CreatedOn
    {
        get;
        set;
    }

    [Required]
    [StringLength(200)]
    [DefaultValueExpression("UserInfo.CurrentUserID")]
    public string CreatedBy
    {
        get;
        set;
    }

    [DefaultValueExpression("this.CreatedOn", EvaluationOrder = 1)]
    [UpdateValueExpression("DateTime.UtcNow")]
    public DateTime UpdatedOn
    {
        get;
        set;
    }

    [Required]
    [StringLength(200)]
    [DefaultValueExpression("this.CreatedBy", EvaluationOrder = 1)]
    [UpdateValueExpression("UserInfo.CurrentUserID")]
    public string UpdatedBy
    {
        get;
        set;
    }
}
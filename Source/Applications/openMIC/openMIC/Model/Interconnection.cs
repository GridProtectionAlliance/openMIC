using System.ComponentModel.DataAnnotations;
using GSF.Data.Model;

namespace openMIC.Model;

public class Interconnection
{
    [PrimaryKey(true)]
    public int ID
    {
        get;
        set;
    }

    [Required]
    [StringLength(50)]
    public string Acronym
    {
        get;
        set;
    }

    [Required]
    [StringLength(100)]
    public string Name
    {
        get;
        set;
    }

    public int? LoadOrder
    {
        get;
        set;
    }
}
using Microsoft.LightSwitch;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System;

namespace LightSwitchApplication
{
    public partial class Alarm
    {
        partial void TagName_Validate(EntityValidationResultsBuilder results)
        {
            openMICDataService.ValidateAcronym(TagName, results, "TagName");
        }
    }
}
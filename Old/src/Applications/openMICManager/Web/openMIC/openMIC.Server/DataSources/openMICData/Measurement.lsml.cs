using Microsoft.LightSwitch;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System;

namespace LightSwitchApplication
{
    public partial class Measurement
    {
        partial void PointTag_Validate(EntityValidationResultsBuilder results)
        {
            openMICDataService.ValidateAcronym(PointTag, results, "PointTag");
        }

        partial void AlternateTag_Validate(EntityValidationResultsBuilder results)
        {
            openMICDataService.ValidateAcronym(PointTag, results, "AlternateTag", true);
        }
    }
}
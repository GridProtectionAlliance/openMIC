using Microsoft.LightSwitch;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System;

namespace LightSwitchApplication
{
    public partial class Company
    {
        partial void Acronym_Validate(EntityValidationResultsBuilder results)
        {
            openMICDataService.ValidateAcronym(Acronym, results);
        }

        partial void MapAcronym_Validate(EntityValidationResultsBuilder results)
        {
            openMICDataService.ValidateAcronym(MapAcronym, results, "MapAcronym");
        }
    }
}
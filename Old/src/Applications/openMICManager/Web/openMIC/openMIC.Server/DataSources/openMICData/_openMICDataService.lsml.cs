using System.Linq;
using Microsoft.LightSwitch;
using System;
using System.Text.RegularExpressions;
using GSF;

namespace LightSwitchApplication
{
    public partial class openMICDataService
    {
        const string ValidAcronymExpression = @"^[A-Z0-9\-!_\.@#\$]+$";
        private static readonly Regex AcronymExpression = new Regex(ValidAcronymExpression, RegexOptions.Compiled);

        public static void ValidateAcronym(string acronym, EntityValidationResultsBuilder results, string fieldName = "Acronym", bool emptyIsValid = false)
        {
            if (!emptyIsValid && string.IsNullOrEmpty(acronym))
            {
                results.AddPropertyError($"{fieldName} cannot be empty");
                return;
            }

            if (!acronym.IsAllUpper())
                results.AddPropertyResult($"{fieldName} is not all upper-case", ValidationSeverity.Warning);

            if (!AcronymExpression.IsMatch(acronym))
                results.AddPropertyError($"{fieldName} contains invalid symbols:\nOnly letters, numbers, '!', '-', '@', '#', '_', '.' and '$' are allowed");
        }

        partial void Devices_Validate(Device entity, EntitySetValidationResultsBuilder results)
        {
            if (!string.IsNullOrEmpty(entity.Acronym))
            {
                if (string.IsNullOrEmpty(entity.Name))
                    entity.Name = entity.Acronym.ToTitleCase();
            }
        }

        partial void Companies_Validate(Company entity, EntitySetValidationResultsBuilder results)
        {
            if (!string.IsNullOrEmpty(entity.Acronym))
            {
                if (string.IsNullOrEmpty(entity.Name))
                    entity.Name = entity.Acronym.ToTitleCase();

                if (string.IsNullOrEmpty(entity.MapAcronym))
                    entity.MapAcronym = entity.Acronym.Substring(0, Math.Min(entity.Acronym.Length, 3));
            }
        }

        partial void SaveChanges_CanExecute(ref bool result)
        {
            result = true;
        }
    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.ObjectModel;
using System.Text;

namespace DataHubFunctions
{
    [TestClass]
    public class SectionBuilders
    {
        [TestMethod]
        public void BuildTimeZones()
        {
            StringBuilder timeZones = new StringBuilder();

            timeZones.AppendLine("<PROPDEF ORDER=\"0\" NAME=\"Time_Zone_Region\" ALIAS=\"Time zone (automatic daylight savings time)\" TYPE=\"8\" DEFAULT=\"0\" RO=\"0\" MAP=\".Time_Zone_Region\">");
            timeZones.AppendLine("  <PARAM TYPE=\"4\">");
            timeZones.AppendLine("    <LIST>");

            foreach (TimeZoneInfo info in TimeZoneInfo.GetSystemTimeZones())
            {
                timeZones.AppendLine($"      <ITEM NAME=\"{info.DisplayName}\" VALUE=\"{info.StandardName}\">");
            }

            timeZones.AppendLine("    </LIST>");
            timeZones.AppendLine("  </PARAM>");
            timeZones.AppendLine("</PROPDEF>");

            Console.Write(timeZones.ToString());
        }
    }
}

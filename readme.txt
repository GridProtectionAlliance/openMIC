{{https://raw.githubusercontent.com/GridProtectionAlliance/openMIC/master/Readme%20files/openMIC%20logo.png|openMIC}}

====== openMIC ======

Meter Information Collector

===== What Does it Do? =====

  * Interrogates multiple vendors' DFRs
  * Via IP
  * Via Modem (RAS & FTP)
  * Schedules calls & organizes returned data
  * Admins can define multiple interrogation schedules - including as-fast-as-possible
  * Output location can be specified on a device-by-device basis
  * Logs and reports meter problems
  * Includes a mobile-ready web app to view interrogation status
  * A tool for configuration
  * Shows interrogation history / status of each DFR

===== Components =====

  * Configuration data base (SQL server)
  * The openMIC service (//the automation engine//)
  * GPA's new Web Solutions Framework
  * The Grid Solutions Framework
  * An implementation of the WSF - The openMIC Web Management Interface
  * The GSF Admin console
  * File server / file share to receive files
  * Installer / Installation Package

{{https://raw.githubusercontent.com/GridProtectionAlliance/openMIC/master/Readme%20files/GPA%20WSF.png|GPA WSF - A New Technology Layer}}

**Where It Fits In:**\\
{{https://raw.githubusercontent.com/GridProtectionAlliance/openMIC/master/Readme%20files/Architecture.png|Architecture}}

====== Documentation ======

openMIC is a new project so documentation is limited, but you can check out some [[#screenshots|screenshots]] below.

====== Deployment ======

  - Make sure your system meets all the requirements below.

  * Choose a [[#downloads|download]] below.
  * Unzip if necessary.
  * Run "Setup.exe".
  * Follow the wizard.
  * Enjoy.

===== Requirements =====

  * .NET 4.6 or higher.
  * 64-bit Windows 7 or newer.
  * Database management system such as:
  * SQL Server (Recommended)
  * MySQL
  * Oracle
  * SQLite (Not recommended for production use) - included.

===== Downloads =====

  * Download the lastest stable release [[https://github.com/GridProtectionAlliance/openMIC/releases|here]].
  * Download the nightly build [[http://www.gridprotectionalliance.org/nightlybuilds/openMIC/Beta/Setup.zip|here]].

====== Contributing ======

If you would like to contribute please:

  - Read our [[https://www.gridprotectionalliance.org/docs/GPA_Coding_Guidelines_2011_03.pdf|styleguide]].

  * Fork the repository.
  * Code like a boss.
  * Create a pull request.

====== Screenshots ======

===== Main Screen =====

{{https://raw.githubusercontent.com/GridProtectionAlliance/openMIC/master/Readme%20files/Main%20Screen.png|Main Screen}}

====== Status ======

{{https://raw.githubusercontent.com/GridProtectionAlliance/openMIC/master/Readme%20files/openMIC%20Status.png|Status}}

===== Manage Devices and Schedules =====

{{https://raw.githubusercontent.com/GridProtectionAlliance/openMIC/master/Readme%20files/Manage%20Devices%20and%20Schedules.png|Manage Devices and Schedules}}

===== Edit Device Connection Info =====

{{https://raw.githubusercontent.com/GridProtectionAlliance/openMIC/master/Readme%20files/Edit%20Device%20Connection%20Info.png|Edit Device Connection Info}}

===== Regular Expression Field Validation =====

{{https://raw.githubusercontent.com/GridProtectionAlliance/openMIC/master/Readme%20files/Regular%20Expression%20Field%20Validation.png|Regular Expression Field Validation}}

===== Cron Schedule Syntax Helper =====

{{https://raw.githubusercontent.com/GridProtectionAlliance/openMIC/master/Readme%20files/Cron%20Schedule%20Syntax%20Helper.png|Cron Schedule Syntax Helper}}

===== Manage Connection Profiles =====

{{https://raw.githubusercontent.com/GridProtectionAlliance/openMIC/master/Readme%20files/Manage%20Connection%20Profiles.png|Manage Connection Profiles}}

===== Manage Connection Profile Task =====

{{https://raw.githubusercontent.com/GridProtectionAlliance/openMIC/master/Readme%20files/Manage%20Connection%20Profile%20Task.png|Manage Connection Profile Tasks}}

===== Edit Connection Profile Task =====

{{https://raw.githubusercontent.com/GridProtectionAlliance/openMIC/master/Readme%20files/Edit%20Connection%20Profile%20Task.png|Edit Connection Profile Task}}

===== Monitor the Service via Web =====

{{https://raw.githubusercontent.com/GridProtectionAlliance/openMIC/master/Readme%20files/Monitor%20the%20Service%20via%20Web.png|Monitor the Service via Web}}

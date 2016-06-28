![openMIC](https://raw.githubusercontent.com/GridProtectionAlliance/openMIC/master/Source/Documentation/Readme%20files/openMIC%20logo.png)

# openMIC

Meter Information Collector

## What Does it Do?

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

## Components

* Configuration data base (SQL server)
* The openMIC service (*the automation engine*)
  * GPA's new Web Solutions Framework
  * The Grid Solutions Framework
* An implementation of the WSF - The openMIC Web Management Interface
* The GSF Admin console
* File server / file share to receive files
* Installer / Installation Package

![GPA WSF - A New Technology Layer](https://raw.githubusercontent.com/GridProtectionAlliance/openMIC/master/Source/Documentation/Readme%20files/GPA%20WSF.png)


**Where It Fits In:**
![Architecture](https://raw.githubusercontent.com/GridProtectionAlliance/openMIC/master/Source/Documentation/Readme%20files/Architecture.png)


# Documentation and Support

* openMIC is a new project so documentation is limited, but you can check out some [screenshots](#screenshots) below.
* Get in contact with our development team on our [discussion board](http://discussions.gridprotectionalliance.org/c/gpa-products/openmic).
* Check out the [wiki](https://gridprotectionalliance.org/wiki/doku.php?id=openmic:overview).

# Deployment

1. Make sure your system meets all the requirements below.
* Choose a [download](#downloads) below.
* Unzip if necessary.
* Run "Setup.exe".
* Follow the wizard.
* Enjoy.

## Requirements

* .NET 4.6 or higher.
* 64-bit Windows 7 or newer.
* Database management system such as:
  * SQL Server (Recommended)
  * MySQL
  * Oracle
  * SQLite (Not recommended for production use) - included.

## Downloads
* Download the lastest stable release [here](https://github.com/GridProtectionAlliance/openMIC/releases).
* Download the nightly build [here](http://www.gridprotectionalliance.org/nightlybuilds/openMIC/Beta/Setup.zip).

# Contributing
If you would like to contribute please:

1. Read our [styleguide](https://www.gridprotectionalliance.org/docs/GPA_Coding_Guidelines_2011_03.pdf).
* Fork the repository.
* Code like a boss.
* Create a pull request.

# Screenshots

## Main Screen

![Main Screen](https://raw.githubusercontent.com/GridProtectionAlliance/openMIC/master/Source/Documentation/Readme%20files/Main%20Screen.png)

# Status

![Status](https://raw.githubusercontent.com/GridProtectionAlliance/openMIC/master/Source/Documentation/Readme%20files/openMIC%20Status.png)

## Manage Devices and Schedules

![Manage Devices and Schedules](https://raw.githubusercontent.com/GridProtectionAlliance/openMIC/master/Source/Documentation/Readme%20files/Manage%20Devices%20and%20Schedules.png)

## Edit Device Connection Info

![Edit Device Connection Info](https://raw.githubusercontent.com/GridProtectionAlliance/openMIC/master/Source/Documentation/Readme%20files/Edit%20Device%20Connection%20Info.png)

## Regular Expression Field Validation

![Regular Expression Field Validation](https://raw.githubusercontent.com/GridProtectionAlliance/openMIC/master/Source/Documentation/Readme%20files/Regular%20Expression%20Field%20Validation.png)

## Cron Schedule Syntax Helper

![Cron Schedule Syntax Helper](https://raw.githubusercontent.com/GridProtectionAlliance/openMIC/master/Source/Documentation/Readme%20files/Cron%20Schedule%20Syntax%20Helper.png)

## Manage Connection Profiles

![Manage Connection Profiles](https://raw.githubusercontent.com/GridProtectionAlliance/openMIC/master/Source/Documentation/Readme%20files/Manage%20Connection%20Profiles.png)

## Manage Connection Profile Task

![Manage Connection Profile Tasks](https://raw.githubusercontent.com/GridProtectionAlliance/openMIC/master/Source/Documentation/Readme%20files/Manage%20Connection%20Profile%20Task.png)

## Edit Connection Profile Task

![Edit Connection Profile Task](https://raw.githubusercontent.com/GridProtectionAlliance/openMIC/master/Source/Documentation/Readme%20files/Edit%20Connection%20Profile%20Task.png)

## Monitor the Service via Web

![Monitor the Service via Web](https://raw.githubusercontent.com/GridProtectionAlliance/openMIC/master/Source/Documentation/Readme%20files/Monitor%20the%20Service%20via%20Web.png)

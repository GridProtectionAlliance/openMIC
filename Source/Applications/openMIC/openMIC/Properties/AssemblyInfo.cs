using System.Reflection;
using System.Runtime.InteropServices;

// Assembly identity attributes.
[assembly: AssemblyVersion("2.0.122.0")]

// Informational attributes.
[assembly: AssemblyCompany("Grid Protection Alliance")]
[assembly: AssemblyCopyright("Copyright © 2016-2021.  All Rights Reserved.")]
[assembly: AssemblyProduct("openMIC")]

// Assembly manifest attributes.
#if DEBUG
[assembly: AssemblyConfiguration("Debug Build")]
#else
[assembly: AssemblyConfiguration("Release Build")]
#endif
[assembly: AssemblyDescription("Windows service that hosts input, action and output adapters.")]
[assembly: AssemblyTitle("openMIC Iaon Host")]

// Other configuration attributes.
[assembly: ComVisible(false)]
[assembly: Guid("a14ea627-5188-48fd-b32f-d649a038b2bc")]

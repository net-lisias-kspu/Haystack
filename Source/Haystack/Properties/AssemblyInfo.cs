﻿using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Resources;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Haystack /L Unleashed")]
[assembly: AssemblyDescription("Plugin for Kerbal Space Program which helps to easily switch vessels and targets.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(Haystack.LegalMamboJambo.Company)]
[assembly: AssemblyProduct(Haystack.LegalMamboJambo.Product)]
[assembly: AssemblyCopyright(Haystack.LegalMamboJambo.Copyright)]
[assembly: AssemblyTrademark(Haystack.LegalMamboJambo.Trademark)]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion(Haystack.Version.Number)]
[assembly: AssemblyFileVersion(Haystack.Version.Number)]
[assembly: NeutralResourcesLanguageAttribute("en")]

[assembly: KSPAssembly("Haystack", Haystack.Version.major, Haystack.Version.minor)]
[assembly: KSPAssemblyDependency("KSPe", 2, 4)]
[assembly: KSPAssemblyDependency("KSPe.UI", 2, 4)]

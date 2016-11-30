//All contants from Version.cs are visible for test projects. It is normal to disable this warning
#pragma warning disable 0436

using System.Reflection;

[assembly: AssemblyVersion(Constants.ASSEMBLY_VERSION)]
[assembly: AssemblyFileVersion(Constants.ASSEMBLY_VERSION)]
[assembly: AssemblyCompany(Constants.AUTHOR_NAME)]
[assembly: AssemblyProduct(Constants.DESCRIPTION)]
[assembly: AssemblyCopyright(Constants.COPYRIGHT)]

static class Constants
{
    internal const string ASSEMBLY_VERSION = "2.2.17.0";
    internal const string AUTHOR_NAME = "Flux Factory, Inc.";
    internal const string DESCRIPTION = "\"Flux SDK\" allows to create .Net add-ins for design & engineering programs";
    internal const string COPYRIGHT = "Copyright © Flux Factory, Inc. 2016";
}
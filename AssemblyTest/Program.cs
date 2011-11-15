using System;
using System.Reflection;

namespace AssemblyTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //var setup = new AppDomainSetup();
            //setup.ApplicationBase = @"C:\Documents and Settings\willcraftia\My Documents\Visual Studio 2010\Projects\WindowsGame8\AssemblyTestLib\bin\Debug";
            ////setup.ApplicationBase = @"C:\Documents and Settings\willcraftia\My Documents\Visual Studio 2010\Projects\WindowsGame8\AssemblyTestLib\";
            //setup.DisallowBindingRedirects = false;
            //setup.DisallowCodeDownload = false;
            //setup.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

            //var domain = AppDomain.CreateDomain("TestDomain", null, setup);
            ////var iconReaderService = domain.CreateInstanceAndUnwrap("Willcraftia.Win.Framework", "Willcraftia.Win.Framework.IconReaderService");
            ////var domain = AppDomain.CreateDomain("TestDomain");
            ////var iconReaderService = domain.CreateInstanceFromAndUnwrap(
            ////    @"C:\Documents and Settings\willcraftia\My Documents\Visual Studio 2010\Projects\WindowsGame8\AssemblyTestLib\bin\Debug\AssemblyTestLib.dll",
            ////    "AssemblyTestLib.Class1");
            //var iconReaderService = domain.CreateInstanceAndUnwrap("AssemblyTestLib", "AssemblyTestLib.Class1");
            //if (iconReaderService != null)
            //{
            //    Console.WriteLine("Good!");
            //}
            //else
            //{
            //    Console.WriteLine("Bad...");
            //}

            var assembly = Assembly.LoadFrom(@"C:\Documents and Settings\willcraftia\My Documents\Visual Studio 2010\Projects\WindowsGame8\AssemblyTestLib\bin\Debug\AssemblyTestLib.dll");
            var class1 = assembly.CreateInstance("AssemblyTestLib.Class1");

            //class1 = Activator.CreateInstance("AssemblyTestLib.Class1", "AssemblyTestLib.Class1");
        }
    }
}

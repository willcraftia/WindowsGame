#region Using

using System;
using System.Globalization;
using System.Reflection;

#endregion

namespace Willcraftia.Content.Studio.Models
{
    public sealed class AssemblyNameEdit
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Culture { get; set; }
        public string PublicKeyToken { get; set; }
        public ProcessorArchitecture ProcessorArchitecture { get; set; }

        public AssemblyNameEdit() { }

        public AssemblyNameEdit(AssemblyName assemblyName)
        {
            SetAssemblyName(assemblyName);
        }

        public void SetAssemblyName(AssemblyName assemblyName)
        {
            Name = assemblyName.Name;
            Version = assemblyName.Version.ToString();
            Culture = (assemblyName.CultureInfo == null) ? string.Empty : assemblyName.CultureInfo.Name;
            PublicKeyToken = ToHexString(assemblyName.GetPublicKeyToken());
            ProcessorArchitecture = assemblyName.ProcessorArchitecture;
        }

        public AssemblyName CreateAssemblyName()
        {
            var assemblyName = new AssemblyName(Name);
            assemblyName.Version = new Version(Version);
            assemblyName.CultureInfo = new CultureInfo(Culture);
            assemblyName.SetPublicKeyToken(FromHexString(PublicKeyToken));
            assemblyName.ProcessorArchitecture = ProcessorArchitecture;
            return assemblyName;
        }

        string ToHexString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", string.Empty).ToLower();
        }

        byte[] FromHexString(string hexString)
        {
            var bytes = new byte[hexString.Length / 2];
            int cursor = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(cursor, 2), 16);
                cursor += 2;
            }
            return bytes;
        }
    }
}

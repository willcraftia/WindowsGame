#region Using

using System;
using System.Globalization;
using System.Reflection;
using Willcraftia.Win.Framework;
using Willcraftia.Xna.Framework.Content.Build;

#endregion

namespace Willcraftia.Content.Studio.Models
{
    public sealed class ReferenceEdit : ObservableObject
    {
        string name;
        public string Name
        {
            get { return name; }
            private set
            {
                if (name == value) return;

                name = value;
                OnPropertyChanged("Name");
            }
        }

        string version;
        public string Version
        {
            get { return version; }
            private set
            {
                if (version == value) return;

                version = value;
                OnPropertyChanged("Version");
            }
        }

        string culture;
        public string Culture
        {
            get { return culture; }
            private set
            {
                if (culture == value) return;

                culture = value;
                OnPropertyChanged("Culture");
            }
        }

        string publicKeyToken;
        public string PublicKeyToken
        {
            get { return publicKeyToken; }
            private set
            {
                if (publicKeyToken == value) return;

                publicKeyToken = value;
                OnPropertyChanged("PublicKeyToken");
            }
        }

        ProcessorArchitecture processorArchitecture;
        public ProcessorArchitecture ProcessorArchitecture
        {
            get { return processorArchitecture; }
            private set
            {
                if (processorArchitecture == value) return;

                processorArchitecture = value;
                OnPropertyChanged("ProcessorArchitecture");
            }
        }

        string hintPath;
        public string HintPath
        {
            get { return hintPath; }
            set
            {
                if (hintPath == value) return;

                hintPath = value;
                OnPropertyChanged("HintPath");

                UseHintPath = !string.IsNullOrEmpty(hintPath);
            }
        }

        bool useHintPath;
        public bool UseHintPath
        {
            get { return useHintPath; }
            set
            {
                if (useHintPath == value) return;

                useHintPath = value;
                OnPropertyChanged("UseHintPath");
            }
        }

        public ReferenceEdit() { }

        public ReferenceEdit(Reference reference)
        {
            SetAssemblyName(new AssemblyName(reference.AssemblyName));
            HintPath = reference.HintPath;
        }

        public void SetAssemblyName(string path)
        {
            var assemblyName = AssemblyName.GetAssemblyName(path);
            SetAssemblyName(assemblyName);
            HintPath = path;
        }

        void SetAssemblyName(AssemblyName assemblyName)
        {
            Name = assemblyName.Name;
            Version = assemblyName.Version.ToString();
            Culture = (assemblyName.CultureInfo == null) ? string.Empty : assemblyName.CultureInfo.Name;
            PublicKeyToken = ToHexString(assemblyName.GetPublicKeyToken());
            ProcessorArchitecture = assemblyName.ProcessorArchitecture;
        }

        public void Register(ContentProject project)
        {
            var assemblyName = CreateAssemblyName().FullName;
            var hintPath = (UseHintPath && !string.IsNullOrEmpty(HintPath)) ? HintPath : null;
            project.AddReference(assemblyName, hintPath);
        }

        AssemblyName CreateAssemblyName()
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

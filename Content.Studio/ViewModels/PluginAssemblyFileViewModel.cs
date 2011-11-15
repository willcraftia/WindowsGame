#region Using

using System;
using System.Reflection;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

#endregion

namespace Willcraftia.Content.Studio.ViewModels
{
    public sealed class PluginAssemblyFileViewModel : ViewModelBase
    {
        readonly PluginSettingsEditViewModel owner;

        string path;
        public string Path
        {
            get { return path; }
            set
            {
                if (path == value) return;

                path = value;
                RaisePropertyChanged("Path");

                if (!string.IsNullOrEmpty(path))
                {
                    var assemblyName = AssemblyName.GetAssemblyName(path);
                    FullName = assemblyName.FullName;
                    Name = assemblyName.Name;
                    Version = assemblyName.Version.ToString();
                    Culture = (assemblyName.CultureInfo == null) ? string.Empty : assemblyName.CultureInfo.Name;
                    PublicKeyToken = ToHexString(assemblyName.GetPublicKeyToken());
                    ProcessorArchitecture = assemblyName.ProcessorArchitecture;
                }
            }
        }

        string fullName;
        public string FullName
        {
            get { return fullName; }
            private set
            {
                if (fullName == value) return;

                fullName = value;
                RaisePropertyChanged("FullName");
            }
        }

        string name;
        public string Name
        {
            get { return name; }
            private set
            {
                if (name == value) return;

                name = value;
                RaisePropertyChanged("Name");
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
                RaisePropertyChanged("Version");
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
                RaisePropertyChanged("Culture");
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
                RaisePropertyChanged("PublicKeyToken");
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
                RaisePropertyChanged("ProcessorArchitecture");
            }
        }

        public ICommand ReferAssemblyFileCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }

        public IMessenger Messenger
        {
            get { return MessengerInstance; }
        }

        public PluginAssemblyFileViewModel(PluginSettingsEditViewModel owner)
            : base(new Messenger())
        {
            this.owner = owner;
            ReferAssemblyFileCommand = new RelayCommand(ReferAssemblyFile);
            DeleteCommand = new RelayCommand(Delete);
        }

        void ReferAssemblyFile()
        {
            var message = new ReferFileMessage();
            message.Title = Properties.Resources.TitleSelectPluginAssembly;
            message.CheckFileExists = true;
            message.CheckPathExists = true;
            message.AddExtension = true;
            message.DefaultExt = ".dll";
            message.Filter = "Assembly files (*.dll;*.exe)|*.dll;*.exe|All files (*.*)|*.*";

            Messenger.Send(message);
            if (message.Result != true) return;

            Path = message.FileName;
        }

        void Delete()
        {
            owner.DeleteAssembly(this);
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

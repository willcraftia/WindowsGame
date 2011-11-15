#region Using

using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Willcraftia.Content.Studio.Models;

#endregion

namespace Willcraftia.Content.Studio.ViewModels
{
    public sealed class PluginSettingsEditViewModel : ViewModelBase
    {
        public PluginSettingsEdit Model { get; private set; }
        public ObservableCollection<PluginAssemblyFileViewModel> Assemblies { get; private set; }

        public ICommand AddAssemblyCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }

        public IMessenger Messenger
        {
            get { return MessengerInstance; }
        }

        public PluginSettingsEditViewModel(PluginSettingsEdit model)
            : base(new Messenger())
        {
            Model = model;

            Assemblies = new ObservableCollection<PluginAssemblyFileViewModel>();
            foreach (var assemblyFile in model.AssemblyFiles)
            {
                var assembly = new PluginAssemblyFileViewModel(this)
                {
                    Path = assemblyFile
                };
                Assemblies.Add(assembly);
            }

            AddAssemblyCommand = new RelayCommand(AddAssembly);
            SaveCommand = new RelayCommand(Save);
        }

        public void DeleteAssembly(PluginAssemblyFileViewModel assembly)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");

            Model.AssemblyFiles.Remove(assembly.Path);
            Assemblies.Remove(assembly);
        }

        void AddAssembly()
        {
            var assembly = new PluginAssemblyFileViewModel(this);
            var message = new ReferPluginAssemblyFileMessage(assembly);
            Messenger.Send(message);

            if (message.Result != true) return;

            Model.AssemblyFiles.Add(assembly.Path);
            Assemblies.Add(assembly);
        }

        void Save()
        {
            Model.Save();

            Messenger.Send(new CloseWindowMessage(this));
        }
    }
}

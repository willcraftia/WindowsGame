#region Using

using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Willcraftia.Content.Studio.Models;

#endregion

namespace Willcraftia.Content.Studio.ViewModels
{
    public sealed class ProjectReferenceEditViewModel : ViewModelBase
    {
        readonly ProjectPropertiesEditViewModel owner;

        public ProjectReferenceEdit Model { get; private set; }

        bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected == value) return;

                isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }

        public ICommand ReferProjectFileCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }

        public IMessenger Messenger
        {
            get { return MessengerInstance; }
        }

        public ProjectReferenceEditViewModel(ProjectPropertiesEditViewModel owner, ProjectReferenceEdit model)
            : base(owner.Messenger)
        {
            this.owner = owner;
            Model = model;

            ReferProjectFileCommand = new RelayCommand(ReferProjectFile);
            DeleteCommand = new RelayCommand(Delete);
        }

        void ReferProjectFile()
        {
            var message = new ReferFileMessage();
            message.Title = Properties.Resources.TitleSelectPluginAssembly;
            message.CheckFileExists = true;
            message.CheckPathExists = true;
            message.AddExtension = true;
            message.DefaultExt = ".dll";
            message.Filter = "C# project files (*.csproj)|*.csproj|All files (*.*)|*.*";
            if (!string.IsNullOrEmpty(Model.Path))
            {
                message.FileName = Model.Path;
            }

            Messenger.Send(message);
            if (message.Result != true) return;

            Model.Path = message.FileName;
        }

        void Delete()
        {
            owner.DeleteProjectReference(this);
        }
    }
}

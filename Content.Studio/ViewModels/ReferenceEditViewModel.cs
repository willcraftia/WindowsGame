#region Using

using System.Reflection;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Willcraftia.Content.Studio.Models;
using Willcraftia.Win.Framework;
using Willcraftia.Xna.Framework.Content.Build;

#endregion

namespace Willcraftia.Content.Studio.ViewModels
{
    public sealed class ReferenceEditViewModel : ViewModelBase
    {
        readonly ProjectPropertiesEditViewModel owner;

        public ReferenceEdit Model { get; private set; }

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

        public ICommand ReferAssemblyFileCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }

        public IMessenger Messenger
        {
            get { return MessengerInstance; }
        }

        public ReferenceEditViewModel(ProjectPropertiesEditViewModel owner, ReferenceEdit model)
            : base(owner.Messenger)
        {
            this.owner = owner;
            Model = model;

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
            message.FileName = Model.HintPath;

            Messenger.Send(message);
            if (message.Result != true) return;

            Model.SetAssemblyName(message.FileName);
        }

        void Delete()
        {
            owner.DeleteReference(this);
        }
    }
}

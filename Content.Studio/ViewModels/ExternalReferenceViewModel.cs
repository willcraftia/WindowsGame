#region Using

using System;
using System.IO;
using System.Windows.Input;
using Microsoft.Xna.Framework.Content.Pipeline;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Willcraftia.Win.Framework;

#endregion

namespace Willcraftia.Content.Studio.ViewModels
{
    public class ExternalReferenceViewModel<T> : ViewModelBase
    {
        protected FileInfo OwnerFile { get; private set; }

        protected PropertyModel<ExternalReference<T>> PropertyModel { get; private set; }

        protected ExternalReference<T> Model
        {
            get { return PropertyModel.Value; }
            set { PropertyModel.Value = value; }
        }

        public string FullName
        {
            get
            {
                if (Model == null) return null;
                return Model.Filename;
            }
        }

        public string FileName
        {
            get
            {
                if (Model == null || string.IsNullOrEmpty(Model.Filename)) return null;

                var baseUri = new Uri(OwnerFile.FullName);
                var targetUri = new Uri(Model.Filename);
                return baseUri.MakeRelativeUri(targetUri).ToString();
            }
            set
            {
                if (Model == null && string.IsNullOrEmpty(value)) return;
                if (Model != null && Model.Filename == value) return;

                Model = string.IsNullOrEmpty(value) ? null : new ExternalReference<T>(value);
                RaisePropertyChanged("FileName");
                RaisePropertyChanged("FullName");

                OnModelChanged();
            }
        }

        public ICommand ReferFileCommand { get; private set; }

        public IMessenger Messenger
        {
            get { return MessengerInstance; }
        }

        protected ExternalReferenceViewModel(FileInfo ownerFile, PropertyModel<ExternalReference<T>> propertyModel)
            : base(new Messenger())
        {
            OwnerFile = ownerFile;
            PropertyModel = propertyModel;

            ReferFileCommand = new RelayCommand(ExecuteReferFile);
        }

        protected virtual void OnModelChanged()
        {
        }

        protected virtual void InitializeOpenFileDialogMessage(ReferFileMessage message)
        {
        }

        void ExecuteReferFile()
        {
            var message = new ReferFileMessage();
            message.CheckFileExists = true;
            message.CheckPathExists = true;
            message.AddExtension = true;
            if (Model != null)
            {
                message.FileName = Model.Filename;
            }
            InitializeOpenFileDialogMessage(message);

            Messenger.Send(message);
            if (message.Result != true) return;

            FileName = message.FileName;
        }
    }
}

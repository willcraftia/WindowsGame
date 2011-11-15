#region Using

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Willcraftia.Win.Framework;
using Willcraftia.Content.Studio.Models;

#endregion

namespace Willcraftia.Content.Studio.ViewModels
{
    public sealed class ContentFolderViewModel : ViewModelBase
    {
        static readonly ContentFolderViewModel dummyChild = new ContentFolderViewModel();

        readonly ContentFolder model;

        public ContentFolderViewModel Parent { get; private set; }

        public string Name
        {
            get { return model.Directory.Name; }
        }

        public ObservableCollection<ContentFolderViewModel> Children { get; private set; }

        public ObservableCollection<ContentFileViewModel> Files { get; private set; }

        bool isExpanded;
        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                if (isExpanded != value)
                {
                    isExpanded = value;
                    RaisePropertyChanged("IsExpanded");

                    Icon = isExpanded ? OpenedIcon : ClosedIcon;
                }

                if (HasDummyChild)
                {
                    Children.Remove(dummyChild);
                    LoadChildren();
                }
            }
        }

        bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected == value) return;

                isSelected = value;
                RaisePropertyChanged("IsSelected");

                if (isSelected)
                {
                    LoadFiles();
                }
                else
                {
                    Files.Clear();
                }
            }
        }

        ImageSource icon;
        public ImageSource Icon
        {
            get
            {
                if (icon == null)
                    icon = isExpanded ? OpenedIcon : ClosedIcon;
                return icon;
            }
            set
            {
                if (icon == value) return;

                icon = value;
                RaisePropertyChanged("Icon");
            }
        }

        ImageSource openedIcon;
        ImageSource OpenedIcon
        {
            get
            {
                if (openedIcon == null)
                    openedIcon = IconReaderService.GetFolderIconImage(false, true);
                return openedIcon;
            }
        }

        ImageSource closedIcon;
        ImageSource ClosedIcon
        {
            get
            {
                if (closedIcon == null)
                    closedIcon = IconReaderService.GetFolderIconImage(true, true);
                return closedIcon;
            }
        }

        IIconReaderService IconReaderService
        {
            get { return Workspace.Current.Services.GetService(typeof(IIconReaderService)) as IIconReaderService; }
        }

        bool HasDummyChild
        {
            get { return Children.Count == 1 && Children[0] == dummyChild; }
        }

        public ICommand CreateSubFolderCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }

        public IMessenger Messenger
        {
            get { return MessengerInstance; }
        }

        ContentFolderViewModel() { }

        public ContentFolderViewModel(ContentFolder model, ContentFolderViewModel parent, IMessenger messenger)
            : base(messenger)
        {
            this.model = model;

            Parent = parent;
            Children = new ObservableCollection<ContentFolderViewModel>();
            Children.Add(dummyChild);

            CreateSubFolderCommand = new RelayCommand(CreateSubFolder, () => IsSelected);
            DeleteCommand = new RelayCommand(Delete, () => IsSelected);
        }

        void LoadChildren()
        {
            foreach (var childModel in model.EnumerateFolders())
            {
                Children.Add(new ContentFolderViewModel(childModel, this, Messenger));
            }
        }

        void LoadFiles()
        {
            if (Files == null)
            {
                Files = new ObservableCollection<ContentFileViewModel>();
            }

            foreach (var fileModel in model.EnumerateFiles())
            {
                Files.Add(new ContentFileViewModel(fileModel, this, Messenger));
            }
        }

        void CreateSubFolder()
        {
            var message = new EditFolderMessage();
            message.Title = Properties.Resources.TitleCreateFolder;

            Messenger.Send(message);
            if (message.Result != true) return;

            var subFolderModel = model.CreateSubFolder(message.FolderName);
            if (HasDummyChild)
            {
                Children.Remove(dummyChild);
            }
            Children.Add(new ContentFolderViewModel(subFolderModel, this, Messenger));
        }

        void Delete()
        {
            var confirmationText = string.Format(Properties.Resources.ConfirmDeleteFolder, model.Directory.Name);
            if (!ConfirmWithMessageBox(confirmationText)) return;

            try
            {
                // ファイル システムから削除します。
                model.Delete();

                // メモリ内の親フォルダから削除します。
                if (Parent != null)
                {
                    Parent.Children.Remove(this);
                }
            }
            catch (IOException e)
            {
                NortifyErrorWithMessageBox(e.Message);
                Tracer.TraceSource.TraceEvent(TraceEventType.Warning, 0, e.Message + "\n" + e.StackTrace);
            }
        }

        bool ConfirmWithMessageBox(string text)
        {
            bool confirmed = false;
            var message = DialogMessageHelper.CreateConfirmDialogBoxMessage(
                text,
                result => confirmed = result == MessageBoxResult.Yes);
            message.Caption = Properties.Resources.TitleConfirmation;

            Messenger.Send(message);
            return confirmed;
        }

        void NortifyErrorWithMessageBox(string text)
        {
            var errorText = DialogMessageHelper.CreateErrorDialogBoxMessage(text, null);
            errorText.Caption = Properties.Resources.TitleError;
            Messenger.Send(errorText);
        }
    }
}

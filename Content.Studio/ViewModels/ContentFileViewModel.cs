#region Using

using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Xna.Framework.Content;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Willcraftia.Win.Framework;
using Willcraftia.Content.Studio.Models;

#endregion

namespace Willcraftia.Content.Studio.ViewModels
{
    public sealed class ContentFileViewModel : ViewModelBase
    {
        readonly ContentFile model;

        public ContentFolderViewModel Parent { get; private set; }

        /// <summary>
        /// 絶対パス。
        /// </summary>
        public string FullName
        {
            get { return model.File.FullName; }
        }

        /// <summary>
        /// ファイル名 (パスを含まない)。
        /// </summary>
        public string FileName
        {
            get { return model.File.Name; }
        }

        bool isAsset;
        public bool IsAsset
        {
            get { return isAsset; }
            private set
            {
                if (isAsset == value) return;

                isAsset = value;
                RaisePropertyChanged("IsAsset");
            }
        }

        ImageSource icon;

        /// <summary>
        /// アイコン。
        /// </summary>
        public ImageSource Icon
        {
            get
            {
                if (icon == null)
                    icon = IconReaderService.GetFileIconImage(model.File.FullName, true);
                return icon;
            }
        }

        bool isSelected;

        /// <summary>
        /// ビュー上で選択されているかどうかを示す値。
        /// </summary>
        /// <value>
        /// true (ビュー上で選択されている場合)、false (それ以外の場合)。
        /// </value>
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

        IIconReaderService IconReaderService
        {
            get { return Workspace.Current.Services.GetService(typeof(IIconReaderService)) as IIconReaderService; }
        }

        public ICommand EditCommand { get; private set; }
        public ICommand ViewCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }

        public ICommand RegisterAsAssetCommand { get; private set; }
        public ICommand EditAssetCommand { get; private set; }
        public ICommand UnregisterAsAssetCommand { get; private set; }

        public IMessenger Messenger
        {
            get { return MessengerInstance; }
        }

        public ContentFileViewModel(ContentFile model, ContentFolderViewModel parent, IMessenger messenger)
            : base(messenger)
        {
            this.model = model;
            Parent = parent;
            IsAsset = (model.Asset != null);

            EditCommand = new RelayCommand(Edit);
            ViewCommand = new RelayCommand(View, () => IsAsset);
            DeleteCommand = new RelayCommand(Delete);
            RegisterAsAssetCommand = new RelayCommand(RegisterAsAsset, () => !IsAsset);
            EditAssetCommand = new RelayCommand(EditAsset, () => IsAsset);
            UnregisterAsAssetCommand = new RelayCommand(UnregisterAsAsset, () => IsAsset);
        }

        void Edit()
        {
            var content = ContentViewModelFactory.Instance.CreateViewModel(model.File);
            if (content == null)
            {
                Tracer.TraceSource.TraceEvent(TraceEventType.Warning, 0,
                    "Cannot edit content because of no view model for '{0}'", model.File.FullName);
                return;
            }

            var message = new EditContentMessage(content);
            Messenger.Send(message);
        }

        void View()
        {
            if (!IsAsset) throw new InvalidOperationException(string.Format("File '{0}' is not an asset: ", model.File.FullName));

            var content = new RuntimeContentViewModel(model.Asset);
            var message = new OpenRuntimeContentMessage(content);
            Messenger.Send(message);
        }

        void Delete()
        {
            var confirmationText = string.Format(Properties.Resources.ConfirmDeleteFile, model.File.FullName);
            if (!ConfirmWithMessageBox(confirmationText)) return;

            try
            {
                // Model についてファイルを削除します。
                model.Delete();
                
                // ViewModel についてメモリ内の親フォルダからを削除します。
                if (Parent != null)
                {
                    Parent.Files.Remove(this);
                }
            }
            catch (IOException e)
            {
                NortifyErrorWithMessageBox(e.Message);
                Tracer.TraceSource.TraceEvent(TraceEventType.Warning, 0, e.Message + "\n" + e.StackTrace);
            }
        }

        void RegisterAsAsset()
        {
            var edit = new AssetEdit(model);
            var message = new EditAssetMessage(new AssetEditViewModel(edit));
            Messenger.Send(message);

            if (message.Result != true) return;

            edit.Register();
            IsAsset = true;
        }

        void EditAsset()
        {
            var edit = new AssetEdit(model);
            var message = new EditAssetMessage(new AssetEditViewModel(edit));
            Messenger.Send(message);

            if (message.Result != true) return;

            edit.Update();
        }

        void UnregisterAsAsset()
        {
            var confirmationText = string.Format(Properties.Resources.ConfirmDeleteFileFromProject, model.Asset.ResolveAssetPath());
            if (!ConfirmWithMessageBox(confirmationText)) return;

            model.UnregisterAsAsset();

            IsAsset = false;
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

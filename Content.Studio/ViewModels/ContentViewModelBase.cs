#region Using

using System;
using System.IO;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Willcraftia.Content.Studio.Models;

#endregion

namespace Willcraftia.Content.Studio.ViewModels
{
    public class ContentViewModelBase : ViewModelBase
    {
        public FileInfo File { get; private set; }
        public object Model { get; private set; }

        public Type ContentType
        {
            get { return Model.GetType(); }
        }

        public string FileName
        {
            get { return File.Name; }
        }

        public ICommand RevertCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }

        public IMessenger Messenger
        {
            get { return MessengerInstance; }
        }

        protected ContentViewModelBase(FileInfo file)
            : base(new Messenger())
        {
            if (file == null) throw new ArgumentNullException("file");

            File = file;
            Model = ContentLoaderManager.Instance.Load(file);

            RevertCommand = new RelayCommand(Revert);
            SaveCommand = new RelayCommand(Save);
        }

        void Revert()
        {
            // 編集ウィンドウを閉じるよう通知します。
            //
            // TODO: VM 固有にする？
            //
            Messenger.Send(new CloseDocumentMessage(this));
        }

        void Save()
        {
            // コンテンツをセーブします。
            ContentLoaderManager.Instance.Save(File, Model);

            // 編集ウィンドウを閉じるよう通知します。
            //
            // TODO: VM 固有にする？
            //
            Messenger.Send(new CloseDocumentMessage(this));
        }
    }
}

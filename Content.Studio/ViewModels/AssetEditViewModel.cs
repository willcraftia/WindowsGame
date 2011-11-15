#region Using

using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Willcraftia.Content.Studio.Models;
using Willcraftia.Win.Framework;

#endregion

namespace Willcraftia.Content.Studio.ViewModels
{
    public sealed class AssetEditViewModel : ViewModelBase
    {
        public AssetEdit Model { get; private set; }

        public ICommand RegisterCommand { get; private set; }

        public IMessenger Messenger
        {
            get { return MessengerInstance; }
        }

        public AssetEditViewModel(AssetEdit model)
            : base(new Messenger())
        {
            Model = model;

            RegisterCommand = new RelayCommand(Register);
        }

        void Register()
        {
            Model.Register();
        }
    }
}

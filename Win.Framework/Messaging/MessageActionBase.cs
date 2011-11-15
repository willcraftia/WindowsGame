#region Using

using System.Windows;
using System.Windows.Interactivity;
using GalaSoft.MvvmLight.Messaging;

#endregion

namespace Willcraftia.Win.Framework.Messaging
{
    public abstract class MessageActionBase<T> : TriggerAction<DependencyObject> where T : class
    {
        protected override void Invoke(object parameter)
        {
            var message = parameter as T;
            if (message == null) return;

            InvokeAction(message);
        }

        protected abstract void InvokeAction(T message);
    }
}

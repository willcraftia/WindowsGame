#region Using

using System.Windows;
using System.Windows.Interactivity;
using GalaSoft.MvvmLight.Messaging;

#endregion

namespace Willcraftia.Win.Framework.Messaging
{
    public class MessageTriggerBase<T> : TriggerBase<DependencyObject>
    {
        public static readonly DependencyProperty SourceObjectProperty = DependencyProperty.RegisterAttached(
                "SourceObject", typeof(IMessenger), typeof(MessageTriggerBase<T>),
                new FrameworkPropertyMetadata(Messenger.Default, new PropertyChangedCallback(OnSourceObjectChanged)));

        static void OnSourceObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var trigger = d as MessageTriggerBase<T>;

            if (e.OldValue != null)
            {
                trigger.Unregister();
            }

            if (e.NewValue != null)
            {
                trigger.Register();
            }
        }

        public IMessenger SourceObject
        {
            get { return GetValue(SourceObjectProperty) as IMessenger; }
            set { SetValue(SourceObjectProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            Register();
        }

        protected override void OnDetaching()
        {
            Unregister();

            base.OnDetaching();
        }

        void Register()
        {
            SourceObject.Register<T>(AssociatedObject, message => InvokeActions(message));
        }

        void Unregister()
        {
            SourceObject.Unregister<T>(AssociatedObject);
        }
    }
}

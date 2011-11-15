#region Using

using System;
using System.Collections.Generic;
using System.Windows.Controls;

#endregion

namespace Willcraftia.Content.Studio.Views
{
    public sealed class ContentControlRegistry
    {
        static ContentControlRegistry instance = new ContentControlRegistry();
        public static ContentControlRegistry Instance
        {
            get { return instance; }
        }

        Dictionary<Type, Type> controlTypes = new Dictionary<Type, Type>();

        ContentControlRegistry() { }

        public void Register<TViewModel, TControl>() where TControl : ContentControl
        {
            controlTypes[typeof(TViewModel)] = typeof(TControl);

            Tracer.TraceSource.TraceInformation("Registerd ContentControl '{0}' for '{1}'",
                typeof(TControl).FullName, typeof(TViewModel).FullName);
        }

        public void Unregister<TContent>()
        {
            controlTypes.Remove(typeof(TContent));

            Tracer.TraceSource.TraceInformation("Unregisterd ContentControl for '{0}'", typeof(TContent).FullName);
        }

        public Type GetControlType(Type viewModelType)
        {
            Type controlType = null;
            controlTypes.TryGetValue(viewModelType, out controlType);
            return controlType;
        }
    }
}

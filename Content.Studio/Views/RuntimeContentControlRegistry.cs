#region Using

using System;
using System.Collections.Generic;
using System.Windows.Controls;

#endregion

namespace Willcraftia.Content.Studio.Views
{
    public sealed class RuntimeContentControlRegistry
    {
        static RuntimeContentControlRegistry instance = new RuntimeContentControlRegistry();
        public static RuntimeContentControlRegistry Instance
        {
            get { return instance; }
        }

        Dictionary<Type, Type> controlTypes = new Dictionary<Type, Type>();

        RuntimeContentControlRegistry() { }

        public void Register<TRuntimeContent, TControl>() where TControl : ContentControl
        {
            controlTypes[typeof(TRuntimeContent)] = typeof(TControl);

            Tracer.TraceSource.TraceInformation("Registerd RuntimeContentControl '{0}' for '{1}'",
                typeof(TControl).FullName, typeof(TRuntimeContent).FullName);
        }

        public void Unregister<TRuntimeContent>()
        {
            controlTypes.Remove(typeof(TRuntimeContent));

            Tracer.TraceSource.TraceInformation("Unregisterd RuntimeContentControl for '{0}'", typeof(TRuntimeContent).FullName);
        }

        public Type GetControlType(Type runtimeContentType)
        {
            Type controlType = null;
            controlTypes.TryGetValue(runtimeContentType, out controlType);
            return controlType;
        }
    }
}

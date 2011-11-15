#region Using

using System;
using System.Collections.Generic;

#endregion

namespace Willcraftia.Content.Studio.ViewModels
{
    public sealed class ContentViewModelRegistry
    {
        static ContentViewModelRegistry instance = new ContentViewModelRegistry();
        public static ContentViewModelRegistry Instance
        {
            get { return instance; }
        }

        Dictionary<Type, Type> viewModelTypes = new Dictionary<Type, Type>();

        ContentViewModelRegistry() { }

        public void Register<TContent, TViewModel>()
        {
            viewModelTypes[typeof(TContent)] = typeof(TViewModel);

            Tracer.TraceSource.TraceInformation("Registerd ContentViewModel '{0}' for '{1}'",
                typeof(TViewModel).FullName, typeof(TContent).FullName);
        }

        public void Unregister<TContent>()
        {
            viewModelTypes.Remove(typeof(TContent));

            Tracer.TraceSource.TraceInformation("Unregisterd ContentViewModel for '{0}'", typeof(TContent).FullName);
        }

        public Type GetViewModelType(Type contentType)
        {
            Type viewModelType = null;
            viewModelTypes.TryGetValue(contentType, out viewModelType);
            return viewModelType;
        }
    }
}

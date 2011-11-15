#region Using

using System;
using System.Collections.Generic;

#endregion

namespace Willcraftia.Content.Studio.Models
{
    public sealed class ContentLoaderRegistry
    {
        static ContentLoaderRegistry instance = new ContentLoaderRegistry();
        public static ContentLoaderRegistry Instance
        {
            get { return instance; }
        }

        Dictionary<string, IContentLoader> loaders = new Dictionary<string, IContentLoader>();

        ContentLoaderRegistry() { }

        public void Register(string extension, IContentLoader loader)
        {
            if (extension == null) throw new ArgumentNullException("extension");
            if (loader == null) throw new ArgumentNullException("loader");

            loaders[extension] = loader;

            Tracer.TraceSource.TraceInformation("Registerd ContentLoader '{0}' for '{1}'",
                loader.GetType().FullName, extension);
        }

        public void Unregister(string extension)
        {
            if (extension == null) throw new ArgumentNullException("extension");

            loaders.Remove(extension);

            Tracer.TraceSource.TraceInformation("Unregisterd ContentLoader for '{0}'", extension);
        }

        public IContentLoader GetLoader(string extension)
        {
            if (extension == null) throw new ArgumentNullException("extension");

            IContentLoader loader = null;
            loaders.TryGetValue(extension, out loader);
            return loader;
        }
    }
}

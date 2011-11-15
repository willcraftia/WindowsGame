#region Using

using System;
using System.Collections.Generic;

#endregion

namespace Willcraftia.Content.Studio.Models
{
    public sealed class ContentTypeResolverRegistry
    {
        static ContentTypeResolverRegistry instance = new ContentTypeResolverRegistry();
        public static ContentTypeResolverRegistry Instance
        {
            get { return instance; }
        }

        Dictionary<string, IContentTypeResolver> resolvers = new Dictionary<string, IContentTypeResolver>();

        ContentTypeResolverRegistry() { }

        public void Register(string extension, IContentTypeResolver resolver)
        {
            if (extension == null) throw new ArgumentNullException("extension");
            if (resolver == null) throw new ArgumentNullException("resolver");

            resolvers[extension] = resolver;

            Tracer.TraceSource.TraceInformation("Registerd ContentTypeResolver '{0}' for '{1}'",
                resolver.GetType().FullName, extension);
        }

        public void Unregister(string extension)
        {
            if (extension == null) throw new ArgumentNullException("extension");

            resolvers.Remove(extension);

            Tracer.TraceSource.TraceInformation("Unregisterd ContentTypeResolver for '{0}'", extension);
        }

        public IContentTypeResolver GetResolver(string extension)
        {
            if (extension == null) throw new ArgumentNullException("extension");

            IContentTypeResolver resolver = null;
            resolvers.TryGetValue(extension, out resolver);
            return resolver;
        }
    }
}

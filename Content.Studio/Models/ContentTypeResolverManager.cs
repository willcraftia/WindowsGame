#region Using

using System;
using System.Diagnostics;
using System.IO;

#endregion

namespace Willcraftia.Content.Studio.Models
{
    public sealed class ContentTypeResolverManager
    {
        static ContentTypeResolverManager instance = new ContentTypeResolverManager();
        public static ContentTypeResolverManager Instance
        {
            get { return instance; }
        }

        ContentTypeResolverManager() { }

        public Type ResolveContentType(FileInfo file)
        {
            var resolver = ContentTypeResolverRegistry.Instance.GetResolver(file.Extension);
            if (resolver == null)
            {
                Tracer.TraceSource.TraceEvent(TraceEventType.Warning, 0, "ContentTypeResolver for '{0}' not found.", file.Extension);
                return null;
            }

            return resolver.ResolveContentType(file);
        }
    }
}

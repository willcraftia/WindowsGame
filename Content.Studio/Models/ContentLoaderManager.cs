#region Using

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

#endregion

namespace Willcraftia.Content.Studio.Models
{
    public sealed class ContentLoaderManager
    {
        static ContentLoaderManager instance = new ContentLoaderManager();
        public static ContentLoaderManager Instance
        {
            get { return instance; }
        }

        ContentLoaderManager() { }

        public object Load(FileInfo file)
        {
            if (file == null) throw new ArgumentNullException("file");

            var loader = ContentLoaderRegistry.Instance.GetLoader(file.Extension);
            if (loader == null)
            {
                Tracer.TraceSource.TraceEvent(TraceEventType.Warning, 0, "ContentLoader not found for '{0}'.", file.Extension);
                return null;
            }

            var content = loader.Load(file.FullName);
            if (content == null)
            {
                Tracer.TraceSource.TraceEvent(TraceEventType.Warning, 0,
                    "Cannot load content '{0}' with loader '{1}'.", file.FullName, loader.GetType().FullName);
                return null;
            }

            Tracer.TraceSource.TraceInformation("Loaded content '{0}'.", file.FullName);
            return content;
        }

        public bool Save(FileInfo file, object content)
        {
            if (file == null) throw new ArgumentNullException("file");
            if (content == null) throw new ArgumentNullException("content");

            var loader = ContentLoaderRegistry.Instance.GetLoader(file.Extension);
            if (loader == null)
            {
                Tracer.TraceSource.TraceEvent(TraceEventType.Warning, 0, "ContentLoader not found for '{0}'.", file.Extension);
                return false;
            }

            if (!loader.Save(file.FullName, content))
            {
                Tracer.TraceSource.TraceEvent(TraceEventType.Warning, 0,
                    "Cannot save content '{0}' with loader '{1}'.", file.FullName, loader.GetType().FullName);
                return false;
            }

            Tracer.TraceSource.TraceInformation("Save content '{0}'.", file.FullName);
            return true;
        }
    }
}

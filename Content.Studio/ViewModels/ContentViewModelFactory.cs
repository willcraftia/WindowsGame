#region Using

using System;
using System.Diagnostics;
using System.IO;
using Willcraftia.Content.Studio.Models;
using Willcraftia.Content.Studio.ViewModels;

#endregion

namespace Willcraftia.Content.Studio.ViewModels
{
    public sealed class ContentViewModelFactory
    {
        static ContentViewModelFactory instance = new ContentViewModelFactory();
        public static ContentViewModelFactory Instance
        {
            get { return instance; }
        }

        ContentViewModelFactory() { }

        public bool HasViewModel(FileInfo file)
        {
            return GetViewModelType(file) != null;
        }

        public ContentViewModelBase CreateViewModel(FileInfo file)
        {
            if (file == null) throw new ArgumentNullException("file");

            Type type = GetViewModelType(file);
            if (type == null) return null;

            return Activator.CreateInstance(type, file) as ContentViewModelBase;
        }

        Type GetViewModelType(FileInfo file)
        {
            Type contentType = ContentTypeResolverManager.Instance.ResolveContentType(file);
            if (contentType == null)
            {
                Tracer.TraceSource.TraceEvent(TraceEventType.Warning, 0, "Cannot resolve content type for '{0}'.", file.FullName);
                return null;
            }

            Type type = GetViewModelType(contentType);
            if (type == null)
            {
                Tracer.TraceSource.TraceEvent(TraceEventType.Warning, 0,
                    "ContentViewModel for '{0}' not found.", contentType.FullName);
                return null;
            }
            return type;
        }

        Type GetViewModelType(Type contentType)
        {
            // コンテンツの型で検索します。
            Type type = ContentViewModelRegistry.Instance.GetViewModelType(contentType);
            if (type != null) return type;

            // 基底クラスについて検索します。
            if (contentType.BaseType != null)
            {
                type = GetViewModelType(contentType.BaseType);
                if (type != null) return type;
            }

            // 基底クラスでも見つからない場合はインタフェースについて検索します。
            foreach (var interfaceType in contentType.GetInterfaces())
            {
                type = GetViewModelType(interfaceType);
                if (type != null) return type;
            }

            return null;
        }
    }
}

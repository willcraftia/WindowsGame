#region Using

using System;
using System.Diagnostics;

#endregion

namespace Willcraftia.Content.Studio.Views
{
    public sealed class RuntimeContentControlFactory
    {
        static RuntimeContentControlFactory instance = new RuntimeContentControlFactory();
        public static RuntimeContentControlFactory Instance
        {
            get { return instance; }
        }

        RuntimeContentControlFactory() { }

        public object CreateControl(Type runtimeContentType)
        {
            if (runtimeContentType == null) throw new ArgumentNullException("runtimeContentType");

            var type = GetControlType(runtimeContentType);
            if (type == null)
            {
                Tracer.TraceSource.TraceEvent(TraceEventType.Warning, 0,
                    "RuntimeContentControl for '{0}' not found.", runtimeContentType.FullName);
                return null;
            }

            return Activator.CreateInstance(type);
        }

        Type GetControlType(Type runtimeContentType)
        {
            // コンテンツの型で検索します。
            Type type = RuntimeContentControlRegistry.Instance.GetControlType(runtimeContentType);
            if (type != null) return type;

            // 基底クラスについて検索します。
            if (runtimeContentType.BaseType != null)
            {
                type = GetControlType(runtimeContentType.BaseType);
                if (type != null) return type;
            }

            // 基底クラスでも見つからない場合はインタフェースについて検索します。
            foreach (var interfaceType in runtimeContentType.GetInterfaces())
            {
                type = GetControlType(interfaceType);
                if (type != null) return type;
            }

            return null;
        }
    }
}

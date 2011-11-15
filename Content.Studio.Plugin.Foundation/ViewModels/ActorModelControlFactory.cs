#region Using

using System;
using System.Diagnostics;
using System.Windows.Controls;

#endregion

namespace Willcraftia.Content.Studio.Plugin.Foundation.ViewModels
{
    public static class ActorModelControlFactory
    {
        public static ContentControl CreateControl(Type viewModelType)
        {
            var type = GetControlType(viewModelType);
            if (type == null)
            {
                Tracer.TraceSource.TraceEvent(TraceEventType.Warning, 0,
                    "ContentControl for '{0}' not found.", viewModelType.FullName);
                return null;
            }

            return Activator.CreateInstance(type) as ContentControl;
        }

        static Type GetControlType(Type viewModelType)
        {
            // コンテンツの型で検索します。
            Type type = ActorModelControlRegistry.GetControlType(viewModelType);
            if (type != null) return type;

            // 基底クラスについて検索します。
            if (viewModelType.BaseType != null)
            {
                type = GetControlType(viewModelType.BaseType);
                if (type != null) return type;
            }

            // 基底クラスでも見つからない場合はインタフェースについて検索します。
            foreach (var interfaceType in viewModelType.GetInterfaces())
            {
                type = GetControlType(interfaceType);
                if (type != null) return type;
            }

            return null;
        }
    }
}

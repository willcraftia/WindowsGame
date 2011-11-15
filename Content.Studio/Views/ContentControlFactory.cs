#region Using

using System;
using System.Diagnostics;
using System.Windows.Controls;

#endregion

namespace Willcraftia.Content.Studio.Views
{
    public sealed class ContentControlFactory
    {
        static ContentControlFactory instance = new ContentControlFactory();
        public static ContentControlFactory Instance
        {
            get { return instance; }
        }

        ContentControlFactory() { }

        public ContentControl CreateControl(Type viewModelType)
        {
            if (viewModelType == null) throw new ArgumentNullException("viewModelType");

            var type = GetControlType(viewModelType);
            if (type == null)
            {
                Tracer.TraceSource.TraceEvent(TraceEventType.Warning, 0,
                    "ContentControl for '{0}' not found.", viewModelType.FullName);
                return null;
            }

            return Activator.CreateInstance(type) as ContentControl;
        }

        Type GetControlType(Type viewModelType)
        {
            // コンテンツの型で検索します。
            Type type = ContentControlRegistry.Instance.GetControlType(viewModelType);
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

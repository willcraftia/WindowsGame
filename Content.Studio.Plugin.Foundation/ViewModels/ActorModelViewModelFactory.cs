#region Using

using System;
using System.Diagnostics;
using Willcraftia.Xna.Foundation.Scenes;

#endregion

namespace Willcraftia.Content.Studio.Plugin.Foundation.ViewModels
{
    public static class ActorModelViewModelFactory
    {
        public static ActorModelViewModel CreateViewModel(ActorModel model)
        {
            var type = GetViewModelType(model.GetType());
            if (type == null)
            {
                Tracer.TraceSource.TraceEvent(TraceEventType.Warning, 0,
                    "ActorModelViewModel for '{0}' not found.", model.GetType().FullName);
                return null;
            }

            return Activator.CreateInstance(type, model) as ActorModelViewModel;
        }

        static Type GetViewModelType(Type modelType)
        {
            // コンテンツの型で検索します。
            Type type = ActorModelViewModelRegistry.GetViewModelType(modelType);
            if (type != null) return type;

            // 基底クラスについて検索します。
            if (modelType.BaseType != null)
            {
                type = GetViewModelType(modelType.BaseType);
                if (type != null) return type;
            }

            // 基底クラスでも見つからない場合はインタフェースについて検索します。
            foreach (var interfaceType in modelType.GetInterfaces())
            {
                type = GetViewModelType(interfaceType);
                if (type != null) return type;
            }

            return null;
        }
    }
}

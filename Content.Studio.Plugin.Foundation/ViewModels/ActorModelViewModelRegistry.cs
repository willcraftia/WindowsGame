#region Using

using System;
using System.Collections.Generic;
using Willcraftia.Xna.Foundation.Scenes;

#endregion

namespace Willcraftia.Content.Studio.Plugin.Foundation.ViewModels
{
    public static class ActorModelViewModelRegistry
    {
        static Dictionary<Type, Type> viewModelTypes = new Dictionary<Type, Type>();

        public static void Register<TActorModel, TViewModel>() where TActorModel : ActorModel
        {
            viewModelTypes[typeof(TActorModel)] = typeof(TViewModel);

            Tracer.TraceSource.TraceInformation("Registered ActorModelViewModel '{0}' for '{1}'",
                typeof(TViewModel), typeof(TActorModel));
        }

        public static void Unregister<TActorModel>() where TActorModel : ActorModel
        {
            viewModelTypes.Remove(typeof(TActorModel));

            Tracer.TraceSource.TraceInformation("UnRegistered ActorModelViewModel '{0}'", typeof(TActorModel));
        }

        public static Type GetViewModelType(Type actorModelType)
        {
            Type type = null;
            viewModelTypes.TryGetValue(actorModelType, out type);
            return type;
        }
    }
}

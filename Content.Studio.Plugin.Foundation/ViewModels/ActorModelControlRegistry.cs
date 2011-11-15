#region Using

using System;
using System.Collections.Generic;

#endregion

namespace Willcraftia.Content.Studio.Plugin.Foundation.ViewModels
{
    public static class ActorModelControlRegistry
    {
        static Dictionary<Type, Type> controlTypes = new Dictionary<Type, Type>();

        public static void Register<TActorModelViewModel, TControl>() where TActorModelViewModel : ActorModelViewModel
        {
            controlTypes[typeof(TActorModelViewModel)] = typeof(TControl);

            Tracer.TraceSource.TraceInformation("Registered ActorModelControl '{0}' for '{1}'",
                typeof(TControl), typeof(TActorModelViewModel));
        }

        public static void Unregister<TActorModelViewModel>() where TActorModelViewModel : ActorModelViewModel
        {
            controlTypes.Remove(typeof(TActorModelViewModel));

            Tracer.TraceSource.TraceInformation("UnRegistered ActorModelControl '{0}'", typeof(TActorModelViewModel));
        }

        public static Type GetControlType(Type actorModelType)
        {
            Type type = null;
            controlTypes.TryGetValue(actorModelType, out type);
            return type;
        }
    }
}

#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Microsoft.Xna.Framework;

#endregion

namespace Willcraftia.Content.Studio.PluginFramework
{
    public sealed class GameRegistry
    {
        static GameRegistry instance = new GameRegistry();
        public static GameRegistry Instance
        {
            get { return instance; }
        }

        static Type gameType = typeof(Game);

        Collection<Type> gameTypes = new Collection<Type>();
        public Collection<Type> GameTypes
        {
            get { return gameTypes; }
        }

        GameRegistry() { }

        public void Register(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");

            foreach (var type in assembly.GetTypes())
            {
                if (!gameType.IsAssignableFrom(type)) continue;

                gameTypes.Add(type);
                Tracer.TraceSource.TraceInformation("Registered Game '{0}'.", type.FullName);
            }
        }

        public void Unregister(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");

            foreach (var type in assembly.GetTypes())
            {
                if (!gameType.IsAssignableFrom(type)) continue;

                gameTypes.Remove(type);
                Tracer.TraceSource.TraceInformation("Unregistered Game '{0}'.", type.FullName);
            }
        }
    }
}

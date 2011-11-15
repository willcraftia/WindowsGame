#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Willcraftia.Xna.Foundation.Graphics
{
    public sealed class EffectManager : IEffectManager, IDisposable
    {
        #region Fields and Properties

        public const string DefaultEffectAssetNamePrefix = "Effects/";

        static readonly Type[] constructorTypes = { typeof(Effect) };

        GraphicsDevice graphicsDevice;
        ContentManager content;

        object[] constructorArgs = new object[1];
        List<Effect> effects = new List<Effect>();

        string prefix = DefaultEffectAssetNamePrefix;
        public string Prefix
        {
            get { return prefix; }
            set { prefix = value; }
        }

        #endregion

        #region Constructors

        public EffectManager(GraphicsDevice graphicsDevice, ContentManager content)
        {
            this.graphicsDevice = graphicsDevice;
            this.content = content;
        }

        #endregion

        #region IEffectManager

        public T Load<T>() where T : Effect
        {
            return Load<T>(typeof(T).Name);
        }

        public T Load<T>(string assetName) where T : Effect
        {
            var constructor = GetConstructor<T>();

            var source = content.Load<Effect>(ResolveAssetName(assetName));

            constructorArgs[0] = source;

            var effect = constructor.Invoke(constructorArgs) as T;
            effects.Add(effect);

            return effect;
        }

        public T Load<T>(params object[] additionalParameters) where T : Effect
        {
            return Load<T>(typeof(T).Name, additionalParameters);
        }

        public T Load<T>(string assetName, params object[] additionalParameters) where T : Effect
        {
            var constructor = GetConstructor<T>(additionalParameters);

            var source = content.Load<Effect>(ResolveAssetName(assetName));

            var cargs = new object[1 + additionalParameters.Length];
            cargs[0] = source;
            for (int i = 0; i < additionalParameters.Length; i++)
            {
                cargs[1 + i] = additionalParameters[i];
            }

            var effect = constructor.Invoke(cargs) as T;
            effects.Add(effect);

            return effect;
        }

        #endregion

        public void Unload()
        {
            foreach (var effect in effects)
            {
                effect.Dispose();
            }
            effects.Clear();
        }

        string ResolveAssetName(string assetName)
        {
            if (string.IsNullOrEmpty(prefix))
            {
                return assetName;
            }

            return prefix + assetName;
        }

        string ResolveAssetName<T>() where T : Effect
        {
            return ResolveAssetName(typeof(T).Name);
        }

        ConstructorInfo GetConstructor<T>() where T : Effect
        {
            var constructor = typeof(T).GetConstructor(constructorTypes);
            if (constructor == null)
            {
                throw new InvalidOperationException(
                    string.Format("Constructor '{0}({1})' not found", typeof(T), typeof(Effect)));
            }
            return constructor;
        }

        ConstructorInfo GetConstructor<T>(params object[] additionalParameters) where T : Effect
        {
            var ctypes = new Type[1 + additionalParameters.Length];
            ctypes[0] = typeof(Effect);
            for (int i = 0; i < additionalParameters.Length; i++)
            {
                ctypes[1 + i] = additionalParameters[i].GetType();
            }

            var constructor = typeof(T).GetConstructor(ctypes);
            if (constructor == null)
            {
                throw new InvalidOperationException(
                    string.Format("Constructor '{0}({1}, ...)' not found", typeof(T), typeof(Effect)));
            }
            return constructor;
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        bool disposed;

        ~EffectManager()
        {
            Dispose(false);
        }

        void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    Unload();
                }
                disposed = true;
            }
        }

        #endregion
    }
}

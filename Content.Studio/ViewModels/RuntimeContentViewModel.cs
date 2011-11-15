#region Using

using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework.Content;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Willcraftia.Content.Studio.Models;
using Willcraftia.Xna.Framework.Content.Build;

#endregion

namespace Willcraftia.Content.Studio.ViewModels
{
    public sealed class RuntimeContentViewModel : ViewModelBase
    {
        public Asset Asset { get; private set; }
        public Type ContentType { get; private set; }

        public IMessenger Messenger
        {
            get { return MessengerInstance; }
        }

        public RuntimeContentViewModel(Asset asset)
            : base(new Messenger())
        {
            if (asset == null) throw new ArgumentNullException("asset");

            Asset = asset;
            ContentType = RuntimeContentTypeManager.Instance.ResolveRuntimeContentType(asset);
        }

        public object LoadContent(ContentManager contentManager)
        {
            try
            {
                return contentManager.Load<object>(Asset.ResolveAssetPath());
            }
            catch (ContentLoadException e)
            {
                Tracer.TraceSource.TraceEvent(TraceEventType.Warning, 0, e.Message + "\n" + e.StackTrace);
                return null;
            }
        }
    }
}

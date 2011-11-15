#region Using

using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using GalaSoft.MvvmLight.Messaging;
using Willcraftia.Win.Framework;

#endregion

namespace Willcraftia.Content.Studio.ViewModels
{
    public sealed class EffectReferenceViewModel : ExternalReferenceViewModel<EffectContent>
    {
        public EffectReferenceViewModel(FileInfo ownerFile, PropertyModel<ExternalReference<EffectContent>> propertyModel)
            : base(ownerFile, propertyModel)
        {
        }

        protected override void InitializeOpenFileDialogMessage(ReferFileMessage message)
        {
            message.DefaultExt = ".fx";
            message.Filter = "Effect files (*.fx)|*.fx|All files (*.*)|*.*";

            base.InitializeOpenFileDialogMessage(message);
        }
    }
}

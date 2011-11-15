#region Using

using System.Collections.Generic;
using Willcraftia.Content.Studio.ViewModels;
using Willcraftia.Win.Framework.Messaging;

#endregion

namespace Willcraftia.Content.Studio.Views
{
    public sealed class CloseProjectMessageAction : MessageActionBase<CloseProjectMessage>
    {
        protected override void InvokeAction(CloseProjectMessage message)
        {
            var documents = new List<AvalonDock.DocumentContent>((AssociatedObject as MainWindow).DockingManager.Documents);
            documents.ForEach(d => d.Close());
        }
    }
}

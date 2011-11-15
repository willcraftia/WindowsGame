#region Using

using System.Windows;
using Willcraftia.Content.Studio.ViewModels;
using Willcraftia.Win.Framework.Messaging;

#endregion

namespace Willcraftia.Content.Studio.Views
{
    public sealed class EditContentMessageAction : MessageActionBase<EditContentMessage>
    {
        protected override void InvokeAction(EditContentMessage message)
        {
            var document = GetOpenedContentDocument(message.Content);
            if (document == null)
            {
                document = new ContentDocument();
                document.DataContext = message.Content;
                document.Show((AssociatedObject as MainWindow).DockingManager);
            }

            document.Activate();
        }

        /// <summary>
        /// 既に開かれている コンテンツ の DocumentContent を取得します。
        /// </summary>
        /// <param name="file">コンテンツ。</param>
        /// <returns>
        /// DocumentContent (既に ContentFileViewModel が開かれていた場合)、null (それ以外の場合)。
        /// </returns>
        ContentDocument GetOpenedContentDocument(ContentViewModelBase content)
        {
            foreach (var document in (AssociatedObject as MainWindow).DockingManager.Documents)
            {
                var contentDocument = document as ContentDocument;
                if (contentDocument == null) continue;

                var openedContent = contentDocument.DataContext as ContentViewModelBase;
                if (openedContent != null && openedContent.File == content.File)
                {
                    return contentDocument;
                }
            }

            return null;
        }
    }
}

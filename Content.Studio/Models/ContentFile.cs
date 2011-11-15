#region Using

using System;
using System.IO;
using Willcraftia.Content.Studio.PluginFramework;
using Willcraftia.Xna.Framework.Content.Build;

#endregion

namespace Willcraftia.Content.Studio.Models
{
    public sealed class ContentFile
    {
        public FileInfo File { get; private set; }

        public Asset Asset { get; private set; }

        public ContentFile(FileInfo file)
        {
            File = file;
            Asset = Workspace.Current.Project.GetAsset(file);
        }

        public void Delete()
        {
            // 必要に応じてアセット情報をプロジェクトから削除します。
            UnregisterAsAsset();
            // ファイル システムから削除します。
            File.Delete();
        }

        public void RegisterAsAsset(AssetBuildActionKind buidAction, string name, string importer, string processor)
        {
            Asset = Workspace.Current.Project.AddAsset(File, buidAction, name, importer, processor);
        }

        public void UnregisterAsAsset()
        {
            if (Asset == null) return;

            Workspace.Current.Project.DeleteAsset(File);
            Asset = null;
        }
    }
}

#region Using

using System;
using System.IO;

#endregion

namespace Willcraftia.Xna.Framework.Content.Build
{
    public sealed class Asset
    {
        public ContentProject Project { get; private set; }

        public FileInfo File { get; private set; }

        /// <summary>
        /// ビルド アクション。
        /// </summary>
        public AssetBuildActionKind BuildAction { get; private set; }

        /// <summary>
        /// アセット名 (パスは含まない)。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Content Importer 名。
        /// </summary>
        /// <value>
        /// null の場合は、ビルド時にファイルの拡張子からデフォルトの Content Importer が選択されます。
        /// </value>
        public string Importer { get; private set; }

        /// <summary>
        /// Content Processor 名。
        /// </summary>
        /// <value>
        /// null の場合は、ビルド時に PassThroughProcessor が選択されます。
        /// </value>
        public string Processor { get; private set; }

        public Asset(ContentProject project, FileInfo file,
            AssetBuildActionKind buildAction, string name, string importer, string processor)
        {
            Project = project;
            File = file;
            BuildAction = buildAction;
            Name = name;
            Importer = importer;
            Processor = processor;
        }

        public string ResolveAssetPath()
        {
            var path = ResolvePath();
            return path.Substring(0, path.LastIndexOf(File.Extension));
        }

        public string ResolvePath()
        {
            var baseUri = new Uri(Project.ContentPath);
            var fileUri = new Uri(baseUri, File.FullName);
            return baseUri.MakeRelativeUri(fileUri).ToString();
        }
    }
}

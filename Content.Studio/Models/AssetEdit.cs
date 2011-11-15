#region Using

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Willcraftia.Win.Framework;
using Willcraftia.Xna.Framework.Content.Build;

#endregion

namespace Willcraftia.Content.Studio.Models
{
    public sealed class AssetEdit : ObservableObject
    {
        readonly ContentFile contentFile;

        public string Path { get; private set; }

        string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (name == value) return;

                name = value;
                OnPropertyChanged("Name");
            }
        }

        AssetBuildActionKind buildAction;
        public AssetBuildActionKind BuildAction
        {
            get { return buildAction; }
            set
            {
                if (buildAction == value) return;

                buildAction = value;
                OnPropertyChanged("BuildAction");
            }
        }

        string importer;
        public string Importer
        {
            get { return importer; }
            set
            {
                if (importer == value) return;

                importer = value;
                OnPropertyChanged("Importer");
            }
        }

        string processor;
        public string Processor
        {
            get { return processor; }
            set
            {
                if (processor == value) return;

                processor = value;
                OnPropertyChanged("Processor");
            }
        }

        public IEnumerable<string> Importers { get; private set; }
        public IEnumerable<string> Processors { get; private set; }

        public AssetEdit(ContentFile contentFile)
        {
            this.contentFile = contentFile;

            Path = ResolvePath();

            var asset = contentFile.Asset;
            if (asset != null)
            {
                Name = asset.Name;
                BuildAction = asset.BuildAction;
                Importer = asset.Importer;
                Processor = asset.Processor;
            }
            else
            {
                Name = System.IO.Path.GetFileNameWithoutExtension(contentFile.File.Name);
                BuildAction = AssetBuildActionKind.Compile;
            }

            // TODO
            // IImporterInfo でなんとか処理したい。表示名を使用したいから。
            Importers = Workspace.Current.Project.ImporterInfoRegistry.EnumerateImporterNames().OrderBy(i => i);
            Processors = Workspace.Current.Project.ProcessorInfoRegistry.EnumerateProcessorNames().OrderBy(i => i);
        }

        string ResolvePath()
        {
            var baseUri = new Uri(Workspace.Current.Project.ContentPath);
            var fileUri = new Uri(baseUri, contentFile.File.FullName);
            return baseUri.MakeRelativeUri(fileUri).ToString();
        }

        public void Register()
        {
            contentFile.RegisterAsAsset(buildAction, name, importer, processor);
        }

        public void Update()
        {
            contentFile.UnregisterAsAsset();
            contentFile.RegisterAsAsset(buildAction, name, importer, processor);
        }
    }
}

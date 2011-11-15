#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Win32;

#endregion

namespace Willcraftia.Xna.Framework.Content.Build
{
    /// <summary>
    /// MSBuild コンテンツ プロジェクトを管理するクラスです。
    /// </summary>
    public class ContentProject
    {
        #region Delegates

        /// <summary>
        /// ビルドの終了で呼び出されるコールバックのための delegate です。
        /// </summary>
        /// <param name="submission">ビルド要求。</param>
        public delegate void BuildCallback(BuildSubmission submission);

        #endregion

        #region Fields and Properties

        const string xnaVersion = ", Version=4.0.0.0, PublicKeyToken=842cf8be1de50553";

        // XNA 標準インポータ。
        static string[] importerAssembyNames =
        {
            "Microsoft.Xna.Framework.Content.Pipeline.FBXImporter" + xnaVersion,
            "Microsoft.Xna.Framework.Content.Pipeline.XImporter" + xnaVersion,
            "Microsoft.Xna.Framework.Content.Pipeline.TextureImporter" + xnaVersion,
            "Microsoft.Xna.Framework.Content.Pipeline.EffectImporter" + xnaVersion,
        };

        static string pipelineAssembyName = "Microsoft.Xna.Framework.Content.Pipeline" + xnaVersion;

        Project project;

        ProjectRootElement rootElement;

        BuildParameters buildParameters;

        public ILogger Logger { get; private set; }

        /// <summary>
        /// OutputPath。
        /// </summary>
        public string OutputPath
        {
            get { return project.GetPropertyValue("OutputPath"); }
            set { project.SetProperty("OutputPath", value); }
        }

        /// <summary>
        /// ContentRootDirectory。
        /// </summary>
        public string ContentRootDirectoryPath
        {
            get { return project.GetPropertyValue("ContentRootDirectory"); }
        }

        /// <summary>
        /// プロジェクト ファイルの完全パス。
        /// </summary>
        public string FullPath
        {
            get { return rootElement.FullPath; }
            set { rootElement.FullPath = value; }
        }

        /// <summary>
        /// プロジェクト ファイルのディレクトリ パス。
        /// </summary>
        public string DirectoryPath
        {
            get { return rootElement.DirectoryPath; }
        }

        /// <summary>
        /// ビルドされたコンテンツの出力先ディレクトリのパス。
        /// </summary>
        public string RuntimeContentPath
        {
            get { return Path.Combine(Path.Combine(DirectoryPath, OutputPath), ContentRootDirectoryPath); }
        }

        /// <summary>
        /// ビルドするコンテンツの配置ディレクトリのパス。
        /// </summary>
        public string ContentPath
        {
            get { return Path.Combine(DirectoryPath, ContentRootDirectoryPath); }
        }

        string xnaAssemblyFolder;
        public string XnaAssemblyFolder
        {
            get
            {
                if (xnaAssemblyFolder == null)
                {
                    var xnaLibBase = project.GetPropertyValue("XNAGSv4");
                    var xnaPlatform = project.GetPropertyValue("XnaPlatform");
                    var platform = project.GetPropertyValue("Platform");
                    xnaAssemblyFolder = Path.Combine(Path.Combine(Path.Combine(xnaLibBase, "References"), xnaPlatform), platform);
                }
                return xnaAssemblyFolder;
            }
        }

        public bool IsDirty
        {
            get { return project.IsDirty; }
        }

        List<Reference> references = new List<Reference>();
        List<ProjectReference> projectReferences = new List<ProjectReference>();

        ImporterInfoRegistry importerInfoRegistry = new ImporterInfoRegistry();
        public IImporterInfoRegistry ImporterInfoRegistry
        {
            get { return importerInfoRegistry; }
        }

        ProcessorInfoRegistry processorInfoRegistry = new ProcessorInfoRegistry();
        public IProcessorInfoRegistry ProcessorInfoRegistry
        {
            get { return processorInfoRegistry; }
        }

        #endregion

        ContentProject(ILogger logger)
        {
            Logger = logger;

            buildParameters = new BuildParameters();

            if (Logger != null)
            {
                buildParameters.Loggers = new ILogger[] { Logger };
            }
        }

        public static ContentProject Load(string path)
        {
            return Load(path, null);
        }

        /// <summary>
        /// 指定のパスからコンテンツ プロジェクトをロードします。
        /// </summary>
        /// <param name="path">コンテンツ プロジェクト ファイルのパス。</param>
        /// <param name="logger">ロガー、null (ロガーを設定しない場合)</param>
        /// <returns>ContentProject。</returns>
        public static ContentProject Load(string path, ILogger logger)
        {
            if (path == null) throw new ArgumentNullException("path");

            var rootElement = ProjectRootElement.Open(path);
            var project = new ContentProject(logger);
            project.LoadBuildProject(rootElement);

            Tracer.TraceSource.TraceInformation("Loaded project '{0}'.", project.FullPath);

            return project;
        }

        /// <summary>
        /// MSBuild コンテンツ プロジェクトをメモリ内にロードします。
        /// </summary>
        /// <param name="rootElement">コンテンツ プロジェクト ファイル。</param>
        void LoadBuildProject(ProjectRootElement rootElement)
        {
            this.rootElement = rootElement;
            project = new Project(rootElement);
            project.SetProperty("XNAContentPipelineTargetProfile", "HiDef");

            // ---------- test start
            //var projectInstance = project.CreateProjectInstance();

            //BuildManager.DefaultBuildManager.BeginBuild(buildParameters);

            //var request = new BuildRequestData(project.CreateProjectInstance(), new string[] { "GetReferenceAssemblyPaths" });
            //var submission = BuildManager.DefaultBuildManager.PendBuildRequest(request);
            //submission.ExecuteAsync(null, null);

            //// ビルド終了までスレッドを待機します。
            //submission.WaitHandle.WaitOne();

            //BuildManager.DefaultBuildManager.EndBuild();
            // ---------- test end

            foreach (var item in project.GetItems("Reference"))
            {
                var reference = new Reference(this, item.EvaluatedInclude, item.GetMetadataValue("HintPath"));
                AddReference(reference);
            }

            var pipelineAssemblyPath = ResolveAssemblyPath(new AssemblyName(pipelineAssembyName));
            if (!File.Exists(pipelineAssemblyPath))
            {
                throw new FileNotFoundException("XNA content pipeline assembly not found.", pipelineAssemblyPath);
            }
            var pipelineAssembly = Assembly.LoadFrom(pipelineAssemblyPath);
            importerInfoRegistry.Register(pipelineAssembly);

            foreach (var item in project.GetItems("ProjectReference"))
            {
                var projectReference = new ProjectReference(this,
                    item.EvaluatedInclude, item.GetMetadataValue("Project"), item.GetMetadataValue("Name"));
                AddProjectReference(projectReference);
            }
            processorInfoRegistry.Register(pipelineAssembly);
        }

        string ResolveAssemblyPath(AssemblyName assemblyName)
        {
            var assemblyFileName = assemblyName.Name + ".dll";
            return Path.Combine(XnaAssemblyFolder, assemblyFileName);
        }

        public static ContentProject Create(string path)
        {
            return Create(path, null);
        }

        /// <summary>
        /// 指定のパスでコンテンツ プロジェクトをメモリ内で作成します。
        /// </summary>
        /// <param name="logger">ロガー、null (ロガーを設定しない場合)</param>
        /// <returns>ContentProject。</returns>
        public static ContentProject Create(string path, ILogger logger)
        {
            if (path == null) throw new ArgumentNullException("path");

            var rootElement = ProjectRootElement.Create(path);
            rootElement.AddImport(
                @"$(MSBuildExtensionsPath)\Microsoft\XNA Game Studio\v4.0\Microsoft.Xna.GameStudio.ContentPipeline.targets");
            var project = new ContentProject(logger);
            project.CreateBuildProject(rootElement);
            project.project.SetProperty("ContentRootDirectory", "Content");

            Tracer.TraceSource.TraceInformation("Created project '{0}'.", project.FullPath);
            
            return project;
        }

        /// <summary>
        /// MSBuild コンテンツ プロジェクトをメモリ内に作成します。
        /// </summary>
        /// <param name="rootElement">コンテンツ プロジェクト ファイル。</param>
        void CreateBuildProject(ProjectRootElement rootElement)
        {
            this.rootElement = rootElement;
            project = new Project(rootElement);

            project.SetProperty("XnaPlatform", "Windows");
            //buildProject.SetProperty("XnaProfile", "Reach");
            project.SetProperty("XnaProfile", "HiDef");
            project.SetProperty("XnaFrameworkVersion", "v4.0");
            project.SetProperty("Configuration", "Release");
            project.SetProperty("Platform", "x86");
            project.SetProperty("ContentRootDirectory", "Content");
            project.SetProperty("OutputPath", @"bin\$(Platform)\$(Configuration)");

            // Register any custom importers or processors.
            foreach (var assemblyName in importerAssembyNames)
            {
                AddReference(assemblyName);
            }

            foreach (var item in project.GetItems("Reference"))
            {
                var reference = new Reference(this, item.EvaluatedInclude, item.GetMetadataValue("HintPath"));
                AddReference(reference);
            }
        }

        public void ReevaluateIfNecessary()
        {
            project.ReevaluateIfNecessary();
        }

        /// <summary>
        /// プロジェクトを保存します。
        /// </summary>
        public void Save()
        {
            var outputPathMemento = OutputPath;
            OutputPath = @"bin\$(Platform)\$(Configuration)";

            project.Save();

            OutputPath = outputPathMemento;
            project.ReevaluateIfNecessary();
        }

        /// <summary>
        /// プロジェクトを指定のファイル パスで保存します。
        /// </summary>
        /// <param name="path">ファイル パス。</param>
        public void Save(string path)
        {
            if (path == null) throw new ArgumentNullException("path");

            var outputPathMemento = OutputPath;
            OutputPath = @"bin\$(Platform)\$(Configuration)";

            project.Save(path);
            
            OutputPath = outputPathMemento;
            project.ReevaluateIfNecessary();
        }

        /// <summary>
        /// プロジェクトをアンロードします。
        /// </summary>
        public void Unload()
        {
            foreach (var projectReference in projectReferences)
            {
                projectReference.UnloadReferencedProject();
            }

            ProjectCollection.GlobalProjectCollection.UnloadProject(project);

            Tracer.TraceSource.TraceInformation("Unloaded project '{0}'.", project.FullPath);
        }

        /// <summary>
        /// Reference 項目を追加します。
        /// </summary>
        /// <param name="assemblyName">完全修飾アセンブリ名。</param>
        public void AddReference(string assemblyName)
        {
            AddReference(assemblyName, null);
        }

        /// <summary>
        /// Reference 項目を追加します。
        /// </summary>
        /// <param name="assemblyName">完全修飾アセンブリ名。</param>
        /// <param name="hintPath">アセンブリ ファイルのパス。</param>
        public void AddReference(string assemblyName, string hintPath)
        {
            if (assemblyName == null) throw new ArgumentNullException("assemblyName");

            var item = project.AddItem("Reference", assemblyName)[0];
            if (!string.IsNullOrEmpty(hintPath))
            {
                item.SetMetadataValue("HintPath", hintPath);
            }

            AddReference(new Reference(this, assemblyName, hintPath));
        }

        public void ClearReferences()
        {
            project.RemoveItems(project.GetItems("Reference"));
            references.Clear();
        }

        public IEnumerable<Reference> EnumerateReferences()
        {
            return references;
        }

        void AddReference(Reference reference)
        {
            references.Add(reference);
            importerInfoRegistry.Register(reference);
            processorInfoRegistry.Register(reference);
        }

        /// <summary>
        /// ProjectReference 項目を追加します。
        /// </summary>
        /// <param name="path">プロジェクト ファイルのパス。</param>
        public void AddProjectReference(string path)
        {
            AddProjectReference(path, null, null);
        }

        /// <summary>
        /// ProjectReference 項目を追加します。
        /// </summary>
        /// <param name="path">プロジェクト ファイルのパス。</param>
        /// <param name="guid">プロジェクトの GUID。</param>
        /// <param name="name">プロジェクトの名前。</param>
        public void AddProjectReference(string path, string guid, string name)
        {
            if (path == null) throw new ArgumentNullException("path");

            var item = project.AddItem("ProjectReference", path)[0];
            if (!string.IsNullOrEmpty(guid))
            {
                item.SetMetadataValue("Project", guid);
            }
            if (!string.IsNullOrEmpty(name))
            {
                item.SetMetadataValue("Name", name);
            }

            AddProjectReference(new ProjectReference(this, path, guid, name));
        }

        public void AddProjectReference(ProjectReference projectReference)
        {
            projectReferences.Add(projectReference);
            importerInfoRegistry.Register(projectReference);
            processorInfoRegistry.Register(projectReference);
        }

        public void ClearProjectReferences()
        {
            foreach (var projectReference in projectReferences)
            {
                projectReference.UnloadReferencedProject();
            }

            project.RemoveItems(project.GetItems("ProjectReference"));
            projectReferences.Clear();
        }

        public IEnumerable<ProjectReference> EnumerateProjectReferences()
        {
            return projectReferences;
        }

        public Asset AddAsset(FileInfo file, AssetBuildActionKind buidAction, string name, string importer, string processor)
        {
            if (file == null) throw new ArgumentNullException("file");

            var assetPath = ResolveAssetPath(file);
            
            AddItem(buidAction.ToString(), assetPath, name, importer, processor);

            return GetAsset(file);
        }

        void AddItem(string itemType, string path, string name, string importer, string processor)
        {
            var item = project.AddItem(itemType, path)[0];

            item.SetMetadataValue("Link", Path.GetFileName(path));
            item.SetMetadataValue("Name", name);

            if (!string.IsNullOrEmpty(importer))
            {
                item.SetMetadataValue("Importer", importer);
            }

            if (!string.IsNullOrEmpty(processor))
            {
                item.SetMetadataValue("Processor", processor);
            }

            Tracer.TraceSource.TraceInformation("Asset added:");
            Tracer.TraceSource.TraceInformation("    Type     : {0}", itemType);
            Tracer.TraceSource.TraceInformation("    Path     : {0}", path);
            Tracer.TraceSource.TraceInformation("    AssetName: {0}", name);
            Tracer.TraceSource.TraceInformation("    Importer : {0}", importer);
            Tracer.TraceSource.TraceInformation("    Processor: {0}", processor);
        }

        public Asset GetAsset(FileInfo file)
        {
            if (file == null) throw new ArgumentNullException("file");

            var item = GetAssetProjectItem(file);
            if (item == null) return null;

            return ToAsset(file, item);
        }

        Asset ToAsset(FileInfo file, ProjectItem item)
        {
            var buildAction = (AssetBuildActionKind) Enum.Parse(typeof(AssetBuildActionKind), item.ItemType);
            var name = item.GetMetadataValue("Name");
            var importer = item.GetMetadataValue("Importer");
            var processor = item.GetMetadataValue("Processor");
            if (processor == "PassThroughProcessor") processor = null;

            return new Asset(this, file, buildAction, name, importer, processor);
        }

        public void DeleteAsset(FileInfo file)
        {
            if (file == null) throw new ArgumentNullException("file");

            var item = GetAssetProjectItem(file);
            project.RemoveItem(item);
        }

        ProjectItem GetAssetProjectItem(FileInfo file)
        {
            var assetPath = ResolveAssetPath(file);
            return project.GetItemsByEvaluatedInclude(assetPath).SingleOrDefault();
        }

        string ResolveAssetPath(FileInfo file)
        {
            // プロジェクト ファイルのアセット定義では '\\' を区切り文字として扱いますが、
            // Uri クラスは区切り文字が '/' であることに加えて URI エンコードを施すため、
            // Uri.MakeRelativeUri を用いた相対パス計算ではその差異についての加工処理が必要です。
            // ここでは、通常、指定されるファイルがコンテンツ ディレクトリ下のものであると仮定し、
            // 単純な文字列比較による相対パス計算を行うことにします。

            if (!file.FullName.StartsWith(DirectoryPath)) return file.FullName;

            var directoryPath = DirectoryPath;
            if (!directoryPath.EndsWith("\\"))
            {
                directoryPath += "\\";
            }

            return file.FullName.Substring(directoryPath.Length, file.FullName.Length - directoryPath.Length);
        }

        /// <summary>
        /// すべてのコンテンツ ファイルを MSBuild プロジェクトから削除します。
        /// </summary>
        public void ClearAssets()
        {
            project.RemoveItems(project.GetItems(AssetBuildActionKind.Compile.ToString()));
            project.RemoveItems(project.GetItems(AssetBuildActionKind.None.ToString()));
        }

        /// <summary>
        /// プロジェクトに追加したすべてのコンテンツ ファイルを非同期呼び出しでビルドします。
        /// </summary>
        public BuildSubmission BuildAsync()
        {
            return BuildAsync(null);
        }

        /// <summary>
        /// プロジェクトに追加したすべてのコンテンツ ファイルを非同期呼び出しでビルドします。
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public BuildSubmission BuildAsync(BuildCallback callback)
        {
            BuildManager.DefaultBuildManager.BeginBuild(buildParameters);

            var submission = CreateBuildSubmission();

            // 非同期呼び出しでビルドします。
            CreateBuildSubmission().ExecuteAsync(
                (BuildSubmissionCompleteCallback) delegate(BuildSubmission s)
                {
                    BuildManager.DefaultBuildManager.EndBuild();
                    if (callback != null)
                    {
                        callback(s);
                    }
                }, null);

            return submission;
        }

        /// <summary>
        /// プロジェクトに追加したすべてのコンテンツ ファイルを同期呼び出しでビルドします。
        /// </summary>
        /// <returns>
        /// エラー メッセージ (ビルド エラーが発生した場合)、null (それ以外の場合)。
        /// </returns>
        public BuildSubmission Build()
        {
            BuildManager.DefaultBuildManager.BeginBuild(buildParameters);

            var submission = CreateBuildSubmission();
            submission.ExecuteAsync(null, null);

            // ビルド終了までスレッドを待機します。
            submission.WaitHandle.WaitOne();

            BuildManager.DefaultBuildManager.EndBuild();

            return submission;
        }

        BuildSubmission CreateBuildSubmission()
        {
            var request = new BuildRequestData(project.CreateProjectInstance(), new string[0]);
            return BuildManager.DefaultBuildManager.PendBuildRequest(request);
        }
    }
}

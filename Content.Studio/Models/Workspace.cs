#region Using

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework.Content;
using Willcraftia.Content.Studio.PluginFramework;
using Willcraftia.Win.Framework;
using Willcraftia.Xna.Framework.Content.Build;

#endregion

namespace Willcraftia.Content.Studio.Models
{
    public sealed class Workspace
    {
        public static Workspace Current { get; set; }

        PluginSettings pluginSettings = new PluginSettings();
        PluginManager pluginManager = new PluginManager();

        public ContentProject Project { get; private set; }

        public ContentManager ContentManager { get; private set; }

        public ServiceContainer Services { get; private set; }

        public ContentFolder RootFolder { get; private set; }

        public ContentFolder CurrentFolder { get; set; }

        BuildLogger buildLogger;

        static Workspace()
        {
            Current = new Workspace();
        }

        public Workspace()
        {
            Services = new ServiceContainer();
            Services.AddService<IIconReaderService>(new IconReaderService());
            buildLogger = new BuildLogger(Tracer.TraceSource);

            LoadPlugins();

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        }

        void LoadPlugins()
        {
            // プラグインをロードします。
            foreach (var assemblyFile in pluginSettings.AssemblyFiles)
            {
                pluginManager.Register(assemblyFile);
            }

            // Converter を初期化します。
            //ContentImporterConverter.Initialize(PluginManager.ContentImporterRegistry.Importers.Keys);
            //ContentProcessorConverter.Initialize(PluginManager.ContentProcessorRegistry.Processors.Keys);
        }

        public void NewProject(string path)
        {
            if (path == null) throw new ArgumentNullException("path");

            // プロジェクトが既に開かれているならば、それを閉じます。
            CloseProject();

            // プロジェクトを作成します。
            Project = ContentProject.Create(path, buildLogger);
            Project.OutputPath = AppDomain.CurrentDomain.BaseDirectory;

            // このプロジェクトのコンテンツをロードする AdhocContentManager を生成します。
            ContentManager = new AdhocContentManager(Services, Project.RuntimeContentPath);

            // ルート ディレクトリを設定します。
            RootFolder = new ContentFolder(new DirectoryInfo(Project.DirectoryPath));
        }

        /// <summary>
        /// 指定されたプロジェクト ファイルを開きます。
        /// </summary>
        /// <param name="path">プロジェクト ファイルのパス。</param>
        public void OpenProject(string path)
        {
            if (path == null) throw new ArgumentNullException("path");

            // プロジェクトが既に開かれているならば、それを閉じます。
            // 同一プロジェクトを選択した場合にはロード状態の衝突が発生するため、
            // 選択したプロジェクトを開く前に既存のプロジェクトを閉じます。
            CloseProject();

            // プロジェクトを開きます。
            Project = ContentProject.Load(path, buildLogger);
            Project.OutputPath = AppDomain.CurrentDomain.BaseDirectory;
            // 出力先の変更により IsDirty=true となるため、再評価します。
            Project.ReevaluateIfNecessary();

            // このプロジェクトのコンテンツをロードする AdhocContentManager を生成します。
            ContentManager = new AdhocContentManager(Services, Project.RuntimeContentPath);

            // ルート ディレクトリを設定します。
            RootFolder = new ContentFolder(new DirectoryInfo(Project.DirectoryPath));
        }

        /// <summary>
        /// 現在開かれているプロジェクトを閉じます。
        /// </summary>
        public void CloseProject()
        {
            if (Project == null) return;

            // プロジェクトをアンロードします。
            Project.Unload();
            Project = null;

            // コンテンツ マネージャをアンロードします。
            if (ContentManager != null)
            {
                ContentManager.Unload();
            }

            // ディレクトリ階層をリセットします。
            RootFolder = null;
            CurrentFolder = null;
            // ファイル リストをリセットします。
            //currentFiles.Clear();
            //currentFile = null;
        }

        public void SaveProject()
        {
            if (Project == null) return;

            Project.Save();
        }

        public void SaveProjectAs(string path)
        {
            if (Project == null) return;

            Project.Save(path);
        }

        public void BuildProject()
        {
            if (Project == null) return;

            Project.BuildAsync();
        }

        public PluginSettingsEdit CreatePluginSettingsEdit()
        {
            return new PluginSettingsEdit(pluginSettings);
        }

        public ContentManager CreateContentManager(IServiceProvider services)
        {
            if (Project == null) throw new InvalidOperationException("No project is opened.");

            return new AdhocContentManager(services, Project.RuntimeContentPath);
        }

        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return GetAssembly(args.Name);
        }

        Assembly GetAssembly(string assemblyName)
        {
            return GetAssembly(new AssemblyName(assemblyName));
        }

        Assembly GetAssembly(AssemblyName assemblyName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (IsMatched(assemblyName, assembly))
                {
                    return assembly;
                }
            }

            //if (Project != null)
            //{
            //    foreach (var reference in Project.EnumerateReferences())
            //    {
            //        var assembly = reference.LoadAssembly();
            //        if (IsMatched(assemblyName, assembly))
            //        {
            //            return assembly;
            //        }
            //    }
            //    foreach (var projectReference in Project.EnumerateProjectReferences())
            //    {
            //        var assembly = projectReference.LoadAssembly();
            //        if (IsMatched(assemblyName, assembly))
            //        {
            //            return assembly;
            //        }
            //    }
            //}

            return null;
        }

        bool IsMatched(AssemblyName assemblyName, Assembly assembly)
        {
            var targetAssemblyName = assembly.GetName();

            if (targetAssemblyName.Equals(assemblyName))
            {
                return true;
            }

            if (targetAssemblyName.Name == assemblyName.Name &&
                targetAssemblyName.Version.Equals(assemblyName.Version) &&
                targetAssemblyName.CultureInfo.Equals(assemblyName.CultureInfo))
            {
                return true;
            }

            if (targetAssemblyName.Name == assemblyName.Name &&
                targetAssemblyName.Version.Equals(assemblyName.Version))
            {
                return true;
            }

            if (targetAssemblyName.Name == assemblyName.Name &&
                targetAssemblyName.CultureInfo.Equals(assemblyName.CultureInfo))
            {
                return true;
            }

            if (targetAssemblyName.Name == assemblyName.Name)
            {
                return true;
            }

            return false;
        }
    }
}

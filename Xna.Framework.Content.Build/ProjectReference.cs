#region Using

using System;
using System.Reflection;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;

#endregion

namespace Willcraftia.Xna.Framework.Content.Build
{
    public sealed class ProjectReference : IAssemblyReference
    {
        public ContentProject ContentProject { get; private set; }

        public string Path { get; private set; }

        public string Guid { get; private set; }

        public string Name { get; private set; }

        ProjectRootElement referencedRootElement;

        Project referencedProject;

        public ProjectReference(ContentProject contentProject, string path, string guid, string name)
        {
            if (contentProject == null) throw new ArgumentNullException("contentProject");
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");

            ContentProject = contentProject;
            Path = path;
            Guid = guid;
            Name = name;

            var referencedPath = System.IO.Path.Combine(ContentProject.DirectoryPath, Path);
            referencedRootElement = ProjectRootElement.Open(referencedPath);
            referencedProject = new Project(referencedRootElement);
        }

        public void UnloadReferencedProject()
        {
            ProjectCollection.GlobalProjectCollection.UnloadProject(referencedProject);
        }

        public Assembly LoadAssembly()
        {
            Build();

            var outputDirectoryPath = referencedProject.GetPropertyValue("OutputPath");
            var outputFileName = referencedProject.GetPropertyValue("AssemblyName");
            var outputType = referencedProject.GetPropertyValue("OutputType");
            if ("Library".Equals(outputType))
            {
                outputFileName = outputFileName + ".dll";
            }
            else if ("WinExe".Equals(outputType))
            {
                outputFileName = outputFileName + ".exe";
            }
            else
            {
                throw new InvalidOperationException(string.Format("Unexpected output type: {0}", outputType));
            }

            var projectPath = System.IO.Path.GetDirectoryName(referencedRootElement.FullPath);
            var outputFilePath = System.IO.Path.Combine(projectPath, outputDirectoryPath);
            outputFilePath = System.IO.Path.Combine(outputFilePath, outputFileName);

            return Assembly.LoadFrom(outputFilePath);
        }

        void Build()
        {
            if (!referencedProject.IsDirty) return;

            var buildParameters = new BuildParameters();

            if (ContentProject.Logger != null)
            {
                buildParameters.Loggers = new ILogger[] { ContentProject.Logger };
            }

            BuildManager.DefaultBuildManager.BeginBuild(buildParameters);

            var request = new BuildRequestData(referencedProject.CreateProjectInstance(), new string[0]);
            var submission = BuildManager.DefaultBuildManager.PendBuildRequest(request);
            submission.ExecuteAsync(null, null);

            // ビルド終了までスレッドを待機します。
            submission.WaitHandle.WaitOne();

            BuildManager.DefaultBuildManager.EndBuild();
        }
    }
}

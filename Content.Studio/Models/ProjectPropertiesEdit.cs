#region Using

using System.Collections.ObjectModel;
using Willcraftia.Xna.Framework.Content.Build;

#endregion

namespace Willcraftia.Content.Studio.Models
{
    public sealed class ProjectPropertiesEdit
    {
        public ContentProject Project { get; private set; }

        public ObservableCollection<ReferenceEdit> References { get; private set; }

        public ObservableCollection<ProjectReferenceEdit> ProjectReferences { get; private set; }

        public ProjectPropertiesEdit(ContentProject project)
        {
            Project = project;

            References = new ObservableCollection<ReferenceEdit>();
            foreach (var reference in project.EnumerateReferences())
            {
                References.Add(new ReferenceEdit(reference));
            }

            ProjectReferences = new ObservableCollection<ProjectReferenceEdit>();
            foreach (var projectReference in project.EnumerateProjectReferences())
            {
                ProjectReferences.Add(new ProjectReferenceEdit(projectReference));
            }
        }

        public void Save()
        {
            Project.ClearReferences();
            foreach (var edit in References)
            {
                edit.Register(Project);
            }

            Project.ClearProjectReferences();
            foreach (var edit in ProjectReferences)
            {
                edit.Register(Project);
            }
        }
    }
}

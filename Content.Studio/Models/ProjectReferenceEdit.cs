#region Using

using Willcraftia.Win.Framework;
using Willcraftia.Xna.Framework.Content.Build;

#endregion

namespace Willcraftia.Content.Studio.Models
{
    public sealed class ProjectReferenceEdit : ObservableObject
    {
        string path;
        public string Path
        {
            get { return path; }
            set
            {
                if (path == value) return;

                path = value;
                OnPropertyChanged("Path");

                if (!string.IsNullOrEmpty(path))
                {
                    Name = System.IO.Path.GetFileNameWithoutExtension(path);
                }
            }
        }

        string guid;
        public string Guid
        {
            get { return guid; }
            private set
            {
                if (guid == value) return;

                guid = value;
                OnPropertyChanged("Guid");
            }
        }

        string name;
        public string Name
        {
            get { return name; }
            private set
            {
                if (name == value) return;

                name = value;
                OnPropertyChanged("Name");
            }
        }

        public ProjectReferenceEdit() { }

        public ProjectReferenceEdit(ProjectReference projectReference)
        {
            path = projectReference.Path;
            guid = projectReference.Guid;
            name = projectReference.Name;
        }

        public void Register(ContentProject project)
        {
            project.AddProjectReference(Path, Guid, Name);
        }
    }
}

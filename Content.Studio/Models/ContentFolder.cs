#region Using

using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#endregion

namespace Willcraftia.Content.Studio.Models
{
    public sealed class ContentFolder
    {
        public DirectoryInfo Directory { get; private set; }

        public ContentFolder(DirectoryInfo directory)
        {
            Directory = directory;
        }

        public IEnumerable<ContentFolder> EnumerateFolders()
        {
            var folders = new List<ContentFolder>();
            foreach (var subDirectory in Directory.EnumerateDirectories())
            {
                folders.Add(new ContentFolder(subDirectory));
            }
            return folders;
        }

        public ContentFolder CreateSubFolder(string name)
        {
            var subDirectory = Directory.CreateSubdirectory(name);
            return new ContentFolder(subDirectory);
        }

        public void Delete()
        {
            Directory.Delete(true);
        }

        public IEnumerable<ContentFile> EnumerateFiles()
        {
            var files = new List<ContentFile>();
            foreach (var file in Directory.EnumerateFiles())
            {
                files.Add(new ContentFile(file));
            }
            return files;
        }
    }
}

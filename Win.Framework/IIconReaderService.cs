#region Using

using System.Windows.Media;

#endregion

namespace Willcraftia.Win.Framework
{
    public interface IIconReaderService
    {
        ImageSource GetFileIconImage(string filePath, bool small);

        ImageSource GetFolderIconImage(bool closed, bool small);

        ImageSource GetFolderIconImage(string folderPath, bool closed, bool small);
    }
}

#region Using

#endregion

namespace Willcraftia.Content.Studio.Models
{
    public interface IContentLoader
    {
        object Load(string path);

        bool Save(string path, object content);
    }
}

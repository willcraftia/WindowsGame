#region Using

#endregion

namespace Willcraftia.Content.Studio.PluginFramework
{
    public interface IPluginActivator
    {
        void Load();

        void Unload();
    }
}

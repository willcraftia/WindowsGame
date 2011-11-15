#region Using

using System.Reflection;

#endregion

namespace Willcraftia.Xna.Framework.Content.Build
{
    public interface IAssemblyReference
    {
        Assembly LoadAssembly();
    }
}

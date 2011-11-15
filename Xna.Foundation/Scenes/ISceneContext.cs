#region Using

using Microsoft.Xna.Framework.Content;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes
{
    public interface ISceneContext : IGameContext
    {
        ContentManager Content { get; }
        ContentManager LocalContent { get; }
    }
}

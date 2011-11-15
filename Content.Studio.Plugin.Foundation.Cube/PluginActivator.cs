#region Using

using Willcraftia.Xna.Foundation.Cube.Scenes;
using Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Materials;
using Willcraftia.Xna.Foundation.Content.Pipeline.Cube.StaticMeshes;
using Willcraftia.Content.Studio.PluginFramework;
using Willcraftia.Content.Studio.Plugin.Foundation.Cube.ViewModels;
using Willcraftia.Content.Studio.Plugin.Foundation.Cube.Views;
using Willcraftia.Content.Studio.ViewModels;
using Willcraftia.Content.Studio.Views;

#endregion

namespace Willcraftia.Content.Studio.Plugin.Foundation.Cube
{
    public sealed class PluginActivator : IPluginActivator
    {
        public void Load()
        {
            ContentViewModelRegistry.Instance.Register<CubeMaterial, CubeMaterialViewModel>();
            ContentViewModelRegistry.Instance.Register<CubeBlock, CubeBlockViewModel>();
            ContentViewModelRegistry.Instance.Register<CubeStairs, CubeStairsViewModel>();
            ContentViewModelRegistry.Instance.Register<CubeTile, CubeTileViewModel>();

            ContentControlRegistry.Instance.Register<CubeMaterialViewModel, CubeMaterialControl>();
            ContentControlRegistry.Instance.Register<CubeBlockViewModel, CubeBlockControl>();
            ContentControlRegistry.Instance.Register<CubeStairsViewModel, CubeStairsControl>();
            ContentControlRegistry.Instance.Register<CubeTileViewModel, CubeTileControl>();

            RuntimeContentControlRegistry.Instance.Register<CubeTerrain, RuntimeCubeTerrainControl>();
        }

        public void Unload()
        {
            ContentViewModelRegistry.Instance.Unregister<CubeMaterial>();
            ContentViewModelRegistry.Instance.Unregister<CubeBlock>();
            ContentViewModelRegistry.Instance.Unregister<CubeStairs>();
            ContentViewModelRegistry.Instance.Unregister<CubeTile>();

            ContentControlRegistry.Instance.Unregister<CubeMaterialViewModel>();
            ContentControlRegistry.Instance.Unregister<CubeBlockViewModel>();
            ContentControlRegistry.Instance.Unregister<CubeStairsViewModel>();
            ContentControlRegistry.Instance.Unregister<CubeTileViewModel>();

            RuntimeContentControlRegistry.Instance.Unregister<CubeTerrain>();
        }
    }
}

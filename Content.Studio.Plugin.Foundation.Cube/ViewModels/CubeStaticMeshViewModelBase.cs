#region Using

using System.IO;
using Willcraftia.Xna.Foundation.Content.Pipeline.Cube.StaticMeshes;
using Willcraftia.Content.Studio.Models;
using Willcraftia.Content.Studio.ViewModels;

#endregion

namespace Willcraftia.Content.Studio.Plugin.Foundation.Cube.ViewModels
{
    public class CubeStaticMeshViewModelBase : ContentViewModelBase
    {
        public float UnitScale
        {
            get { return CubeStaticMesh.UnitScale; }
            set
            {
                if (CubeStaticMesh.UnitScale == value) return;

                CubeStaticMesh.UnitScale = value;
                RaisePropertyChanged("UnitScale");
            }
        }

        public int BlockScale
        {
            get { return CubeStaticMesh.BlockScale; }
            set
            {
                if (CubeStaticMesh.BlockScale == value) return;

                CubeStaticMesh.BlockScale = value;
                RaisePropertyChanged("BlockScale");
            }
        }

        CubeStaticMesh CubeStaticMesh
        {
            get { return Model as CubeStaticMesh; }
        }

        protected CubeStaticMeshViewModelBase(FileInfo file)
            : base(file)
        {
        }
    }
}

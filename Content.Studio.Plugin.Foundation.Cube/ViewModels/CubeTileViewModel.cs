#region Using

using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Willcraftia.Content.Studio.Models;
using Willcraftia.Win.Framework;
using Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Materials;
using Willcraftia.Xna.Foundation.Content.Pipeline.Cube.StaticMeshes;

#endregion

namespace Willcraftia.Content.Studio.Plugin.Foundation.Cube.ViewModels
{
    using CubeMaterialReference = ExternalReference<CubeMaterial>;

    public sealed class CubeTileViewModel : CubeStaticMeshViewModelBase
    {
        public int SizeX
        {
            get { return CubeTile.SizeX; }
            set
            {
                if (CubeTile.SizeX == value) return;

                CubeTile.SizeX = value;
                RaisePropertyChanged("SizeX");
            }
        }

        public int SizeZ
        {
            get { return CubeTile.SizeZ; }
            set
            {
                if (CubeTile.SizeZ == value) return;

                CubeTile.SizeZ = value;
                RaisePropertyChanged("SizeZ");
            }
        }

        public CubeMaterialReferenceViewModel Material { get; private set; }

        CubeTile CubeTile
        {
            get { return Model as CubeTile; }
        }

        public CubeTileViewModel(FileInfo file)
            : base(file)
        {
            Material = new CubeMaterialReferenceViewModel(File,
                new PropertyModel<CubeMaterialReference>(Model, "Material"));
        }
    }
}

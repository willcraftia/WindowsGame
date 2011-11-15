#region Using

using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Willcraftia.Content.Studio.Models;
using Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Materials;
using Willcraftia.Xna.Foundation.Content.Pipeline.Cube.StaticMeshes;
using Willcraftia.Win.Framework;

#endregion

namespace Willcraftia.Content.Studio.Plugin.Foundation.Cube.ViewModels
{
    using CubeMaterialReference = ExternalReference<CubeMaterial>;

    public sealed class CubeBlockViewModel : CubeStaticMeshViewModelBase
    {
        public int SizeX
        {
            get { return CubeBlock.SizeX; }
            set
            {
                if (CubeBlock.SizeX == value) return;

                CubeBlock.SizeX = value;
                RaisePropertyChanged("SizeX");
            }
        }

        public int SizeY
        {
            get { return CubeBlock.SizeY; }
            set
            {
                if (CubeBlock.SizeY == value) return;

                CubeBlock.SizeY = value;
                RaisePropertyChanged("SizeY");
            }
        }

        public int SizeZ
        {
            get { return CubeBlock.SizeZ; }
            set
            {
                if (CubeBlock.SizeZ == value) return;

                CubeBlock.SizeZ = value;
                RaisePropertyChanged("SizeZ");
            }
        }

        public CubeMaterialReferenceViewModel BaseMaterial { get; private set; }
        public CubeMaterialReferenceViewModel TopMaterial { get; private set; }
        public CubeMaterialReferenceViewModel BottomMaterial { get; private set; }
        public CubeMaterialReferenceViewModel NorthMaterial { get; private set; }
        public CubeMaterialReferenceViewModel SouthMaterial { get; private set; }
        public CubeMaterialReferenceViewModel WestMaterial { get; private set; }
        public CubeMaterialReferenceViewModel EastMaterial { get; private set; }

        CubeBlock CubeBlock
        {
            get { return Model as CubeBlock; }
        }

        public CubeBlockViewModel(FileInfo file)
            : base(file)
        {
            BaseMaterial = new CubeMaterialReferenceViewModel(
                File, new PropertyModel<CubeMaterialReference>(Model, "BaseMaterial"));
            TopMaterial = new CubeMaterialReferenceViewModel(
                File, new PropertyModel<CubeMaterialReference>(Model, "TopMaterial"));
            BottomMaterial = new CubeMaterialReferenceViewModel(
                File, new PropertyModel<CubeMaterialReference>(Model, "BottomMaterial"));
            NorthMaterial = new CubeMaterialReferenceViewModel(
                File, new PropertyModel<CubeMaterialReference>(Model, "NorthMaterial"));
            SouthMaterial = new CubeMaterialReferenceViewModel(
                File, new PropertyModel<CubeMaterialReference>(Model, "SouthMaterial"));
            WestMaterial = new CubeMaterialReferenceViewModel(
                File, new PropertyModel<CubeMaterialReference>(Model, "WestMaterial"));
            EastMaterial = new CubeMaterialReferenceViewModel(
                File, new PropertyModel<CubeMaterialReference>(Model, "EastMaterial"));
        }
    }
}

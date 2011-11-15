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

    public sealed class CubeStairsViewModel : CubeStaticMeshViewModelBase
    {
        public int StairCount
        {
            get { return CubeStairs.StairCount; }
            set
            {
                if (CubeStairs.StairCount == value) return;

                CubeStairs.StairCount = value;
                RaisePropertyChanged("StairCount");
            }
        }

        public int SizeX
        {
            get { return CubeStairs.SizeX; }
            set
            {
                if (CubeStairs.SizeX == value) return;

                CubeStairs.SizeX = value;
                RaisePropertyChanged("SizeX");
            }
        }

        public int SizeY
        {
            get { return CubeStairs.SizeY; }
            set
            {
                if (CubeStairs.SizeY == value) return;

                CubeStairs.SizeY = value;
                RaisePropertyChanged("SizeY");
            }
        }

        public int SizeZ
        {
            get { return CubeStairs.SizeZ; }
            set
            {
                if (CubeStairs.SizeZ == value) return;

                CubeStairs.SizeZ = value;
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

        CubeStairs CubeStairs
        {
            get { return Model as CubeStairs; }
        }

        public CubeStairsViewModel(FileInfo file)
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

#region Using

using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Willcraftia.Content.Studio.ViewModels;
using Willcraftia.Win.Framework;
using Willcraftia.Xna.Framework.Content.Pipeline.Xml;
using Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Materials;

#endregion

namespace Willcraftia.Content.Studio.Plugin.Foundation.Cube.ViewModels
{
    public sealed class CubeMaterialReferenceViewModel : ExternalReferenceViewModel<CubeMaterial>
    {
        CubeMaterialViewModel material;
        public CubeMaterialViewModel Material
        {
            get { return material; }
            private set
            {
                if (material == value) return;

                material = value;
                RaisePropertyChanged("Material");
            }
        }

        public CubeMaterialReferenceViewModel(FileInfo ownerFile, PropertyModel<ExternalReference<CubeMaterial>> propertyModel)
            : base(ownerFile, propertyModel)
        {
            LoadMaterial();
        }

        protected override void OnModelChanged()
        {
            LoadMaterial();

            base.OnModelChanged();
        }

        protected override void InitializeOpenFileDialogMessage(ReferFileMessage message)
        {
            message.DefaultExt = ".xml";
            message.Filter = "Cube material files (*.xml)|*.xml|All files (*.*)|*.*";

            base.InitializeOpenFileDialogMessage(message);
        }

        void LoadMaterial()
        {
            if (Model == null || string.IsNullOrEmpty(Model.Filename))
            {
                Material = null;
            }
            else
            {
                Material = new CubeMaterialViewModel(new FileInfo(Model.Filename));
            }
        }
    }
}

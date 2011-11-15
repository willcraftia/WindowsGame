#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

#endregion

namespace Willcraftia.Xna.Foundation.Content.Pipeline.CloudLayer
{
    [ContentProcessor(DisplayName = "Willcraftia Cloud Layer Model Processor")]
    public class CloudLayerModelProcessor : ModelProcessor
    {
        const string DefaultEffectFilename = "Effects/CloudLayerModelEffect.fx";

        string effectFilename = DefaultEffectFilename;

        [DisplayName("Effect")]
        [DefaultValue(DefaultEffectFilename)]
        public string EffectFilename
        {
            get { return effectFilename; }
            set { effectFilename = value; }
        }

        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            return base.Process(input, context);
        }

        protected override MaterialContent ConvertMaterial(MaterialContent material, ContentProcessorContext context)
        {
            var customMaterial = new EffectMaterialContent();
            var effectPath = Path.GetFullPath(effectFilename);
            customMaterial.Effect = new ExternalReference<EffectContent>(effectPath);

            var parameters = new OpaqueDataDictionary();
            parameters["ColorKeyColor"] = ColorKeyColor;
            parameters["ColorKeyEnabled"] = ColorKeyEnabled;
            parameters["TextureFormat"] = TextureFormat;
            parameters["GenerateMipmaps"] = GenerateMipmaps;
            parameters["ResizeTexturesToPowerOfTwo"] = ResizeTexturesToPowerOfTwo;
            
            return context.Convert<MaterialContent, MaterialContent>(
                customMaterial, typeof(MaterialProcessor).Name, parameters);
        }
    }
}
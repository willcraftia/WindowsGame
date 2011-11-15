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

namespace Willcraftia.Xna.Foundation.Content.Pipeline.SkyDome
{
    [ContentProcessor(DisplayName = "Willcraftia Sky Dome Model Processor")]
    public class SkyDomeModelProcessor : ModelProcessor
    {
        string skyMapKey = "SkyMap";

        [DisplayName("Sky Map Key")]
        [DefaultValue("SkyMap")]
        public string SkyMapKey
        {
            get { return skyMapKey; }
            set { skyMapKey = value; }
        }

        string effectFilename = "Effects/SkyDomeModelEffect.fx";

        [DisplayName("Effect")]
        [DefaultValue("Effects/SkyDomeModelEffect.fx")]
        public string EffectFilename
        {
            get { return effectFilename; }
            set { effectFilename = value; }
        }

        string effectPath;

        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            effectPath = Path.GetFullPath(effectFilename);
            return base.Process(input, context);
        }

        protected override MaterialContent ConvertMaterial(MaterialContent material, ContentProcessorContext context)
        {
            var customMaterial = new EffectMaterialContent();
            customMaterial.Effect = new ExternalReference<EffectContent>(effectPath);

            //var basicMaterial = (BasicMaterialContent) material;
            //if (basicMaterial.Texture != null)
            //{
            //    customMaterial.Textures.Add(skyMapKey, basicMaterial.Texture);
            //}

            foreach (var texture in material.Textures)
            {
                customMaterial.Textures.Add(texture.Key, texture.Value);
            }

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
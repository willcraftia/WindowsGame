#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Materials
{
    [ContentProcessor]
    [DesignTimeVisible(false)]
    public sealed class CubeMaterialBuilder : ContentProcessor<CubeMaterial, CubeMaterial>
    {
        public override CubeMaterial Process(CubeMaterial input, ContentProcessorContext context)
        {
            return input;
        }

        public MaterialContent Build(ExternalReference<CubeMaterial> input, ContentProcessorContext context)
        {
            var description = context.BuildAndLoadAsset<CubeMaterial, CubeMaterial>(
                input,
                typeof(CubeMaterialBuilder).Name);

            return Build(description, context);
        }

        public MaterialContent Build(CubeMaterial input, ContentProcessorContext context)
        {
            MaterialContent material;
            if (!IsValidEffect(input.Effect))
            {
                material = ProcessBasicMaterialContent(input, context);
            }
            else
            {
                material = ProcessCustumEffectMaterialContent(input, context);
            }
            return material;
        }

        BasicMaterialContent ProcessBasicMaterialContent(CubeMaterial input, ContentProcessorContext context)
        {
            var material = new BasicMaterialContent();

            #region Textures

            if (IsValidCubeTexture(input.Diffuse))
            {
                material.Texture = BuildCubeTexture(input.Diffuse, context);
            }

            #endregion

            #region Alpha

            material.Alpha = input.Alpha;

            #endregion

            #region Vertex colors

            material.DiffuseColor = input.DiffuseColor;
            material.EmissiveColor = input.EmissiveColor;
            material.SpecularColor = input.SpecularColor;
            material.SpecularPower = input.SpecularPower;
            material.VertexColorEnabled = input.VertexColorEnabled;

            #endregion

            return material;
        }

        EffectMaterialContent ProcessCustumEffectMaterialContent(CubeMaterial input, ContentProcessorContext context)
        {
            var material = new EffectMaterialContent();

            material.CompiledEffect = context.BuildAsset<EffectContent, CompiledEffectContent>(
                input.Effect,
                typeof(EffectProcessor).Name);

            #region Textures

            if (IsValidCubeTexture(input.Diffuse))
            {
                material.OpaqueData["DiffuseMap"] = BuildCubeTexture(input.Diffuse, context);
                material.OpaqueData["DiffuseMapEnabled"] = true;
            }

            #endregion

            #region Alpha

            material.OpaqueData["Alpha"] = input.Alpha;

            #endregion

            #region Vertex colors

            material.OpaqueData["DiffuseColor"] = input.DiffuseColor;
            material.OpaqueData["EmissiveColor"] = input.EmissiveColor;
            material.OpaqueData["SpecularColor"] = input.SpecularColor;
            material.OpaqueData["SpecularPower"] = input.SpecularPower;
            material.OpaqueData["VertexColorEnabled"] = input.VertexColorEnabled;

            #endregion

            return material;
        }

        ExternalReference<TextureContent> BuildCubeTexture(CubeTexture sourceAsset, ContentProcessorContext context)
        {
            var parameters = new OpaqueDataDictionary()
            {
                { "Scale", sourceAsset.Scale },
                { "GenerateMipmaps", sourceAsset.GenerateMipmaps },
                { "TextureFormat", sourceAsset.TextureFormat }
            };
            return context.BuildAsset<TextureContent, TextureContent>(
                sourceAsset.Texture,
                typeof(CubeTextureProcessor).Name,
                parameters,
                null,
                null);
        }

        bool IsValidEffect(ExternalReference<EffectContent> effect)
        {
            return effect != null && !string.IsNullOrEmpty(effect.Filename);
        }

        bool IsValidCubeTexture(CubeTexture cubeTexture)
        {
            return cubeTexture != null &&
                cubeTexture.Texture != null &&
                !string.IsNullOrEmpty(cubeTexture.Texture.Filename);
        }
    }
}

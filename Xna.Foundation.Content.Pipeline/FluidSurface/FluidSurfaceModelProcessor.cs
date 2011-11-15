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

namespace Willcraftia.Xna.Foundation.Content.Pipeline.FluidSurface
{
    [ContentProcessor(DisplayName = "FluidSurfaceModel Processor")]
    public sealed class FluidSurfaceModelProcessor : ContentProcessor<FluidSurface, ModelContent>
    {
        const string DefaultEffectFilename = "Effects/FluidSurfaceModelEffect.fx";

        public override ModelContent Process(FluidSurface input, ContentProcessorContext context)
        {
            var meshBuilder = MeshBuilder.StartMesh("Mesh");

            ResolvePositions(meshBuilder);
            ResolveMaterial(meshBuilder, input);
            ResolveVertices(meshBuilder);

            var mesh = meshBuilder.FinishMesh();

            var parameters = new OpaqueDataDictionary();
            return context.Convert<MeshContent, ModelContent>(mesh, typeof(ModelProcessor).Name, parameters);
        }

        void ResolvePositions(MeshBuilder meshBuilder)
        {
            var x0 = -0.5f;
            var x1 = 0.5f;
            var z0 = -0.5f;
            var z1 = 0.5f;

            // 0
            meshBuilder.CreatePosition(x0, 0, z0);
            // 1
            meshBuilder.CreatePosition(x1, 0, z0);
            // 2
            meshBuilder.CreatePosition(x1, 0, z1);
            // 3
            meshBuilder.CreatePosition(x0, 0, z1);
        }

        void ResolveMaterial(MeshBuilder meshBuilder, FluidSurface input)
        {
            var material = new EffectMaterialContent();

            if (input.Effect == null ||
                string.IsNullOrEmpty(input.Effect.Filename))
            {
                var effectPath = Path.GetFullPath(DefaultEffectFilename);
                material.Effect = new ExternalReference<EffectContent>(effectPath);
            }
            else
            {
                material.Effect = input.Effect;
            }

            material.OpaqueData["DiffuseColor"] = input.DiffuseColor;
            material.OpaqueData["EmissiveColor"] = input.EmissiveColor;
            material.OpaqueData["SpecularColor"] = input.SpecularColor;
            material.OpaqueData["SpecularPower"] = input.SpecularPower;
            material.OpaqueData["MinAlpha"] = input.MinAlpha;
            material.OpaqueData["MaxAlpha"] = input.MaxAlpha;
            material.OpaqueData["DistanceAlphaFactor"] = input.DistanceAlphaFactor;
            material.OpaqueData["SpringPower"] = input.SpringPower;

            meshBuilder.SetMaterial(material);
        }

        void ResolveVertices(MeshBuilder meshBuilder)
        {
            var normalChannelId = meshBuilder.CreateVertexChannel<Vector3>(VertexChannelNames.Normal());
            meshBuilder.SetVertexChannelData(normalChannelId, Vector3.Up);

            var texCoordId = meshBuilder.CreateVertexChannel<Vector2>(VertexChannelNames.TextureCoordinate(0));

            //var uv = Scale;
            var uv = 1.0f;

            // first triangle
            meshBuilder.SetVertexChannelData(texCoordId, new Vector2(0, uv));
            meshBuilder.AddTriangleVertex(3);
            meshBuilder.SetVertexChannelData(texCoordId, new Vector2(0, 0));
            meshBuilder.AddTriangleVertex(0);
            meshBuilder.SetVertexChannelData(texCoordId, new Vector2(uv, 0));
            meshBuilder.AddTriangleVertex(1);

            // second triangle
            meshBuilder.SetVertexChannelData(texCoordId, new Vector2(0, uv));
            meshBuilder.AddTriangleVertex(3);
            meshBuilder.SetVertexChannelData(texCoordId, new Vector2(uv, 0));
            meshBuilder.AddTriangleVertex(1);
            meshBuilder.SetVertexChannelData(texCoordId, new Vector2(uv, uv));
            meshBuilder.AddTriangleVertex(2);
        }
    }
}
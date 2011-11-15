#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Materials;

#endregion

namespace Willcraftia.Xna.Foundation.Content.Pipeline.Cube.StaticMeshes
{
    [ContentProcessor(DisplayName = "CubeTile Processor")]
    public class CubeTileProcessor : ContentProcessor<CubeTile, ModelContent>
    {
        #region Inner classes

        public struct IndexTextureNormal
        {
            public int Index;
            public Vector2 UV;
            public Vector3 Normal;
        }

        [ContentProcessor]
        [DesignTimeVisible(false)]
        public class CubeTileModelProcessor : ModelProcessor
        {
            protected override MaterialContent ConvertMaterial(MaterialContent material, ContentProcessorContext context)
            {
                return material;
            }
        }

        #endregion

        #region Fields and Properties

        Vector3 halfSize;
        MaterialContent material;
        List<IndexTextureNormal> indices = new List<IndexTextureNormal>();

        #endregion

        public override ModelContent Process(CubeTile input, ContentProcessorContext context)
        {
            halfSize =
                new Vector3(input.SizeX, 0, input.SizeZ) *
                input.UnitScale *
                (float) input.BlockScale *
                0.5f;

            LoadMaterials(input, context);

            var meshBuilder = MeshBuilder.StartMesh("CubeTile");
            CreatePositions(input, context, meshBuilder);
            CreateVertices(input, context, meshBuilder);
            var mesh = meshBuilder.FinishMesh();

            return context.Convert<MeshContent, ModelContent>(
                mesh,
                typeof(CubeTileModelProcessor).Name);
        }

        bool IsValidCubeMaterial(ExternalReference<CubeMaterial> cubeMaterial)
        {
            return cubeMaterial != null &&
                !string.IsNullOrEmpty(cubeMaterial.Filename);
        }

        void LoadMaterials(CubeTile input, ContentProcessorContext context)
        {
            if (IsValidCubeMaterial(input.Material))
            {
                var builder = new CubeMaterialBuilder();
                material = builder.Build(input.Material, context);
            }
            else
            {
                material = new BasicMaterialContent();
            }
        }

        void CreatePositions(CubeTile input, ContentProcessorContext context, MeshBuilder meshBuilder)
        {
            Vector2 uvScale;
            var index = new IndexTextureNormal();

            uvScale = new Vector2(input.SizeX, input.SizeZ);
            index.Normal = Vector3.Up;

            index.Index = meshBuilder.CreatePosition(-halfSize.X, halfSize.Y, halfSize.Z);
            index.UV = new Vector2(0, 1) * uvScale;
            indices.Add(index);
            index.Index = meshBuilder.CreatePosition(-halfSize.X, halfSize.Y, -halfSize.Z);
            index.UV = new Vector2(0, 0) * uvScale;
            indices.Add(index);
            index.Index = meshBuilder.CreatePosition(halfSize.X, halfSize.Y, -halfSize.Z);
            index.UV = new Vector2(1, 0) * uvScale;
            indices.Add(index);
            index.Index = meshBuilder.CreatePosition(halfSize.X, halfSize.Y, halfSize.Z);
            index.UV = new Vector2(1, 1) * uvScale;
            indices.Add(index);
        }

        void CreateVertices(CubeTile input, ContentProcessorContext context, MeshBuilder meshBuilder)
        {
            var texCoordId = meshBuilder.CreateVertexChannel<Vector2>(VertexChannelNames.TextureCoordinate(0));
            var normalId = meshBuilder.CreateVertexChannel<Vector3>(VertexChannelNames.Normal());

            meshBuilder.SetMaterial(material);
            AddVertices(input, context, meshBuilder, texCoordId, normalId, indices);
        }

        void AddVertices(
            CubeTile input,
            ContentProcessorContext context,
            MeshBuilder meshBuilder,
            int texCoordId,
            int normalId,
            List<IndexTextureNormal> indices)
        {
            for (int i = 0; i < indices.Count; i += 4)
            {
                AddVertex(input, context, meshBuilder, texCoordId, normalId, indices[i]);
                AddVertex(input, context, meshBuilder, texCoordId, normalId, indices[i + 1]);
                AddVertex(input, context, meshBuilder, texCoordId, normalId, indices[i + 2]);

                AddVertex(input, context, meshBuilder, texCoordId, normalId, indices[i]);
                AddVertex(input, context, meshBuilder, texCoordId, normalId, indices[i + 2]);
                AddVertex(input, context, meshBuilder, texCoordId, normalId, indices[i + 3]);
            }
        }

        void AddVertex(
            CubeTile input,
            ContentProcessorContext context,
            MeshBuilder meshBuilder,
            int texCoordId,
            int normalId,
            IndexTextureNormal index)
        {
            meshBuilder.SetVertexChannelData(texCoordId, index.UV);
            meshBuilder.SetVertexChannelData(normalId, index.Normal);
            meshBuilder.AddTriangleVertex(index.Index);
        }
    }
}
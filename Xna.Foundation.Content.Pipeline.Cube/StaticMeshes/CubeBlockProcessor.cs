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
using Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Materials;

#endregion

namespace Willcraftia.Xna.Foundation.Content.Pipeline.Cube.StaticMeshes
{
    [ContentProcessor(DisplayName = "CubeBlock Processor")]
    public class CubeBlockProcessor : ContentProcessor<CubeBlock, ModelContent>
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
        public class CubeBlockModelProcessor : ModelProcessor
        {
            protected override MaterialContent ConvertMaterial(MaterialContent material, ContentProcessorContext context)
            {
                return material;
            }
        }

        #endregion

        #region Fields and Properties

        Vector3 halfSize;
        MaterialContent baseMaterial;
        MaterialContent topMaterial;
        MaterialContent bottomMaterial;
        MaterialContent northMaterial;
        MaterialContent southMaterial;
        MaterialContent westMaterial;
        MaterialContent eastMaterial;
        List<IndexTextureNormal> topIndices = new List<IndexTextureNormal>();
        List<IndexTextureNormal> bottomIndices = new List<IndexTextureNormal>();
        List<IndexTextureNormal> northIndices = new List<IndexTextureNormal>();
        List<IndexTextureNormal> southIndices = new List<IndexTextureNormal>();
        List<IndexTextureNormal> westIndices = new List<IndexTextureNormal>();
        List<IndexTextureNormal> eastIndices = new List<IndexTextureNormal>();

        #endregion

        public override ModelContent Process(CubeBlock input, ContentProcessorContext context)
        {
            halfSize =
                new Vector3(input.SizeX, input.SizeY, input.SizeZ) *
                input.UnitScale *
                (float) input.BlockScale *
                0.5f;

            LoadMaterials(input, context);

            var meshBuilder = MeshBuilder.StartMesh("CubeBlock");
            CreatePositions(input, context, meshBuilder);
            CreateVertices(input, context, meshBuilder);
            var mesh = meshBuilder.FinishMesh();

            return context.Convert<MeshContent, ModelContent>(mesh, typeof(CubeBlockModelProcessor).Name);
        }

        MaterialContent BuildAndLoadMaterialContent(ExternalReference<CubeMaterial> source, ContentProcessorContext context)
        {
            var builder = new CubeMaterialBuilder();
            return builder.Build(source, context);
        }

        bool IsValidCubeMaterial(ExternalReference<CubeMaterial> cubeMaterial)
        {
            return cubeMaterial != null &&
                !string.IsNullOrEmpty(cubeMaterial.Filename);
        }

        void LoadMaterials(CubeBlock input, ContentProcessorContext context)
        {
            if (IsValidCubeMaterial(input.BaseMaterial))
            {
                baseMaterial = BuildAndLoadMaterialContent(input.BaseMaterial, context);
            }
            else
            {
                baseMaterial = new BasicMaterialContent();
            }

            if (IsValidCubeMaterial(input.TopMaterial))
            {
                topMaterial = BuildAndLoadMaterialContent(input.TopMaterial, context);
            }
            else
            {
                topMaterial = baseMaterial;
            }

            if (IsValidCubeMaterial(input.BottomMaterial))
            {
                bottomMaterial = BuildAndLoadMaterialContent(input.BottomMaterial, context);
            }
            else
            {
                bottomMaterial = baseMaterial;
            }

            if (IsValidCubeMaterial(input.NorthMaterial))
            {
                northMaterial = BuildAndLoadMaterialContent(input.NorthMaterial, context);
            }
            else
            {
                northMaterial = baseMaterial;
            }

            if (IsValidCubeMaterial(input.SouthMaterial))
            {
                southMaterial = BuildAndLoadMaterialContent(input.SouthMaterial, context);
            }
            else
            {
                southMaterial = baseMaterial;
            }

            if (IsValidCubeMaterial(input.WestMaterial))
            {
                westMaterial = BuildAndLoadMaterialContent(input.WestMaterial, context);
            }
            else
            {
                westMaterial = baseMaterial;
            }

            if (IsValidCubeMaterial(input.EastMaterial))
            {
                eastMaterial = BuildAndLoadMaterialContent(input.EastMaterial, context);
            }
            else
            {
                eastMaterial = baseMaterial;
            }
        }

        void CreatePositions(CubeBlock input, ContentProcessorContext context, MeshBuilder meshBuilder)
        {
            Vector2 uvScale;
            var index = new IndexTextureNormal();

            #region Top
            uvScale = new Vector2(input.SizeX, input.SizeZ);
            index.Normal = Vector3.Up;

            index.Index = meshBuilder.CreatePosition(-halfSize.X, halfSize.Y, halfSize.Z);
            index.UV = new Vector2(0, 1) * uvScale;
            topIndices.Add(index);
            index.Index = meshBuilder.CreatePosition(-halfSize.X, halfSize.Y, -halfSize.Z);
            index.UV = new Vector2(0, 0) * uvScale;
            topIndices.Add(index);
            index.Index = meshBuilder.CreatePosition(halfSize.X, halfSize.Y, -halfSize.Z);
            index.UV = new Vector2(1, 0) * uvScale;
            topIndices.Add(index);
            index.Index = meshBuilder.CreatePosition(halfSize.X, halfSize.Y, halfSize.Z);
            index.UV = new Vector2(1, 1) * uvScale;
            topIndices.Add(index);
            #endregion

            #region Bottom
            uvScale = new Vector2(input.SizeX, input.SizeZ);
            index.Normal = Vector3.Down;

            index.Index = meshBuilder.CreatePosition(halfSize.X, -halfSize.Y, halfSize.Z);
            index.UV = new Vector2(0, 1) * uvScale;
            bottomIndices.Add(index);
            index.Index = meshBuilder.CreatePosition(halfSize.X, -halfSize.Y, -halfSize.Z);
            index.UV = new Vector2(0, 0) * uvScale;
            bottomIndices.Add(index);
            index.Index = meshBuilder.CreatePosition(-halfSize.X, -halfSize.Y, -halfSize.Z);
            index.UV = new Vector2(1, 0) * uvScale;
            bottomIndices.Add(index);
            index.Index = meshBuilder.CreatePosition(-halfSize.X, -halfSize.Y, halfSize.Z);
            index.UV = new Vector2(1, 1) * uvScale;
            bottomIndices.Add(index);
            #endregion

            #region North
            uvScale = new Vector2(input.SizeX, input.SizeY);
            index.Normal = Vector3.Forward;

            index.Index = meshBuilder.CreatePosition(halfSize.X, -halfSize.Y, -halfSize.Z);
            index.UV = new Vector2(0, 1) * uvScale;
            northIndices.Add(index);
            index.Index = meshBuilder.CreatePosition(halfSize.X, halfSize.Y, -halfSize.Z);
            index.UV = new Vector2(0, 0) * uvScale;
            northIndices.Add(index);
            index.Index = meshBuilder.CreatePosition(-halfSize.X, halfSize.Y, -halfSize.Z);
            index.UV = new Vector2(1, 0) * uvScale;
            northIndices.Add(index);
            index.Index = meshBuilder.CreatePosition(-halfSize.X, -halfSize.Y, -halfSize.Z);
            index.UV = new Vector2(1, 1) * uvScale;
            northIndices.Add(index);
            #endregion

            #region South
            uvScale = new Vector2(input.SizeX, input.SizeY);
            index.Normal = Vector3.Backward;

            index.Index = meshBuilder.CreatePosition(-halfSize.X, -halfSize.Y, halfSize.Z);
            index.UV = new Vector2(0, 1) * uvScale;
            southIndices.Add(index);
            index.Index = meshBuilder.CreatePosition(-halfSize.X, halfSize.Y, halfSize.Z);
            index.UV = new Vector2(0, 0) * uvScale;
            southIndices.Add(index);
            index.Index = meshBuilder.CreatePosition(halfSize.X, halfSize.Y, halfSize.Z);
            index.UV = new Vector2(1, 0) * uvScale;
            southIndices.Add(index);
            index.Index = meshBuilder.CreatePosition(halfSize.X, -halfSize.Y, halfSize.Z);
            index.UV = new Vector2(1, 1) * uvScale;
            southIndices.Add(index);
            #endregion

            #region West
            uvScale = new Vector2(input.SizeZ, input.SizeY);
            index.Normal = Vector3.Left;

            index.Index = meshBuilder.CreatePosition(-halfSize.X, -halfSize.Y, -halfSize.Z);
            index.UV = new Vector2(0, 1) * uvScale;
            westIndices.Add(index);
            index.Index = meshBuilder.CreatePosition(-halfSize.X, halfSize.Y, -halfSize.Z);
            index.UV = new Vector2(0, 0) * uvScale;
            westIndices.Add(index);
            index.Index = meshBuilder.CreatePosition(-halfSize.X, halfSize.Y, halfSize.Z);
            index.UV = new Vector2(1, 0) * uvScale;
            westIndices.Add(index);
            index.Index = meshBuilder.CreatePosition(-halfSize.X, -halfSize.Y, halfSize.Z);
            index.UV = new Vector2(1, 1) * uvScale;
            westIndices.Add(index);
            #endregion

            #region East
            uvScale = new Vector2(input.SizeZ, input.SizeY);
            index.Normal = Vector3.Right;

            index.Index = meshBuilder.CreatePosition(halfSize.X, -halfSize.Y, halfSize.Z);
            index.UV = new Vector2(0, 1) * uvScale;
            eastIndices.Add(index);
            index.Index = meshBuilder.CreatePosition(halfSize.X, halfSize.Y, halfSize.Z);
            index.UV = new Vector2(0, 0) * uvScale;
            eastIndices.Add(index);
            index.Index = meshBuilder.CreatePosition(halfSize.X, halfSize.Y, -halfSize.Z);
            index.UV = new Vector2(1, 0) * uvScale;
            eastIndices.Add(index);
            index.Index = meshBuilder.CreatePosition(halfSize.X, -halfSize.Y, -halfSize.Z);
            index.UV = new Vector2(1, 1) * uvScale;
            eastIndices.Add(index);
            #endregion
        }

        void CreateVertices(CubeBlock input, ContentProcessorContext context, MeshBuilder meshBuilder)
        {
            var texCoordId = meshBuilder.CreateVertexChannel<Vector2>(VertexChannelNames.TextureCoordinate(0));
            var normalId = meshBuilder.CreateVertexChannel<Vector3>(VertexChannelNames.Normal());

            meshBuilder.SetMaterial(topMaterial);
            AddVertices(input, context, meshBuilder, texCoordId, normalId, topIndices);

            meshBuilder.SetMaterial(bottomMaterial);
            AddVertices(input, context, meshBuilder, texCoordId, normalId, bottomIndices);

            meshBuilder.SetMaterial(northMaterial);
            AddVertices(input, context, meshBuilder, texCoordId, normalId, northIndices);

            meshBuilder.SetMaterial(southMaterial);
            AddVertices(input, context, meshBuilder, texCoordId, normalId, southIndices);

            meshBuilder.SetMaterial(westMaterial);
            AddVertices(input, context, meshBuilder, texCoordId, normalId, westIndices);

            meshBuilder.SetMaterial(eastMaterial);
            AddVertices(input, context, meshBuilder, texCoordId, normalId, eastIndices);
        }

        void AddVertices(
            CubeBlock input,
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
            CubeBlock input,
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
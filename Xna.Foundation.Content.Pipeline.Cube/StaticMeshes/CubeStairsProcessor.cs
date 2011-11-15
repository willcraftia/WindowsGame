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
    [ContentProcessor(DisplayName = "CubeStairs Processor")]
    public class CubeStairsProcessor : ContentProcessor<CubeStairs, ModelContent>
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
        public class CubeStairsModelProcessor : ModelProcessor
        {
            protected override MaterialContent ConvertMaterial(MaterialContent material, ContentProcessorContext context)
            {
                return material;
            }
        }

        #endregion

        #region Fields and Properties

        float zyRatio;
        Vector3 halfSize;
        float stairWidth;
        float stairHeight;
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

        public override ModelContent Process(CubeStairs input, ContentProcessorContext context)
        {
            zyRatio = ((float) input.SizeY) / ((float) input.SizeZ);
            halfSize =
                new Vector3(input.SizeX, input.SizeY, input.SizeZ) *
                input.UnitScale *
                (float) input.BlockScale *
                0.5f;
            stairWidth = (input.BlockScale * input.UnitScale) / input.StairCount;
            stairHeight = zyRatio * (input.BlockScale * input.UnitScale) / input.StairCount;

            LoadMaterials(input, context);

            var meshBuilder = MeshBuilder.StartMesh("CubeStairs");
            CreatePositions(input, context, meshBuilder);
            CreateVertices(input, context, meshBuilder);
            var mesh = meshBuilder.FinishMesh();

            return context.Convert<MeshContent, ModelContent>(
                mesh,
                typeof(CubeStairsModelProcessor).Name);
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

        void LoadMaterials(CubeStairs input, ContentProcessorContext context)
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
                ;
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

        void CreateTopPositions(CubeStairs input, ContentProcessorContext context, MeshBuilder meshBuilder)
        {
            var index = new IndexTextureNormal();
            index.Normal = Vector3.Up;

            float stairY = halfSize.Y;
            float startZ = -halfSize.Z;
            float endZ = -halfSize.Z + stairWidth;
            float vStep = 1.0f / ((float) input.StairCount);

            for (int i = 0; i < input.StairCount * input.SizeZ; i++)
            {
                index.Index = meshBuilder.CreatePosition(-halfSize.X, stairY, endZ);
                index.UV = new Vector2(0, (i + 1) * vStep);
                topIndices.Add(index);
                index.Index = meshBuilder.CreatePosition(-halfSize.X, stairY, startZ);
                index.UV = new Vector2(0, i * vStep);
                topIndices.Add(index);
                index.Index = meshBuilder.CreatePosition(halfSize.X, stairY, startZ);
                index.UV = new Vector2(input.SizeX, i * vStep);
                topIndices.Add(index);
                index.Index = meshBuilder.CreatePosition(halfSize.X, stairY, endZ);
                index.UV = new Vector2(input.SizeX, (i + 1) * vStep);
                topIndices.Add(index);

                stairY -= stairHeight;
                startZ += stairWidth;
                endZ += stairWidth;
            }
        }

        void CreateBottomPositions(CubeStairs input, ContentProcessorContext context, MeshBuilder meshBuilder)
        {
            var uvScale = new Vector2(input.SizeX, input.SizeZ);
            var index = new IndexTextureNormal();
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
        }

        void CreateNorthPositions(CubeStairs input, ContentProcessorContext context, MeshBuilder meshBuilder)
        {
            var uvScale = new Vector2(input.SizeX, input.SizeY);
            var index = new IndexTextureNormal();
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
        }

        void CreateSouthPositions(CubeStairs input, ContentProcessorContext context, MeshBuilder meshBuilder)
        {
            var index = new IndexTextureNormal();
            index.Normal = Vector3.Backward;

            float startY = halfSize.Y;
            float endY = halfSize.Y - stairHeight;
            float stairZ = -halfSize.Z + stairWidth;
            float vStep = 1.0f / ((float) input.StairCount);

            for (int i = 0; i < input.StairCount * input.SizeZ; i++)
            {
                index.Index = meshBuilder.CreatePosition(-halfSize.X, endY, stairZ);
                index.UV = new Vector2(0, (i + 1) * vStep);
                southIndices.Add(index);
                index.Index = meshBuilder.CreatePosition(-halfSize.X, startY, stairZ);
                index.UV = new Vector2(0, i * vStep);
                southIndices.Add(index);
                index.Index = meshBuilder.CreatePosition(halfSize.X, startY, stairZ);
                index.UV = new Vector2(input.SizeX, i * vStep);
                southIndices.Add(index);
                index.Index = meshBuilder.CreatePosition(halfSize.X, endY, stairZ);
                index.UV = new Vector2(input.SizeX, (i + 1) * vStep);
                southIndices.Add(index);

                startY -= stairHeight;
                endY -= stairHeight;
                stairZ += stairWidth;
            }
        }

        void CreateWestPositions(CubeStairs input, ContentProcessorContext context, MeshBuilder meshBuilder)
        {
            var index = new IndexTextureNormal();
            index.Normal = Vector3.Left;

            float startZ = -halfSize.Z;
            float endZ = startZ + stairWidth;
            float startY = halfSize.Y;
            float uStep = 1.0f / ((float) input.StairCount);
            float vStep = 1.0f / ((float) input.StairCount) * zyRatio;

            for (int i = 0; i < input.StairCount * input.SizeZ; i++)
            {
                index.Index = meshBuilder.CreatePosition(-halfSize.X, -halfSize.Y, startZ);
                index.UV = new Vector2(i * uStep, input.SizeY);
                westIndices.Add(index);
                index.Index = meshBuilder.CreatePosition(-halfSize.X, startY, startZ);
                index.UV = new Vector2(i * uStep, i * vStep);
                westIndices.Add(index);
                index.Index = meshBuilder.CreatePosition(-halfSize.X, startY, endZ);
                index.UV = new Vector2((i + 1) * uStep, i * vStep);
                westIndices.Add(index);
                index.Index = meshBuilder.CreatePosition(-halfSize.X, -halfSize.Y, endZ);
                index.UV = new Vector2((i + 1) * uStep, input.SizeY);
                westIndices.Add(index);

                startZ += stairWidth;
                endZ += stairWidth;
                startY -= stairHeight;
            }
        }

        void CreateEastPositions(CubeStairs input, ContentProcessorContext context, MeshBuilder meshBuilder)
        {
            var index = new IndexTextureNormal();
            index.Normal = Vector3.Left;

            float startZ = halfSize.Z;
            float endZ = startZ - stairWidth;
            float endY = -halfSize.Y + stairHeight;
            float uStep = 1.0f / ((float) input.StairCount);
            float vStep = 1.0f / ((float) input.StairCount) * zyRatio;

            int len = input.StairCount * input.SizeZ;
            for (int i = 0; i < len; i++)
            {
                index.Index = meshBuilder.CreatePosition(halfSize.X, -halfSize.Y, startZ);
                index.UV = new Vector2(i * uStep, input.SizeY);
                westIndices.Add(index);
                index.Index = meshBuilder.CreatePosition(halfSize.X, endY, startZ);
                index.UV = new Vector2(i * uStep, (len - i - 1) * vStep);
                westIndices.Add(index);
                index.Index = meshBuilder.CreatePosition(halfSize.X, endY, endZ);
                index.UV = new Vector2((i + 1) * uStep, (len - i - 1) * vStep);
                westIndices.Add(index);
                index.Index = meshBuilder.CreatePosition(halfSize.X, -halfSize.Y, endZ);
                index.UV = new Vector2((i + 1) * uStep, input.SizeY);
                westIndices.Add(index);

                startZ -= stairWidth;
                endZ -= stairWidth;
                endY += stairHeight;
            }
        }

        void CreatePositions(CubeStairs input, ContentProcessorContext context, MeshBuilder meshBuilder)
        {
            CreateTopPositions(input, context, meshBuilder);
            CreateBottomPositions(input, context, meshBuilder);
            CreateNorthPositions(input, context, meshBuilder);
            CreateSouthPositions(input, context, meshBuilder);
            CreateWestPositions(input, context, meshBuilder);
            CreateEastPositions(input, context, meshBuilder);
        }

        void CreateVertices(CubeStairs input, ContentProcessorContext context, MeshBuilder meshBuilder)
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
            CubeStairs input,
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
            CubeStairs input,
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
#region Using

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Materials;

#endregion

namespace Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Terrains
{
    [ContentProcessor(DisplayName = "CubeTerrain Processor")]
    public sealed class CubeTerrainProcessor : ContentProcessor<CubeTerrain, CubeTerrainContent>
    {
        #region Inner classes

        public struct IndexInfo
        {
            public int Index;
            public Vector2 UV;
            public Vector3 Normal;
        }

        #endregion

        bool MergeDuplicatePositions = true;
        float MergePositionTolerance = 0.0f;

        PixelBitmapContent<float> heights;
        PixelBitmapContent<Color> alphamap0;
        PixelBitmapContent<Color> alphamap1;
        PixelBitmapContent<Color> alphamap2;
        PixelBitmapContent<Color> alphamap3;

        MaterialContent baseMaterial = new BasicMaterialContent();
        MaterialContent layerMaterial0;
        MaterialContent layerMaterial1;
        MaterialContent layerMaterial2;
        MaterialContent layerMaterial3;
        MaterialContent topMaterial0;
        MaterialContent topMaterial1;
        MaterialContent topMaterial2;
        MaterialContent topMaterial3;
        MaterialContent sideMaterial0;
        MaterialContent sideMaterial1;
        MaterialContent sideMaterial2;
        MaterialContent sideMaterial3;

        List<IndexInfo> junkIndices = new List<IndexInfo>();
        List<IndexInfo> layerIndices0 = new List<IndexInfo>();
        List<IndexInfo> layerIndices1 = new List<IndexInfo>();
        List<IndexInfo> layerIndices2 = new List<IndexInfo>();
        List<IndexInfo> layerIndices3 = new List<IndexInfo>();
        List<IndexInfo> topIndices0 = new List<IndexInfo>();
        List<IndexInfo> topIndices1 = new List<IndexInfo>();
        List<IndexInfo> topIndices2 = new List<IndexInfo>();
        List<IndexInfo> topIndices3 = new List<IndexInfo>();
        List<IndexInfo> sideIndices0 = new List<IndexInfo>();
        List<IndexInfo> sideIndices1 = new List<IndexInfo>();
        List<IndexInfo> sideIndices2 = new List<IndexInfo>();
        List<IndexInfo> sideIndices3 = new List<IndexInfo>();

        PixelBitmapContent<Color> LoadAlphamaps(CubeTerrainLayer input, ContentProcessorContext context)
        {
            if (input != null &&
                input.Alphamap != null &&
                !string.IsNullOrEmpty(input.Alphamap.Filename))
            {
                var texture = context.BuildAndLoadAsset<TextureContent, Texture2DContent>(
                    input.Alphamap,
                    null);
                texture.ConvertBitmapType(typeof(PixelBitmapContent<Color>));
                return (PixelBitmapContent<Color>) texture.Mipmaps[0];
            }
            return null;
        }

        void LoadAlphamaps(CubeTerrain input, ContentProcessorContext context)
        {
            alphamap0 = LoadAlphamaps(input.Layer0, context);
            alphamap1 = LoadAlphamaps(input.Layer1, context);
            alphamap2 = LoadAlphamaps(input.Layer2, context);
            alphamap3 = LoadAlphamaps(input.Layer3, context);
        }

        MaterialContent BuildAndLoadCubeMaterialContent(ExternalReference<CubeMaterial> source, ContentProcessorContext context)
        {
            var builder = new CubeMaterialBuilder();
            return builder.Build(source, context);
        }

        MaterialContent BuildAndLoadCubeMaterialContent(CubeTerrainLayer input, ContentProcessorContext context)
        {
            if (input != null &&
                input.Material != null &&
                !string.IsNullOrEmpty(input.Material.Filename))
            {
                return BuildAndLoadCubeMaterialContent(input.Material, context);
            }
            else
            {
                return baseMaterial;
            }
        }

        MaterialContent BuildAndLoadAltitudeTopCubeMaterialContent(CubeTerrainAltitude input, ContentProcessorContext context)
        {
            if (input != null &&
                input.TopMaterial != null &&
                !string.IsNullOrEmpty(input.TopMaterial.Filename))
            {
                return BuildAndLoadCubeMaterialContent(input.TopMaterial, context);
            }
            else
            {
                return baseMaterial;
            }
        }

        MaterialContent BuildAndLoadAltitudeSideCubeMaterialContent(CubeTerrainAltitude input, ContentProcessorContext context)
        {
            if (input != null &&
                input.SideMaterial != null &&
                !string.IsNullOrEmpty(input.SideMaterial.Filename))
            {
                return BuildAndLoadCubeMaterialContent(input.SideMaterial, context);
            }
            else
            {
                return baseMaterial;
            }
        }

        void LoadMaterials(CubeTerrain input, ContentProcessorContext context)
        {
            #region Layers
            
            layerMaterial0 = BuildAndLoadCubeMaterialContent(input.Layer0, context);
            layerMaterial1 = BuildAndLoadCubeMaterialContent(input.Layer1, context);
            layerMaterial2 = BuildAndLoadCubeMaterialContent(input.Layer2, context);
            layerMaterial3 = BuildAndLoadCubeMaterialContent(input.Layer3, context);
            
            #endregion

            #region Altitude

            topMaterial0 = BuildAndLoadAltitudeTopCubeMaterialContent(input.Altitude0, context);
            sideMaterial0 = BuildAndLoadAltitudeSideCubeMaterialContent(input.Altitude0, context);

            topMaterial1 = BuildAndLoadAltitudeTopCubeMaterialContent(input.Altitude1, context);
            sideMaterial1 = BuildAndLoadAltitudeSideCubeMaterialContent(input.Altitude1, context);

            topMaterial2 = BuildAndLoadAltitudeTopCubeMaterialContent(input.Altitude2, context);
            sideMaterial2 = BuildAndLoadAltitudeSideCubeMaterialContent(input.Altitude2, context);

            topMaterial3 = BuildAndLoadAltitudeTopCubeMaterialContent(input.Altitude3, context);
            sideMaterial3 = BuildAndLoadAltitudeSideCubeMaterialContent(input.Altitude3, context);

            #endregion
        }

        public override CubeTerrainContent Process(CubeTerrain input, ContentProcessorContext context)
        {
            LoadAlphamaps(input, context);
            LoadMaterials(input, context);

            var terrain = new CubeTerrainContent();

            var meshBuilder = MeshBuilder.StartMesh("CubicTerrain");
            meshBuilder.MergeDuplicatePositions = MergeDuplicatePositions;
            meshBuilder.MergePositionTolerance = MergePositionTolerance;

            var heightmap = context.BuildAndLoadAsset<TextureContent, Texture2DContent>(
                input.Heightmap,
                null);

            heightmap.ConvertBitmapType(typeof(PixelBitmapContent<float>));
            heights = (PixelBitmapContent<float>) heightmap.Mipmaps[0];

            ResolvePositions(input, context, meshBuilder);
            ResolveVertices(input, context, meshBuilder);

            var mesh = meshBuilder.FinishMesh();

            terrain.Model = context.Convert<MeshContent, ModelContent>(
                mesh,
                typeof(CubeTerrainModelProcessor).Name);

            terrain.Heightmap = new CubeHeightmapContent(
                heights,
                input.UnitScale,
                input.BlockScale,
                input.AltitudeScale);

            return terrain;
        }

        void ResolvePositions(CubeTerrain input, ContentProcessorContext context, MeshBuilder meshBuilder)
        {
            var altitudeScale = input.AltitudeScale;
            var scale = input.UnitScale * (float) input.BlockScale;
            var texCoordScale = input.TexCoordScale;

            var indexUV = new IndexInfo();

            #region Tops
            indexUV.Normal = Vector3.Up;
            for (int z = 0; z < heights.Height; z++)
            {
                for (int x = 0; x < heights.Width; x++)
                {
                    var y = (int) (heights.GetPixel(x, z) * (float) altitudeScale);

                    #region Resolve material layer

                    List<IndexInfo> indices = ResolveTopIndices(input, context, x, z, y);

                    #endregion

                    var x0 = x * scale;
                    var x1 = (x + 1) * scale;
                    var y0 = y * scale;
                    var z0 = z * scale;
                    var z1 = (z + 1) * scale;

                    // 0
                    indexUV.Index = meshBuilder.CreatePosition(x0, y0, z1);
                    indexUV.UV = new Vector2(x, z + 1) * texCoordScale;
                    indices.Add(indexUV);
                    // 1
                    indexUV.Index = meshBuilder.CreatePosition(x0, y0, z0);
                    indexUV.UV = new Vector2(x, z) * texCoordScale;
                    indices.Add(indexUV);
                    // 2
                    indexUV.Index = meshBuilder.CreatePosition(x1, y0, z0);
                    indexUV.UV = new Vector2(x + 1, z) * texCoordScale;
                    indices.Add(indexUV);
                    // 3
                    indexUV.Index = meshBuilder.CreatePosition(x1, y0, z1);
                    indexUV.UV = new Vector2(x + 1, z + 1) * texCoordScale;
                    indices.Add(indexUV);
                }
            }
            #endregion

            #region Sides
            for (int z = 0; z < heights.Height; z++)
            {
                for (int x = 0; x < heights.Width; x++)
                {
                    var y = (int) (heights.GetPixel(x, z) * (float) altitudeScale);

                    var x0 = x * scale;
                    var x1 = (x + 1) * scale;
                    var z0 = z * scale;
                    var z1 = (z + 1) * scale;

                    #region West sides
                    if (x != 0)
                    {
                        var westY = (int) (heights.GetPixel(x - 1, z) * (float) altitudeScale);
                        if (y < westY)
                        {
                            #region Resolve material layer

                            List<IndexInfo> indices = ResolveSideIndices(input, context, westY);

                            #endregion

                            indexUV.Normal = Vector3.Right;
                            for (int dy = westY; y < dy; dy--)
                            {
                                var y0 = (dy - 1) * scale;
                                var y1 = dy * scale;

                                // 0
                                indexUV.Index = meshBuilder.CreatePosition(x0, y0, z1);
                                indexUV.UV = new Vector2(z, y + 1) * texCoordScale;
                                indices.Add(indexUV);
                                // 1
                                indexUV.Index = meshBuilder.CreatePosition(x0, y1, z1);
                                indexUV.UV = new Vector2(z, y) * texCoordScale;
                                indices.Add(indexUV);
                                // 2
                                indexUV.Index = meshBuilder.CreatePosition(x0, y1, z0);
                                indexUV.UV = new Vector2(z + 1, y) * texCoordScale;
                                indices.Add(indexUV);
                                // 3
                                indexUV.Index = meshBuilder.CreatePosition(x0, y0, z0);
                                indexUV.UV = new Vector2(z + 1, y + 1) * texCoordScale;
                                indices.Add(indexUV);
                            }
                        }
                        else if (westY < y)
                        {
                            #region Resolve material layer

                            List<IndexInfo> indices = ResolveSideIndices(input, context, y);

                            #endregion

                            indexUV.Normal = Vector3.Left;
                            for (int dy = y; westY < dy; dy--)
                            {
                                var y0 = (dy - 1) * scale;
                                var y1 = dy * scale;

                                // 0
                                indexUV.Index = meshBuilder.CreatePosition(x0, y0, z0);
                                indexUV.UV = new Vector2(z, y + 1) * texCoordScale;
                                indices.Add(indexUV);
                                // 1
                                indexUV.Index = meshBuilder.CreatePosition(x0, y1, z0);
                                indexUV.UV = new Vector2(z, y) * texCoordScale;
                                indices.Add(indexUV);
                                // 2
                                indexUV.Index = meshBuilder.CreatePosition(x0, y1, z1);
                                indexUV.UV = new Vector2(z + 1, y) * texCoordScale;
                                indices.Add(indexUV);
                                // 3
                                indexUV.Index = meshBuilder.CreatePosition(x0, y0, z1);
                                indexUV.UV = new Vector2(z + 1, y + 1) * texCoordScale;
                                indices.Add(indexUV);
                            }
                        }
                    }
                    #endregion

                    #region North sides
                    if (z != 0)
                    {
                        var northY = (int) (heights.GetPixel(x, z - 1) * (float) altitudeScale);
                        if (y < northY)
                        {
                            #region Resolve material layer

                            List<IndexInfo> indices = ResolveSideIndices(input, context, northY);

                            #endregion

                            indexUV.Normal = Vector3.Backward;
                            for (int dy = northY; y < dy; dy--)
                            {
                                var y0 = (dy - 1) * scale;
                                var y1 = dy * scale;

                                // 0
                                indexUV.Index = meshBuilder.CreatePosition(x0, y0, z0);
                                indexUV.UV = new Vector2(x, y + 1) * texCoordScale;
                                indices.Add(indexUV);
                                // 1
                                indexUV.Index = meshBuilder.CreatePosition(x0, y1, z0);
                                indexUV.UV = new Vector2(x, y) * texCoordScale;
                                indices.Add(indexUV);
                                // 2
                                indexUV.Index = meshBuilder.CreatePosition(x1, y1, z0);
                                indexUV.UV = new Vector2(x + 1, y) * texCoordScale;
                                indices.Add(indexUV);
                                // 3
                                indexUV.Index = meshBuilder.CreatePosition(x1, y0, z0);
                                indexUV.UV = new Vector2(x + 1, y + 1) * texCoordScale;
                                indices.Add(indexUV);
                            }
                        }
                        else if (northY < y)
                        {
                            #region Resolve material layer

                            List<IndexInfo> indices = ResolveSideIndices(input, context, y);

                            #endregion

                            indexUV.Normal = Vector3.Forward;
                            for (int dy = y; northY < dy; dy--)
                            {
                                var y0 = (dy - 1) * scale;
                                var y1 = dy * scale;

                                // 0
                                indexUV.Index = meshBuilder.CreatePosition(x1, y0, z0);
                                indexUV.UV = new Vector2(x, y + 1) * texCoordScale;
                                indices.Add(indexUV);
                                // 1
                                indexUV.Index = meshBuilder.CreatePosition(x1, y1, z0);
                                indexUV.UV = new Vector2(x, y) * texCoordScale;
                                indices.Add(indexUV);
                                // 2
                                indexUV.Index = meshBuilder.CreatePosition(x0, y1, z0);
                                indexUV.UV = new Vector2(x + 1, y) * texCoordScale;
                                indices.Add(indexUV);
                                // 3
                                indexUV.Index = meshBuilder.CreatePosition(x0, y0, z0);
                                indexUV.UV = new Vector2(x + 1, y + 1) * texCoordScale;
                                indices.Add(indexUV);
                            }
                        }
                    }
                    #endregion
                }
            }
            #endregion
        }

        List<IndexInfo> ResolveTopIndices(CubeTerrain input, ContentProcessorContext context, int x, int z, int altitude)
        {

            if (alphamap0 != null &&
                0 < alphamap0.GetPixel(x, z).R)
            {
                return layerIndices0;
            }
            else if (alphamap1 != null &&
                0 < alphamap1.GetPixel(x, z).R)
            {
                return layerIndices1;
            }
            else if (alphamap2 != null &&
                0 < alphamap2.GetPixel(x, z).R)
            {
                return layerIndices2;
            }
            else if (alphamap3 != null &&
                0 < alphamap3.GetPixel(x, z).R)
            {
                return layerIndices3;
            }
            else if (input.Altitude0 != null &&
                input.Altitude0.ContainsAltitude(altitude))
            {
                return topIndices0;
            }
            else if (input.Altitude1 != null &&
                input.Altitude1.ContainsAltitude(altitude))
            {
                return topIndices1;
            }
            else if (input.Altitude2 != null &&
                input.Altitude2.ContainsAltitude(altitude))
            {
                return topIndices2;
            }
            else if (input.Altitude3 != null &&
                input.Altitude3.ContainsAltitude(altitude))
            {
                return topIndices3;
            }
            else
            {
                return junkIndices;
            }
        }

        List<IndexInfo> ResolveSideIndices(CubeTerrain input, ContentProcessorContext context, int altitude)
        {
            if (input.Altitude0 != null &&
                input.Altitude0.ContainsAltitude(altitude))
            {
                return sideIndices0;
            }
            else if (input.Altitude1 != null &&
                input.Altitude1.ContainsAltitude(altitude))
            {
                return sideIndices1;
            }
            else if (input.Altitude2 != null &&
                input.Altitude2.ContainsAltitude(altitude))
            {
                return sideIndices2;
            }
            else if (input.Altitude3 != null &&
                input.Altitude3.ContainsAltitude(altitude))
            {
                return sideIndices3;
            }
            else
            {
                return junkIndices;
            }
        }

        void AddVertex(MeshBuilder meshBuilder, int texCoordId, int normalId, List<IndexInfo> indices)
        {
            for (int i = 0; i < indices.Count; i += 4)
            {
                AddVertex(meshBuilder, texCoordId, normalId, indices[i]);
                AddVertex(meshBuilder, texCoordId, normalId, indices[i + 1]);
                AddVertex(meshBuilder, texCoordId, normalId, indices[i + 2]);

                AddVertex(meshBuilder, texCoordId, normalId, indices[i]);
                AddVertex(meshBuilder, texCoordId, normalId, indices[i + 2]);
                AddVertex(meshBuilder, texCoordId, normalId, indices[i + 3]);
            }
        }

        void AddVertex(MeshBuilder meshBuilder, int texCoordId, int normalId, IndexInfo indexInfo)
        {
            meshBuilder.SetVertexChannelData(texCoordId, indexInfo.UV);
            meshBuilder.SetVertexChannelData(normalId, indexInfo.Normal);
            meshBuilder.AddTriangleVertex(indexInfo.Index);
        }

        void ResolveVertices(CubeTerrain input, ContentProcessorContext context, MeshBuilder meshBuilder)
        {
            var altitudeScale = input.AltitudeScale;
            var texCoordScale = input.TexCoordScale;

            var texCoordId = meshBuilder.CreateVertexChannel<Vector2>(
                VertexChannelNames.TextureCoordinate(0));
            var normalId = meshBuilder.CreateVertexChannel<Vector3>(
                VertexChannelNames.Normal());

            if (0 < layerIndices0.Count)
            {
                meshBuilder.SetMaterial(layerMaterial0);
                AddVertex(meshBuilder, texCoordId, normalId, layerIndices0);
            }
            if (0 < layerIndices1.Count)
            {
                meshBuilder.SetMaterial(layerMaterial1);
                AddVertex(meshBuilder, texCoordId, normalId, layerIndices1);
            }
            if (0 < layerIndices2.Count)
            {
                meshBuilder.SetMaterial(layerMaterial2);
                AddVertex(meshBuilder, texCoordId, normalId, layerIndices2);
            }
            if (0 < layerIndices3.Count)
            {
                meshBuilder.SetMaterial(layerMaterial3);
                AddVertex(meshBuilder, texCoordId, normalId, layerIndices3);
            }
            if (0 < topIndices0.Count)
            {
                meshBuilder.SetMaterial(topMaterial0);
                AddVertex(meshBuilder, texCoordId, normalId, topIndices0);
            }
            if (0 < topIndices1.Count)
            {
                meshBuilder.SetMaterial(topMaterial1);
                AddVertex(meshBuilder, texCoordId, normalId, topIndices1);
            }
            if (0 < topIndices2.Count)
            {
                meshBuilder.SetMaterial(topMaterial2);
                AddVertex(meshBuilder, texCoordId, normalId, topIndices2);
            }
            if (0 < topIndices3.Count)
            {
                meshBuilder.SetMaterial(topMaterial3);
                AddVertex(meshBuilder, texCoordId, normalId, topIndices3);
            }
            if (0 < sideIndices0.Count)
            {
                meshBuilder.SetMaterial(sideMaterial0);
                AddVertex(meshBuilder, texCoordId, normalId, sideIndices0);
            }
            if (0 < sideIndices1.Count)
            {
                meshBuilder.SetMaterial(sideMaterial1);
                AddVertex(meshBuilder, texCoordId, normalId, sideIndices1);
            }
            if (0 < sideIndices2.Count)
            {
                meshBuilder.SetMaterial(sideMaterial2);
                AddVertex(meshBuilder, texCoordId, normalId, sideIndices2);
            }
            if (0 < sideIndices3.Count)
            {
                meshBuilder.SetMaterial(sideMaterial3);
                AddVertex(meshBuilder, texCoordId, normalId, sideIndices3);
            }
        }
    }
}
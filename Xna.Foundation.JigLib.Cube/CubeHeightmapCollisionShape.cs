#region Using

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Willcraftia.Xna.Foundation.Cube.Physics;
using Willcraftia.Xna.Foundation.Cube.Scenes;

#region Alias
using JPlane = JigLibX.Geometry.Plane;
#endregion

#endregion

namespace Willcraftia.Xna.Foundation.JigLib.Cube
{
    public sealed class CubeHeightmapCollisionShape : Primitive, ICubeHeightmapCollisionShape
    {
        #region Inner classes

        public struct MapPosition : IEquatable<MapPosition>
        {
            public int X;
            public int Z;
            public MapPosition(int x, int y)
            {
                X = x;
                Z = y;
            }

            #region IEquatable<MapPosition>

            public bool Equals(MapPosition other)
            {
                return X == other.X && Z == other.Z;
            }

            #endregion
        }

        #endregion

        #region Fields and Properties

        const int MaxVirtualBoxCount = 8;

        const int DefaultMarchCount = 10;

        int marchCount = DefaultMarchCount;
        float marchFraction = 1.0f / (float) DefaultMarchCount;
        Vector3[] marchPoints = new Vector3[DefaultMarchCount];
        List<MapPosition> marchMapPositions = new List<MapPosition>();
        Box marchVirtualBox = new Box(Vector3.Zero, Matrix.Identity, Vector3.One);

        #endregion

        #region Constructors

        public CubeHeightmapCollisionShape()
            : base(PrimitiveTypeManager.GetPrimitiveType<CubeHeightmapCollisionShape>())
        {
        }

        #endregion

        public void GetMapPosition(ref Vector3 point, out MapPosition result)
        {
            var scale = heightmap.Scale;
            var scaleInv = 1.0f / scale;

            int mapX = (int) ((point.X - position.X) * scaleInv);
            int mapZ = (int) ((point.Z - position.Z) * scaleInv);

            result = new MapPosition()
            {
                X = MathExtension.Clamp(mapX, 0, heightmap.MapXLength - 1),
                Z = MathExtension.Clamp(mapZ, 0, heightmap.MapZLength - 1)
            };
        }

        public void GetMapPositions(Vector3[] points, List<MapPosition> results)
        {
            results.Clear();

            var scale = heightmap.Scale;
            var scaleInv = 1.0f / scale;

            for (int i = 0; i < points.Length; i++)
            {
                int mapX = (int) ((points[i].X - position.X) * scaleInv);
                int mapZ = (int) ((points[i].Z - position.Z) * scaleInv);

                var mapPosition = new MapPosition()
                {
                    X = MathExtension.Clamp(mapX, 0, heightmap.MapXLength - 1),
                    Z = MathExtension.Clamp(mapZ, 0, heightmap.MapZLength - 1)
                };

                if (!results.Contains(mapPosition))
                {
                    results.Add(mapPosition);
                }
            }
        }

        public float GetHeight(MapPosition mapPosition)
        {
            return GetHeight(ref mapPosition);
        }

        public float GetHeight(ref MapPosition mapPosition)
        {
            return heightmap.GetHeight(mapPosition.X, mapPosition.Z);
        }

        public void PrepareVirtualPlane(MapPosition mapPosition, JPlane plane)
        {
            PrepareVirtualPlane(ref mapPosition, plane);
        }

        public void PrepareVirtualPlane(ref MapPosition mapPosition, JPlane plane)
        {
            var height = heightmap.GetHeight(mapPosition.X, mapPosition.Z);
            plane.Normal = Vector3.Up;
            plane.D = -(position.Y + height);
        }

        public void PrepareVirtualBox(ref MapPosition mapPosition, Box box)
        {
            var scale = heightmap.Scale;
            var height = heightmap.GetHeight(mapPosition.X, mapPosition.Z);

            var x = ((float) mapPosition.X) * scale + position.X;
            var z = ((float) mapPosition.Z) * scale + position.Z;

            box.Position = new Vector3(x, position.Y, z);
            box.SideLengths = new Vector3(scale, height, scale);
        }

        public void PrepareVirtualBox(MapPosition mapPosition, Box box)
        {
            PrepareVirtualBox(ref mapPosition, box);
        }

        public override Primitive Clone()
        {
            var clone = new CubeHeightmapCollisionShape();
            clone.Heightmap = heightmap;
            clone.Position = position;
            return clone;
        }

        public override bool SegmentIntersect(out float frac, out Vector3 pos, out Vector3 normal, Segment seg)
        {
            for (int i = 0; i < marchCount; i++)
            {
                marchPoints[i] = seg.Origin + seg.Delta * (marchFraction * (i + 1));
            }

            GetMapPositions(marchPoints, marchMapPositions);

            foreach (var mapPosition in marchMapPositions)
            {
                PrepareVirtualBox(mapPosition, marchVirtualBox);
                if (marchVirtualBox.SegmentIntersect(out frac, out pos, out normal, seg))
                {
                    return true;
                }
            }

            frac = 0;
            pos = Vector3.Zero;
            normal = Vector3.Up;
            return false;
        }

        public override float GetVolume()
        {
            return 0.0f;
        }

        public override float GetSurfaceArea()
        {
            return 0.0f;
        }

        public override void GetMassProperties(
            PrimitiveProperties primitiveProperties,
            out float mass,
            out Vector3 centerOfMass,
            out Matrix inertiaTensor)
        {
            mass = 0.0f;
            centerOfMass = Vector3.Zero;
            inertiaTensor = Matrix.Identity;
        }

        #region ICollisionShape

        float staticFriction;
        public float StaticFriction
        {
            get { return staticFriction; }
            set { staticFriction = value; }
        }

        float dynamicFriction;
        public float DynamicFriction
        {
            get { return dynamicFriction; }
            set { dynamicFriction = value; }
        }

        float restitution;
        public float Restitution
        {
            get { return restitution; }
            set { restitution = value; }
        }

        public float Volume
        {
            get { return 0; }
        }

        public bool Intersects(
            ref Vector3 position,
            ref Vector3 vector,
            out Vector3 resultPosition,
            out Vector3 resultNormal,
            out float resultFraction)
        {
            var segment = new Segment(position, vector);
            return SegmentIntersect(out resultFraction, out resultPosition, out resultNormal, segment);
        }

        #endregion

        #region ICubeHeightmapCollisionShape

        CubeHeightmap heightmap;
        public CubeHeightmap Heightmap
        {
            get { return heightmap; }
            set { heightmap = value; }
        }

        Vector3 position = Vector3.Zero;
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        #endregion
    }
}

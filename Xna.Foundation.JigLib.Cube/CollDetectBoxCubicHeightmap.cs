#region Using

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Willcraftia.Xna.Foundation.JigLib;

#endregion

namespace Willcraftia.Xna.Foundation.JigLib.Cube
{
    using JPlane = JigLibX.Geometry.Plane;

    public sealed class CollDetectBoxCubicHeightmap : DetectFunctor
    {
        #region Inner classes

        sealed class VirtualCollisionFunctor : CollisionFunctor
        {
            public CollisionFunctor CollisionFunctorEntity;
            public CollDetectInfo CollDetectInfoEntity;

            public override unsafe void CollisionNotify(
                ref CollDetectInfo collDetectInfo,
                ref Vector3 dirToBody0,
                SmallCollPointInfo* pointInfos,
                int numCollPts)
            {
                CollisionFunctorEntity.CollisionNotify(ref CollDetectInfoEntity, ref dirToBody0, pointInfos, numCollPts);
            }
        }

        #endregion

        #region Fields and Properties

        CollisionSkin virtualBoxSkin = new CollisionSkin();
        DetectFunctor boxBoxDetectFunctor;
        List<CubeHeightmapCollisionShape.MapPosition> oldMapPositions = new List<CubeHeightmapCollisionShape.MapPosition>();
        List<CubeHeightmapCollisionShape.MapPosition> newMapPositions = new List<CubeHeightmapCollisionShape.MapPosition>();
        VirtualCollisionFunctor virtualCollisionFunctor = new VirtualCollisionFunctor();

        CollisionSkin virtualPlaneSkin = new CollisionSkin();
        DetectFunctor boxPlaneDetectFunctor;

        #endregion

        #region Constructors

        public CollDetectBoxCubicHeightmap()
            : base("BoxCubicHeightmap", (int) PrimitiveType.Box, PrimitiveTypeManager.GetPrimitiveType<CubeHeightmapCollisionShape>())
        {
            virtualBoxSkin.AddPrimitive(
                new Box(Vector3.Zero, Matrix.Identity, Vector3.One), (int) MaterialTable.MaterialID.Unset);

            virtualPlaneSkin.AddPrimitive(new JPlane(), (int) MaterialTable.MaterialID.Unset);
        }

        #endregion

        public override void CollDetect(CollDetectInfo info, float collTolerance, CollisionFunctor collisionFunctor)
        {
            if (info.Skin0.GetPrimitiveOldWorld(info.IndexPrim0).Type == Type1)
            {
                var skinSwap = info.Skin0;
                info.Skin0 = info.Skin1;
                info.Skin1 = skinSwap;
                int primSwap = info.IndexPrim0;
                info.IndexPrim0 = info.IndexPrim1;
                info.IndexPrim1 = primSwap;
            }

            var oldBox = info.Skin0.GetPrimitiveOldWorld(info.IndexPrim0) as Box;
            var newBox = info.Skin0.GetPrimitiveNewWorld(info.IndexPrim0) as Box;

            var oldHeightmap = info.Skin1.GetPrimitiveOldWorld(info.IndexPrim1) as CubeHeightmapCollisionShape;
            var newHeightmap = info.Skin1.GetPrimitiveNewWorld(info.IndexPrim1) as CubeHeightmapCollisionShape;

            Vector3[] oldPoints;
            Vector3[] newPoints;
            oldBox.GetCornerPoints(out oldPoints);
            newBox.GetCornerPoints(out newPoints);

            if (boxBoxDetectFunctor == null)
            {
                var collisionSystem = PhysicsSystem.CurrentPhysicsSystem.CollisionSystem;
                boxBoxDetectFunctor = collisionSystem.GetCollDetectFunctor((int) PrimitiveType.Box, (int) PrimitiveType.Box);
            }

            oldHeightmap.GetMapPositions(oldPoints, oldMapPositions);
            newHeightmap.GetMapPositions(newPoints, newMapPositions);
            // TODO: needed ?
            foreach (var oldMapPosition in oldMapPositions)
            {
                if (!newMapPositions.Contains(oldMapPosition))
                {
                    newMapPositions.Add(oldMapPosition);
                }
            }

            bool sameHeight = true;
            float prevHeight = newHeightmap.GetHeight(newMapPositions[0]);
            for (int i = 1; i < newMapPositions.Count; i++)
            {
                if (prevHeight != newHeightmap.GetHeight(newMapPositions[i]))
                {
                    sameHeight = false;
                    break;
                }
            }

            #region As plane

            if (sameHeight)
            {
                virtualPlaneSkin.SetMaterialProperties(0, info.Skin1.GetMaterialProperties(info.IndexPrim1));

                var vcdi = new CollDetectInfo()
                {
                    Skin0 = info.Skin0,
                    Skin1 = virtualPlaneSkin,
                    IndexPrim0 = info.IndexPrim0,
                    IndexPrim1 = 0
                };

                virtualCollisionFunctor.CollisionFunctorEntity = collisionFunctor;
                virtualCollisionFunctor.CollDetectInfoEntity = info;

                var oldPlane = virtualPlaneSkin.GetPrimitiveOldWorld(0) as JPlane;
                var newPlane = virtualPlaneSkin.GetPrimitiveNewWorld(0) as JPlane;
                oldHeightmap.PrepareVirtualPlane(newMapPositions[0], oldPlane);
                newHeightmap.PrepareVirtualPlane(newMapPositions[0], newPlane);

                if (boxPlaneDetectFunctor == null)
                {
                    var collisionSystem = PhysicsSystem.CurrentPhysicsSystem.CollisionSystem;
                    boxPlaneDetectFunctor = collisionSystem.GetCollDetectFunctor(
                        (int) PrimitiveType.Box, (int) PrimitiveType.Plane);
                }

                virtualCollisionFunctor.CollisionFunctorEntity = collisionFunctor;
                virtualCollisionFunctor.CollDetectInfoEntity = info;

                boxPlaneDetectFunctor.CollDetect(vcdi, collTolerance, virtualCollisionFunctor);

                return;
            }

            #endregion

            #region As boxes

            virtualBoxSkin.SetMaterialProperties(0, info.Skin1.GetMaterialProperties(info.IndexPrim1));

            var virtualCollDetectInfo = new CollDetectInfo()
            {
                Skin0 = info.Skin0,
                Skin1 = virtualBoxSkin,
                IndexPrim0 = info.IndexPrim0,
                IndexPrim1 = 0
            };

            virtualCollisionFunctor.CollisionFunctorEntity = collisionFunctor;
            virtualCollisionFunctor.CollDetectInfoEntity = info;

            foreach (var newMapPosition in newMapPositions)
            {
                var oldPrimitive = virtualBoxSkin.GetPrimitiveOldWorld(0) as Box;
                var newPrimitive = virtualBoxSkin.GetPrimitiveNewWorld(0) as Box;
                oldHeightmap.PrepareVirtualBox(newMapPosition, oldPrimitive);
                newHeightmap.PrepareVirtualBox(newMapPosition, newPrimitive);

                boxBoxDetectFunctor.CollDetect(virtualCollDetectInfo, collTolerance, virtualCollisionFunctor);
            }

            #endregion
        }
    }
}

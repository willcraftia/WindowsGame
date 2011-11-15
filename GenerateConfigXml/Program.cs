#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Willcraftia.Xna.Foundation.Content.Pipeline.FluidSurface;
using Willcraftia.Xna.Foundation.Content.Pipeline.Bridges;
using Willcraftia.Xna.Foundation.Content.Pipeline.SkyDome;
using Willcraftia.Xna.Foundation.Content.Pipeline.TerrainMap;
using Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Characters;
using Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Materials;
using Willcraftia.Xna.Foundation.Content.Pipeline.Cube.StaticMeshes;
using Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Terrains;
using Willcraftia.Xna.Foundation.Cube.Scenes;
using Willcraftia.Xna.Foundation.Cube.Scenes.Factories;
using Willcraftia.Xna.Foundation.Scenes;
using Willcraftia.Xna.Foundation.Scenes.Factories;
using Willcraftia.Xna.Framework;
using Willcraftia.Xna.Framework.Audio;
using Willcraftia.Xna.Framework.Content;
using Willcraftia.Xna.Framework.Content.Pipeline.Audio;
using Willcraftia.Xna.Framework.Content.Pipeline.Xml;

#endregion

namespace GenerateConfigXml
{
    class Program
    {
        static void Main(string[] args)
        {
            ProcessSceneConfig();
            ProcessActors();
            ProcessModels();
            ProcessMaterials();
            ProcessAudioConfig();
            ProcessTerrainMap();
        }

        #region SceneConfig XML

        static void ProcessSceneConfig()
        {
            var sceneConfig = new SceneConfig();
            sceneConfig.SceneSettings.Time = 0.25f;
            sceneConfig.SceneSettings.TimeScale = 0.1f;
            sceneConfig.SceneAudioAssetName = "Audio/Scenes/Test1Scene";
            sceneConfig.CharacterAudioAssetName = "Audio/Characters/Character";

            #region AmbientSound
            {
                var ambientSound = new AmbientSound()
                {
                    Name = "FieldBGM",
                    SoundName = "FieldBGM"
                };
                sceneConfig.AmbientSounds.Add(ambientSound);
            }
            {
                var ambientSound = new AmbientSound()
                {
                    Name = "River",
                    SoundName = "River",
                    PlayableRadius = 200
                };
                ambientSound.Emitter = new AudioEmitter();
                ambientSound.Emitter.Position = new Vector3(354, 29, 357);
                ambientSound.Emitter.Forward = Vector3.Up;
                ambientSound.Emitter.Up = Vector3.Left;
                ambientSound.Emitter.Velocity = Vector3.Zero;
                sceneConfig.AmbientSounds.Add(ambientSound);
            }
            #endregion

            #region SceneSettings
            {
                sceneConfig.SceneSettings.GlobalAmbientColor = Color.DimGray.ToVector3();
                var direction0 = -(new Vector3(0, 1, -1));
                direction0.Normalize();
                sceneConfig.SceneSettings.DirectionalLight0.Direction = direction0;
                sceneConfig.SceneSettings.DirectionalLight0.DiffuseColor = Vector3.One;
                sceneConfig.SceneSettings.DirectionalLight0.SpecularColor = Vector3.One;
                sceneConfig.SceneSettings.DirectionalLight0.ShadowColor = Color.DimGray.ToVector3();
                sceneConfig.SceneSettings.DirectionalLight0.ShadowEnabled = true;
                sceneConfig.SceneSettings.DirectionalLight0.Enabled = true;
                sceneConfig.SceneSettings.Fog.Color = Vector3.One;
                sceneConfig.SceneSettings.Fog.Enabled = true;
                sceneConfig.SceneSettings.ShadowSettings.Enabled = false;
                sceneConfig.SceneSettings.ShadowSettings.Shape = LightFrustumShape.Pssm;
                sceneConfig.SceneSettings.ShadowSettings.ScreenSpaceShadow.MapScale = 1.0f;
            }
            #endregion

            sceneConfig.PlayerCharacterActorName = "Player";

            var actorBaseY = 80;

            #region CameraSceneActorConfig

            #region CameraActor [free]
            {
                var config = new ActualActorConfig();
                config.Factory = typeof(CameraActorFactory).FullName;
                config.Name = "free";
                config.Position = new Vector3(50, actorBaseY, 50);
                config.Actor = new CameraActor();
                (config.Actor as CameraActor).LookAt(new Vector3(100, actorBaseY, 100));
                (config.Actor as CameraActor).Pov.FarPlaneDistance = 10000.0f;
                sceneConfig.Actors.Add(config);
            }
            #endregion

            #region CameraActor [satellite]
            {
                var config = new ActualActorConfig();
                config.Factory = typeof(CameraActorFactory).FullName;
                config.Name = "satellite";
                config.Position = new Vector3(256.0f * 0.5f * 32.0f * 0.1f, 1000, 256.0f * 1.0f * 32.0f * 0.1f);
                config.Actor = new CameraActor();
                (config.Actor as CameraActor).LookAt(new Vector3(256.0f * 0.5f * 32.0f * 0.1f, 0, 256.0f * 0.5f * 32.0f * 0.1f));
                (config.Actor as CameraActor).Pov.FarPlaneDistance = 30000.0f;
                sceneConfig.Actors.Add(config);
            }
            #endregion

            #region CameraActor [char]
            {
                var config = new ActualActorConfig();
                config.Factory = typeof(CameraActorFactory).FullName;
                config.Name = "char";
                config.Actor = new CameraActor();
                (config.Actor as CameraActor).Active = true;
                sceneConfig.Actors.Add(config);
            }
            #endregion

            #endregion

            #region SceneActorConfig - CharacterActor

            #region SceneActorConfig [Player]
            {
                var config = new AssetActorConfig();
                config.Factory = typeof(CharacterActorFactory).FullName;
                config.Name = "Player";
                config.Position = new Vector3(0, actorBaseY, 100);
                config.AssetName = "Actors/Characters/CubeAnimateCharacter";
                sceneConfig.Actors.Add(config);
            }
            #endregion

            #region SceneActorConfig [Cuboid_n]
            {
                for (int i = 0; i < 100; i++)
                {
                    var config = new AssetActorConfig();
                    config.Factory = typeof(CharacterActorFactory).FullName;
                    config.Name = "Cuboid_" + i;
                    config.Position = new Vector3(i * 2, actorBaseY, i * 2);
                    config.AssetName = "Actors/StaticMeshes/CubeBlock";
                    sceneConfig.Actors.Add(config);
                }
            }
            #endregion

            #region SceneActorConfig [Cuboid_Gravity_Zero_n]
            {
                for (int i = 0; i < 10; i++)
                {
                    var config = new AssetActorConfig();
                    config.Factory = typeof(CharacterActorFactory).FullName;
                    config.Name = "Cuboid_Gravity_Zero_" + i;
                    config.Position = new Vector3(
                        i * 2 + 20 + 1,
                        actorBaseY - 15,
                        i * 2 + 150 + 1);
                    config.AssetName = "Actors/StaticMeshes/CubeBlock";
                    sceneConfig.Actors.Add(config);
                }
            }
            #endregion

            #region SceneActorConfig [Cuboid_Gravity_Reverse_n]
            {
                for (int i = 0; i < 10; i++)
                {
                    var config = new AssetActorConfig();
                    config.Factory = typeof(CharacterActorFactory).FullName;
                    config.Name = "Cuboid_Gravity_Reverse_" + i;
                    config.Position = new Vector3(
                        i * 2 + 80 + 1,
                        actorBaseY - 10,
                        i * 2 + 150 + 1);
                    config.AssetName = "Actors/StaticMeshes/CubeBlock";
                    sceneConfig.Actors.Add(config);
                }
            }
            #endregion

            #region SceneActorConfig [Cuboid_Fluid_n]
            {
                for (int i = 0; i < 10; i++)
                {
                    var config = new AssetActorConfig();
                    config.Factory = typeof(CharacterActorFactory).FullName;
                    config.Name = "Cuboid_Fluid_" + i;
                    config.Position = new Vector3(
                        i * 2 + 110 + 1,
                        actorBaseY - 10,
                        i * 2 + 150 + 1);
                    config.AssetName = "Actors/StaticMeshes/CubeBlock";
                    sceneConfig.Actors.Add(config);
                }
            }
            #endregion

            #endregion

            #region SceneActorConfig - StaticMeshActor

            #region SceneActorConfig [CubeStairs]
            {
                var config = new AssetActorConfig();
                config.Factory = typeof(StaticMeshActorFactory).FullName;
                config.Name = "CubeStairs";
                config.Position = new Vector3(20, actorBaseY - 11, 90);
                config.AssetName = "Actors/StaticMeshes/CubeStairs";
                sceneConfig.Actors.Add(config);
            }
            #endregion

            #region SceneActorConfig [CubeTile]
            {
                var config = new AssetActorConfig();
                config.Factory = typeof(StaticMeshActorFactory).FullName;
                config.Name = "CubeTile";
                config.Position = new Vector3(3, actorBaseY - 11, 90);
                config.AssetName = "Actors/StaticMeshes/CubeTile";
                sceneConfig.Actors.Add(config);
            }
            #endregion

            #endregion

            var terrainPosition = new Vector3(-1, 0, -1);

            #region SceneActorConfig - Terrain

            #region SceneActorConfig [Terrain]
            {
                var config = new AssetActorConfig();
                config.Factory = typeof(TerrainActorFactory).FullName;
                config.Name = "Terrain";
                config.Position = terrainPosition;
                config.AssetName = "Actors/Terrains/CubeTerrain";
                sceneConfig.Actors.Add(config);
            }
            #endregion

            #endregion

            #region SceneActorConfig - SkyDome

            #region SceneActorConfig [SkyDome]
            {
                var config = new AssetActorConfig();
                config.Factory = typeof(SkyDomeActorFactory).FullName;
                config.Name = "SkyDome";
                config.Position = new Vector3(0, -5000, 0);
                config.AssetName = "Actors/SkyDomes/SkyDome";
                sceneConfig.Actors.Add(config);
            }
            #endregion

            #endregion

            Vector3 fluidSurfacePosition;
            Vector3 fluidSurfaceScale;

            #region SceneActorConfig - FluidSurface

            #region SceneActorConfig [FluidSurface]
            {
                var unitScale = 0.1f;
                var blockScale = 32.0f;
                var blockAltitude = 8.0f;
                var scale = unitScale * blockScale;

                fluidSurfaceScale = new Vector3(320, 1, 320);
                fluidSurfacePosition = new Vector3(352, scale * blockAltitude - (scale - 0.4f), 352);

                var config = new AssetActorConfig();
                config.Factory = typeof(FluidSurfaceActorFactory).FullName;
                config.Name = "FluidSurface";
                config.Position = fluidSurfacePosition;
                config.Scale = fluidSurfaceScale;
                config.AssetName = "Actors/FluidSurfaces/FluidSurface";
                sceneConfig.Actors.Add(config);
            }
            #endregion

            #endregion

            #region SceneActorConfig - VolumeFog

            #region SceneActorConfig [VolumeFog]
            {
                //var actorConfig = new SceneActorConfig();
                //actorConfig.Name = "FolumeFog";
                //actorConfig.Position = new Vector3(
                //    fluidSurfacePosition.X,
                //    fluidSurfacePosition.Y,
                //    fluidSurfacePosition.Z);
                //actorConfig.Scale = new Vector3(15.0f, 5.0f, 15.0f);
                //actorConfig.AssetName = "Actors/VolumeFog";
                //sceneConfig.VolumeFogs.Add(actorConfig);
            }
            #endregion

            #endregion

            #region VolumeSceneActorConfig - PhysicsVolumeActor

            #region GravityVolumeActor [Gravity_Zero]
            {
                var config = new ActualActorConfig();
                config.Factory = typeof(PhysicsVolumeActorFactory).FullName;
                config.Name = "PhysicsVolume_Gravity_Zero";
                config.Actor = new GravityVolumeActor();
                (config.Actor as GravityVolumeActor).Shape.BoundingBox = new BoundingBox(
                    new Vector3(20, 30, 150),
                    new Vector3(40, 80, 190));
                (config.Actor as GravityVolumeActor).Gravity = Vector3.Zero;
                sceneConfig.Actors.Add(config);
            }
            #endregion

            #region GravityVolumeActor [Gravity_Lunatic]
            {
                var config = new ActualActorConfig();
                config.Factory = typeof(PhysicsVolumeActorFactory).FullName;
                config.Name = "PhysicsVolume_Gravity_Lunatic";
                config.Actor = new GravityVolumeActor();
                (config.Actor as GravityVolumeActor).Shape.BoundingBox = new BoundingBox(
                    new Vector3(50, 30, 150),
                    new Vector3(70, 120, 190));
                (config.Actor as GravityVolumeActor).Gravity = Vector3.Down * 1;
                sceneConfig.Actors.Add(config);
            }
            #endregion

            #region GravityVolumeActor [Gravity_Reverse]
            {
                var config = new ActualActorConfig();
                config.Factory = typeof(PhysicsVolumeActorFactory).FullName;
                config.Name = "PhysicsVolume_Gravity_Reverse";
                config.Actor = new GravityVolumeActor();
                (config.Actor as GravityVolumeActor).Shape.BoundingBox = new BoundingBox(
                    new Vector3(80, 30, 150),
                    new Vector3(100, 80, 190));
                (config.Actor as GravityVolumeActor).Gravity = Vector3.Up * 10;
                sceneConfig.Actors.Add(config);
            }
            #endregion

            #region FluidVolumeActor [Fluid]
            {
                var config = new ActualActorConfig();
                config.Factory = typeof(PhysicsVolumeActorFactory).FullName;
                config.Name = "PhysicsVolume_Fluid";
                config.Actor = new FluidVolumeActor();
                (config.Actor as FluidVolumeActor).Shape.BoundingBox = new BoundingBox(
                    new Vector3(110, 30, 150),
                    new Vector3(130, 70, 190));
                (config.Actor as FluidVolumeActor).Density = 1.0f;
                sceneConfig.Actors.Add(config);
            }
            #endregion

            #endregion

            #region VolumeSceneActorConfig - PostProcessVolumeActor

            #region PostProcessVolumeActor [EdgeDetection]
            {
                var config = new ActualActorConfig();
                config.Factory = typeof(PostProcessVolumeActorFactory).FullName;
                config.Name = "PostProcessVolume_EdgeDetection";
                config.Actor = new PostProcessVolumeActor();
                (config.Actor as PostProcessVolumeActor).Shape.BoundingBox = new BoundingBox(
                    new Vector3(0, 0, 0) + terrainPosition,
                    new Vector3(256 * 32 * 0.1f, 100, 256 * 32 * 0.1f));
                (config.Actor as PostProcessVolumeActor).Settings.EdgeDetectionEnabled = true;
                (config.Actor as PostProcessVolumeActor).Settings.EdgeDetectionSettings.FarPlaneDistance = 500.0f;
                (config.Actor as PostProcessVolumeActor).Settings.EdgeDetectionSettings.EdgeIntensity = 1.0f;
                (config.Actor as PostProcessVolumeActor).Settings.EdgeDetectionSettings.NormalSensitivity = 0.0f;
                (config.Actor as PostProcessVolumeActor).Settings.EdgeDetectionSettings.DepthSensitivity = 200.0f;
                (config.Actor as PostProcessVolumeActor).Settings.EdgeDetectionSettings.EdgeColor = Vector3.Zero;
                sceneConfig.Actors.Add(config);
            }
            #endregion

            #region PostProcessVolumeActor [Ssao]
            {
                var config = new ActualActorConfig();
                config.Factory = typeof(PostProcessVolumeActorFactory).FullName;
                config.Name = "PostProcessVolume_Ssao";
                config.Actor = new PostProcessVolumeActor();
                (config.Actor as PostProcessVolumeActor).Shape.BoundingBox = new BoundingBox(
                    new Vector3(20, 30, 100),
                    new Vector3(40, 80, 140));
                (config.Actor as PostProcessVolumeActor).Settings.SsaoEnabled = true;
                sceneConfig.Actors.Add(config);
            }
            #endregion

            #region PostProcessVolumeActor [DoF]
            {
                var config = new ActualActorConfig();
                config.Factory = typeof(PostProcessVolumeActorFactory).FullName;
                config.Name = "PostProcessVolume_Dof";
                config.Actor = new PostProcessVolumeActor();
                (config.Actor as PostProcessVolumeActor).Shape.BoundingBox = new BoundingBox(
                    new Vector3(50, 30, 100),
                    new Vector3(70, 80, 140));
                (config.Actor as PostProcessVolumeActor).Settings.DofEnabled = true;
                sceneConfig.Actors.Add(config);
            }
            #endregion

            #region PostProcessVolumeActor [Fluid]
            {
                var config = new ActualActorConfig();
                config.Factory = typeof(PostProcessVolumeActorFactory).FullName;
                config.Name = "PostProcessVolume_Fluid";
                config.Actor = new PostProcessVolumeActor();
                (config.Actor as PostProcessVolumeActor).Shape.BoundingBox = new BoundingBox(
                    new Vector3(
                        fluidSurfacePosition.X - fluidSurfaceScale.X * 0.5f,
                        fluidSurfacePosition.Y - 32.0f * 5.0f,
                        fluidSurfacePosition.Z - fluidSurfaceScale.Z * 0.5f),
                    new Vector3(
                        fluidSurfacePosition.X + fluidSurfaceScale.X * 0.5f,
                        fluidSurfacePosition.Y,
                        fluidSurfacePosition.Z + fluidSurfaceScale.Z * 0.5f));
                (config.Actor as PostProcessVolumeActor).Settings.DofEnabled = true;
                (config.Actor as PostProcessVolumeActor).Settings.DofSettings.FocusOverrideEnabled = true;
                (config.Actor as PostProcessVolumeActor).Settings.DofSettings.FocusDistance = 1.0f;
                (config.Actor as PostProcessVolumeActor).Settings.DofSettings.FocusRange = 2.0f;
                (config.Actor as PostProcessVolumeActor).Settings.DofSettings.BlurAmount = 0.2f;
                (config.Actor as PostProcessVolumeActor).Settings.ColorOverlapEnabled = true;
                (config.Actor as PostProcessVolumeActor).Settings.ColorOverlapSettings.Color = Color.LightBlue.ToVector3();
                (config.Actor as PostProcessVolumeActor).Settings.ColorOverlapSettings.Alpha = 0.7f;

                sceneConfig.Actors.Add(config);
            }
            #endregion

            #region PostProcessVolumeActor [GodRay]
            {
                var config = new ActualActorConfig();
                config.Factory = typeof(PostProcessVolumeActorFactory).FullName;
                config.Name = "PostProcessVolume_GodRay";
                config.Actor = new PostProcessVolumeActor();
                (config.Actor as PostProcessVolumeActor).Shape.BoundingBox = new BoundingBox(
                    new Vector3(80, 30, 100),
                    new Vector3(100, 80, 140));
                (config.Actor as PostProcessVolumeActor).Settings.GodRayEnabled = true;
                (config.Actor as PostProcessVolumeActor).Settings.GodRaySettings.SampleCount = 2;
                sceneConfig.Actors.Add(config);
            }
            #endregion

            #region PostProcessVolumeActor [Bloom]
            {
                var config = new ActualActorConfig();
                config.Factory = typeof(PostProcessVolumeActorFactory).FullName;
                config.Name = "PostProcessVolume_Bloom";
                config.Actor = new PostProcessVolumeActor();
                (config.Actor as PostProcessVolumeActor).Shape.BoundingBox = new BoundingBox(
                    new Vector3(140, 30, 100),
                    new Vector3(160, 80, 140));
                (config.Actor as PostProcessVolumeActor).Settings.BloomEnabled = true;
                sceneConfig.Actors.Add(config);
            }
            #endregion

            #region PostProcessVolumeActor [Monochrome]
            {
                var config = new ActualActorConfig();
                config.Factory = typeof(PostProcessVolumeActorFactory).FullName;
                config.Name = "PostProcessVolume_Monochrome";
                config.Actor = new PostProcessVolumeActor();
                (config.Actor as PostProcessVolumeActor).Shape.BoundingBox = new BoundingBox(
                    new Vector3(110, 30, 100),
                    new Vector3(130, 80, 140));
                (config.Actor as PostProcessVolumeActor).Settings.MonochromeEnabled = true;
                sceneConfig.Actors.Add(config);
            }
            #endregion

            #endregion

            XmlContentHelper.Serialize<SceneConfig>(sceneConfig, "c:/Dev/Xna/Content/Scenes/Test1Scene.xml");
            sceneConfig = XmlContentHelper.Deserialize<SceneConfig>("c:/Dev/Xna/Content/Scenes/Test1Scene.xml");
        }
        #endregion

        #region Actor XML

        static void ProcessActors()
        {
            #region CharacterActor

            #region CubeAnimateCharacterActorModel [CubeAnimateCharacter]
            {
                var actor = new CharacterActor();
                actor.RigidBodyOrientationIgnored = true;
                var actorModel = new CubeAnimateCharacterActorModel();
                actorModel.ModelAssetName = "Models/Characters/CubeAnimateCharacter";
                actorModel.CastShadowEnabled = true;
                actorModel.NearTransparencyEnabled = true;
                actor.ActorModel = actorModel;
                actor.RigidBodyConfig.InertiaTensorEnabled = false;
                actor.ActionSounds[new ActionSoundKey("Footstep", null)] = "DefaultFootstep";
                actor.ActionSounds[new ActionSoundKey("Jump", null)] = "Jump";
                var shapeConfig = new CollisionShapeConfig();
                shapeConfig.Factory = typeof(BoxActorCollisionShapeFactory).FullName;
                actor.CollisionBoundsConfig = new CollisionBoundsConfig();
                actor.CollisionBoundsConfig.CollisionShapeConfigs.Add(shapeConfig);

                XmlContentHelper.Serialize<CharacterActor>(actor, "c:/Dev/Xna/Content/Actors/Characters/CubeAnimateCharacter.xml");
                actor = XmlContentHelper.Deserialize<CharacterActor>("c:/Dev/Xna/Content/Actors/Characters/CubeAnimateCharacter.xml");
            }
            #endregion

            #region CubeAssetModelActorModel [CubeBlock]
            {
                var actor = new CharacterActor();
                var actorModel = new CubeAssetModelActorModel();
                actorModel.ModelAssetName = "Tests/CubeBlock";
                actor.ActorModel = actorModel;
                var shapeConfig = new CollisionShapeConfig();
                shapeConfig.Factory = typeof(BoxActorCollisionShapeFactory).FullName;
                actor.CollisionBoundsConfig = new CollisionBoundsConfig();
                actor.CollisionBoundsConfig.CollisionShapeConfigs.Add(shapeConfig);

                XmlContentHelper.Serialize<CharacterActor>(actor, "c:/Dev/Xna/Content/Actors/StaticMeshes/CubeBlock.xml");
                actor = XmlContentHelper.Deserialize<CharacterActor>("c:/Dev/Xna/Content/Actors/StaticMeshes/CubeBlock.xml");
            }
            #endregion

            #endregion

            #region StaticMeshes

            #region CubeAssetModelActorModel [CubeStairs]
            {
                var actor = new StaticMeshActor();
                var actorModel = new CubeAssetModelActorModel();
                actorModel.ModelAssetName = "Tests/CubeStairs";
                actor.ActorModel = actorModel;
                var shapeConfig = new CollisionShapeConfig();
                shapeConfig.Factory = typeof(SlopeActorCollisionShapeFactory).FullName;
                actor.CollisionBoundsConfig = new CollisionBoundsConfig();
                actor.CollisionBoundsConfig.CollisionShapeConfigs.Add(shapeConfig);

                XmlContentHelper.Serialize<StaticMeshActor>(actor, "c:/Dev/Xna/Content/Actors/StaticMeshes/CubeStairs.xml");
                actor = XmlContentHelper.Deserialize<StaticMeshActor>("c:/Dev/Xna/Content/Actors/StaticMeshes/CubeStairs.xml");
            }
            #endregion

            #region CubeAssetModelActorModel [CubeTile]
            {
                var actor = new StaticMeshActor();
                var actorModel = new CubeAssetModelActorModel();
                actorModel.ModelAssetName = "Tests/CubeTile";
                actor.ActorModel = actorModel;
                var shapeConfig = new CollisionShapeConfig();
                shapeConfig.Factory = typeof(MeshActorCollisionShapeFactory).FullName;
                actor.CollisionBoundsConfig = new CollisionBoundsConfig();
                actor.CollisionBoundsConfig.CollisionShapeConfigs.Add(shapeConfig);

                XmlContentHelper.Serialize<StaticMeshActor>(actor, "c:/Dev/Xna/Content/Actors/StaticMeshes/CubeTile.xml");
                actor = XmlContentHelper.Deserialize<StaticMeshActor>("c:/Dev/Xna/Content/Actors/StaticMeshes/CubeTile.xml");
            }
            #endregion

            #endregion

            #region Terrain

            #region CubeTerrainActor
            {
                var actor = new TerrainActor();
                var actorModel = new CubeTerrainActorModel();
                actorModel.TerrainAssetName = "Tests/CubeTerrain";
                actor.ActorModel = actorModel;
                var shapeConfig = new CollisionShapeConfig();
                shapeConfig.Factory = typeof(CubeHeightmapActorCollisionShapeFactory).FullName;
                actor.CollisionBoundsConfig = new CollisionBoundsConfig();
                actor.CollisionBoundsConfig.CollisionShapeConfigs.Add(shapeConfig);

                XmlContentHelper.Serialize<TerrainActor>(actor, "c:/Dev/Xna/Content/Actors/Terrains/CubeTerrain.xml");
                actor = XmlContentHelper.Deserialize<TerrainActor>("c:/Dev/Xna/Content/Actors/Terrains/CubeTerrain.xml");
            }
            #endregion

            #endregion

            #region SkyDomeActor
            {
                var actor = new SkyDomeActor();
                var actorModel = new SkyDomeActorModel();
                actorModel.AdjustBlenderRadiusEnabled = true;
                actorModel.ModelAssetName = "Models/SkyDome";
                actorModel.SkyMapDefinitions["SkyDome"] = "Textures/SkyDome";
                actorModel.ActiveSkyMapName = "SkyDome";
                actor.ActorModel = actorModel;

                XmlContentHelper.Serialize<SkyDomeActor>(actor, "c:/Dev/Xna/Content/Actors/SkyDomes/SkyDome.xml");
                actor = XmlContentHelper.Deserialize<SkyDomeActor>("c:/Dev/Xna/Content/Actors/SkyDomes/SkyDome.xml");
            }
            #endregion

            #region FluidSurfaceActor
            {
                var actor = new FluidSurfaceActor();
                var actorModel = new FluidSurfaceActorModel();
                actorModel.ModelAssetName = "FluidSurfaces/FluidSurface";
                actor.ActorModel = actorModel;

                XmlContentHelper.Serialize<FluidSurfaceActor>(actor, "c:/Dev/Xna/Content/Actors/FluidSurfaces/FluidSurface.xml");
                actor = XmlContentHelper.Deserialize<FluidSurfaceActor>("c:/Dev/Xna/Content/Actors/FluidSurfaces/FluidSurface.xml");
            }
            #endregion

            #region VolumeFogActor
            //{
            //    var actor = new VolumeFogActor();
            //    actor.VolumeFogActorModel.ModelAssetName = "Models/SceneValley_Fog";
            //    actor.VolumeFogActorModel.FogColor = Color.LightBlue.ToVector3();
            //    actor.VolumeFogActorModel.FogScale = 0.05f;

            //    XmlContentLoader.Save<VolumeFogActor>(actor, "c:/Dev/Xna/Content/Actors/VolumeFog.xml");
            //    actor = XmlContentLoader.Load<VolumeFogActor>("c:/Dev/Xna/Content/Actors/VolumeFog.xml");
            //}
            #endregion
        }

        #endregion

        #region Model XML

        static void ProcessModels()
        {
            var block = new CubeBlock();
            block.BaseMaterial = new ExternalReference<CubeMaterial>(
                @"..\\Materials\\CubeMaterial.xml");
            XmlContentHelper.Serialize<CubeBlock>(block, "c:/Dev/Xna/Content/Models/StaticMeshes/CubeBlock.xml");

            var stairs = new CubeStairs();
            stairs.BaseMaterial = new ExternalReference<CubeMaterial>(
                @"..\\Materials\\CubeMaterial.xml");
            XmlContentHelper.Serialize<CubeStairs>(stairs, "c:/Dev/Xna/Content/Models/StaticMeshes/CubeStairs.xml");

            var tile = new CubeTile();
            tile.Material = new ExternalReference<CubeMaterial>(
                @"..\\Materials\\CubeMaterial.xml");
            XmlContentHelper.Serialize<CubeTile>(tile, "c:/Dev/Xna/Content/Models/StaticMeshes/CubeTile.xml");

            {
                var template = new Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Characters.CubeAnimateCharacter();

                template.Sprite = new ExternalReference<TextureContent>("Sprite.png");
                template.SpriteWidth = 16;
                template.SpriteHeight = 16;
                template.ModelScale = 1.0f;
                template.CubeScale = 0.1f;
                template.Material = new ExternalReference<CubeMaterial>("CubeCharacterMaterial.xml");

                template.FrontSprites = new List<Point>();
                template.FrontSprites.Add(new Point(0, 0));
                template.FrontSprites.Add(new Point(0, 16));
                template.LeftSprites = new List<Point>();
                template.LeftSprites.Add(new Point(16, 0));
                template.LeftSprites.Add(new Point(16, 16));
                template.BackSprites = new List<Point>();
                template.BackSprites.Add(new Point(32, 0));
                template.BackSprites.Add(new Point(32, 16));
                template.RightSprites = new List<Point>();
                template.RightSprites.Add(new Point(48, 0));
                template.RightSprites.Add(new Point(48, 16));

                template.Animations = new Dictionary<string, Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Characters.CubeAnimateCharacter.Animation>();
                template.Animations["Walk"] = new Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Characters.CubeAnimateCharacter.Animation(2, 0, 1);

                XmlContentHelper.Serialize<Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Characters.CubeAnimateCharacter>(
                    template, "c:/Dev/Xna/Content/Models/Characters/CubicCharacter.xml");
            }

            {
                var template = new Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Terrains.CubeTerrain();

                template.Heightmap = new ExternalReference<TextureContent>("Heightmap.png");
                template.UnitScale = 0.1f;
                template.AltitudeScale = 32;
                template.BlockScale = 32;
                template.TexCoordScale = 2;

                template.Layer0 = new CubeTerrainLayer()
                {
                    Alphamap = new ExternalReference<TextureContent>("Alphamap0.png"),
                    Material = new ExternalReference<CubeMaterial>("Alphamap0Material.xml")
                };
                template.Layer1 = null;
                template.Layer2 = null;
                template.Layer3 = null;

                template.Altitude0 = new CubeTerrainAltitude()
                {
                    MinAltitude = 0,
                    MaxAltitude = 8,
                    TopMaterial = new ExternalReference<CubeMaterial>("TopMaterial0.xml"),
                    SideMaterial = new ExternalReference<CubeMaterial>("SideMaterial0.xml"),
                };
                template.Altitude1 = new CubeTerrainAltitude()
                {
                    MinAltitude = 9,
                    MaxAltitude = 16,
                    TopMaterial = new ExternalReference<CubeMaterial>("TopMaterial1.xml"),
                    SideMaterial = new ExternalReference<CubeMaterial>("SideMaterial1.xml"),
                };
                template.Altitude2 = new CubeTerrainAltitude()
                {
                    MinAltitude = 17,
                    MaxAltitude = 24,
                    TopMaterial = new ExternalReference<CubeMaterial>("TopMaterial2.xml"),
                    SideMaterial = new ExternalReference<CubeMaterial>("SideMaterial2.xml"),
                };
                template.Altitude3 = new CubeTerrainAltitude()
                {
                    MinAltitude = 25,
                    MaxAltitude = 32,
                    TopMaterial = new ExternalReference<CubeMaterial>("TopMaterial3.xml"),
                    SideMaterial = new ExternalReference<CubeMaterial>("SideMaterial3.xml"),
                };

                XmlContentHelper.Serialize<Willcraftia.Xna.Foundation.Content.Pipeline.Cube.Terrains.CubeTerrain>(
                    template, "c:/Dev/Xna/Content/Models/Terrains/CubeTerrain.xml");
            }

            {
                var template = new FluidSurface();
                template.Effect = new ExternalReference<EffectContent>("Effects/FluidSurfaceModelEffect.fx");
                XmlContentHelper.Serialize<FluidSurface>(template, "c:/Dev/Xna/Content/Models/FluidSurfaces/FluidSurface.xml");
            }

            {
                var template = new ModelBridge();
                template.Model = new ExternalReference<NodeContent>("SkyDome.fbx");
                template.Processor = typeof(SkyDomeModelProcessor).FullName;
                template.Parameters.Add("Scale", 100.0f);
                XmlContentHelper.Serialize<ModelBridge>(template, "c:/Dev/Xna/Content/Models/Bridges/SampleModel.bridge.xml");
            }
        }

        #endregion

        #region Material XML

        static void ProcessMaterials()
        {
            var material = new CubeMaterial();
            material.Diffuse.Texture = new ExternalReference<TextureContent>(
                "Textures/CubeTexture.png");
            XmlContentHelper.Serialize<CubeMaterial>(material, "c:/Dev/Xna/Content/Materials/CubeMaterial.xml");
        }

        #endregion

        #region AudioConfig

        static void ProcessAudioConfig()
        {
            var template = new AudioConfigDescription();

            template.XactEnabled = true;
            template.XactSettingsFile = null;
            template.WaveBank = new WaveBankConfigDescription();
            template.WaveBank.Filename = null;
            template.WaveBank.IsStreaming = false;
            template.WaveBank.Offset = 0;
            template.WaveBank.Packetsize = 2;
            template.SoundBank = new SoundBankConfigDescription();
            template.SoundBank.Filename = null;
            //template.BasicSounds = new List<BasicSoundConfigDescription>();

            XmlContentHelper.Serialize<AudioConfigDescription>(template, "c:/Dev/Xna/Content/Audio/AudioConfig.xml");
        }

        #endregion

        #region TerrainMap

        static void ProcessTerrainMap()
        {
            var template = new TerrainMapDescription();
            template.Heightmap = new ExternalReference<TextureContent>("Heightmap.png");
            template.AltitudeScale = 32;
            //template.Scale = 2;
            template.Layer0 = new TerrainMapLayerDescription()
            {
                Alphamap = new ExternalReference<TextureContent>("Alphamap0.png"),
                Color = Color.Gray
            };
            template.Red = new TerrainMapAltitudeColorDescription()
            {
                MinAltitude = 0,
                MaxAltitude = 8,
                MinColor = Color.AntiqueWhite,
                MaxColor = Color.NavajoWhite,
                MinBorderColor = null,
                MaxBorderColor = null
            };
            template.Green = new TerrainMapAltitudeColorDescription()
            {
                MinAltitude = 9,
                MaxAltitude = 16,
                MinColor = Color.YellowGreen,
                MaxColor = Color.DarkGreen,
                MinBorderColor = null,
                MaxBorderColor = null
            };
            template.Blue = new TerrainMapAltitudeColorDescription()
            {
                MinAltitude = 17,
                MaxAltitude = 24,
                MinColor = Color.Chocolate,
                MaxColor = Color.SaddleBrown,
                MinBorderColor = null,
                MaxBorderColor = null
            };
            template.Alpha = new TerrainMapAltitudeColorDescription()
            {
                MinAltitude = 25,
                MaxAltitude = 32,
                MinColor = Color.White,
                MaxColor = Color.Lavender,
                MinBorderColor = null,
                MaxBorderColor = null
            };
            template.FluidSurfaces = new TerrainMapFluidSurfaceDescription[1];
            template.FluidSurfaces[0] = new TerrainMapFluidSurfaceDescription()
            {
                MinPoint = new Point(0, 0),
                MaxPoint = new Point(256, 256),
                Altitude = 0,
                Color = Color.LightSkyBlue
            };

            XmlContentHelper.Serialize<TerrainMapDescription>(template, "c:/Dev/Xna/Content/TerrainMap/TerrainMap.xml");
        }

        #endregion
    }
}

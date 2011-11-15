#region Using

#endregion

namespace Willcraftia.Xna.Foundation.Scenes
{
    public interface IRenderableScene
    {
        ISceneContext SceneContext { get; }
        SceneSettings SceneSettings { get; }
        CameraActor ActiveCamera { get; }
        ActorCollection<CharacterActor> Characters { get; }
        ActorCollection<StaticMeshActor> StaticMeshes { get; }
        ActorCollection<TerrainActor> Terrains { get; }
        ActorCollection<FluidSurfaceActor> FluidSurfaces { get; }
        ActorCollection<SkyDomeActor> SkyDomes { get; }
        ActorCollection<VolumeFogActor> VolumeFogs { get; }
        ActorCollection<PhysicsVolumeActor> PhysicsVolumes { get; }
        ActorCollection<PostProcessVolumeActor> PostProcessVolumes { get; }
    }
}

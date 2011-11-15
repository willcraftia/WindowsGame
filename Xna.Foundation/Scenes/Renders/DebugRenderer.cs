#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Willcraftia.Xna.Framework.Audio;
using Willcraftia.Xna.Foundation.Graphics;

#endregion

namespace Willcraftia.Xna.Foundation.Scenes.Renders
{
    public sealed class DebugRenderer : RenderComponent
    {
        #region Fields and Properties

        BasicEffect effect;
        List<VertexPositionColor> vertices = new List<VertexPositionColor>();
        BoundingBoxDrawer boundingBoxDrawer;
        BoundingSphereDrawer boundingSphereDrawer;

        public Color CollisionColor = Color.Yellow;

        List<VolumeActor> volumes = new List<VolumeActor>();
        public List<VolumeActor> Volumes
        {
            get { return volumes; }
        }

        public Color VolumeColor = Color.Pink;

        List<AmbientSound> ambientSounds = new List<AmbientSound>();
        public List<AmbientSound> AmbientSounds
        {
            get { return ambientSounds; }
        }

        public Color AmbientSoundColor = Color.Cyan;

        #endregion

        #region Constructors

        public DebugRenderer(IRenderContext renderContext)
            : base(renderContext)
        {
        }

        #endregion

        #region LoadContent

        protected override void LoadContent()
        {
            effect = new BasicEffect(GraphicsDevice);
            effect.AmbientLightColor = Vector3.One;
            effect.VertexColorEnabled = true;
            boundingBoxDrawer = new BoundingBoxDrawer(GraphicsDevice);
            boundingSphereDrawer = new BoundingSphereDrawer(GraphicsDevice);

            base.LoadContent();
        }

        #endregion

        #region UnloadContent

        protected override void UnloadContent()
        {
            vertices.Clear();
            Volumes.Clear();
            effect.Dispose();

            base.UnloadContent();
        }

        #endregion

        public void AddVertices(IList<VertexPositionColor> vertices)
        {
            if (0 < this.vertices.Count)
            {
                var lastVertex = this.vertices.Last();
                this.vertices.Add(new VertexPositionColor(lastVertex.Position, Color.Transparent));
                this.vertices.Add(new VertexPositionColor(vertices[0].Position, Color.Transparent));
            }

            this.vertices.AddRange(vertices);
        }

        public void Draw(GameTime gameTime)
        {
            var pov = Scene.ActiveCamera.Pov;
            effect.View = pov.View;
            effect.Projection = pov.Projection;

            DrawVertices(gameTime);
            DrawVolumes(gameTime);
            DrawAmbientSounds(gameTime);
        }

        void DrawVertices(GameTime gameTime)
        {
            if (vertices.Count == 0)
            {
                return;
            }

            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                    PrimitiveType.LineStrip,
                    vertices.ToArray(),
                    0,
                    vertices.Count - 1);
            }

            GraphicsDevice.BlendState = BlendState.Opaque;

            vertices.Clear();
        }

        void DrawVolumes(GameTime gameTime)
        {
            foreach (var volume in Volumes)
            {
                var shape = volume.Shape;
                var boundingBoxShape = shape as BoundingBoxVolumeShape;
                if (boundingBoxShape != null)
                {
                    boundingBoxDrawer.Draw(ref boundingBoxShape.BoundingBox, effect, ref VolumeColor);
                }
            }

            Volumes.Clear();
        }

        void DrawAmbientSounds(GameTime gameTime)
        {
            var sphere = new BoundingSphere();
            foreach (var sound in AmbientSounds)
            {
                if (sound.Emitter == null)
                {
                    continue;
                }

                sphere.Center = sound.Emitter.Position;
                sphere.Radius = sound.PlayableRadius;

                boundingSphereDrawer.Draw(ref sphere, effect, ref AmbientSoundColor);
            }

            effect.World = Matrix.Identity;
            effect.DiffuseColor = Vector3.One;

            AmbientSounds.Clear();
        }
    }
}

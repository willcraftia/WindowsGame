#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Willcraftia.Xna.Framework.Graphics
{
    public sealed class Grid
    {
        #region Fields and Properties

        GraphicsDevice graphicsDevice;
        BasicEffect basicEffect;
        VertexPositionColor[] vertices;
        VertexBuffer vertexBuffer;
        bool vertexBufferDirty = true;

        int lineCount;

        float cellSize;

        /// <summary>
        /// セルのサイズ。
        /// </summary>
        public float CellSize
        {
            get { return cellSize; }
            set
            {
                cellSize = value;
                vertexBufferDirty = true;
            }
        }

        int quadrantCellCount;

        /// <summary>
        /// 第 1 象限 X 方向のセル数。
        /// </summary>
        public int QuadrantCellCount
        {
            get { return QuadrantCellCount; }
            set
            {
                QuadrantCellCount = value;
                vertexBufferDirty = true;
            }
        }

        Color gridColor = Color.LightGray;

        /// <summary>
        /// グリッド色。
        /// </summary>
        public Color GridColor
        {
            get { return gridColor; }
            set
            {
                gridColor = value;
                vertexBufferDirty = true;
            }
        }

        /// <summary>
        /// View 行列。
        /// </summary>
        /// <remarks>
        /// Draw メソッドを呼び出す前に、描画するシーンで使用している View 行列を設定します。
        /// </remarks>
        public Matrix View = Matrix.Identity;

        /// <summary>
        /// Projection 行列。
        /// </summary>
        /// <remarks>
        /// Draw メソッドを呼び出す前に、描画するシーンで使用している Projection 行列を設定します。
        /// </remarks>
        public Matrix Projection = Matrix.Identity;

        #endregion

        #region Constructors

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="graphicsDevice">GraphicsDevice。</param>
        public Grid(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;

            basicEffect = new BasicEffect(graphicsDevice);
            basicEffect.VertexColorEnabled = true;

            quadrantCellCount = 400;
            cellSize = 16.0f * 0.1f;

            InitializeVertices();
        }

        /// <summary>
        /// 頂点を初期化します。
        /// </summary>
        void InitializeVertices()
        {
            // An axis and grid lines
            lineCount = 1 + (quadrantCellCount * 2 + 1) * 2;
            var vertexCount = lineCount * 2;
            vertices = new VertexPositionColor[vertexCount];

            float halfLength = (float) quadrantCellCount * cellSize;

            // create vertices for an axis.
            vertices[0] = new VertexPositionColor(new Vector3(0.0f, -halfLength, 0.0f), Color.Green);
            vertices[1] = new VertexPositionColor(new Vector3(0.0f, halfLength, 0.0f), Color.Green);

            // the cameraRotationMatrix matrix for grid
            var rotation = Matrix.Identity;

            float cursorX = -halfLength;
            float cursorZ = -halfLength;
            for (int i = 2; i < vertexCount; i += 4)
            {
                // create vertices of a line on the z-axis parallel
                var v0 = new Vector3(cursorX, 0.0f, -halfLength);
                v0 = Vector3.Transform(v0, rotation);

                var v1 = new Vector3(cursorX, 0.0f, halfLength);
                v1 = Vector3.Transform(v1, rotation);

                // Resolve axis color
                var color = gridColor;
                if (cursorX == 0.0f)
                {
                    color = Color.Blue;
                }

                vertices[i] = new VertexPositionColor(v0, color);
                vertices[i + 1] = new VertexPositionColor(v1, color);

                // create vertices of a line on the x-axis parallel
                var v2 = new Vector3(-halfLength, 0.0f, cursorZ);
                v2 = Vector3.Transform(v2, rotation);

                var v3 = new Vector3(halfLength, 0.0f, cursorZ);
                v3 = Vector3.Transform(v3, rotation);

                // Resolve axis color
                color = gridColor;
                if (cursorZ == 0.0f)
                {
                    color = Color.Red;
                }

                vertices[i + 2] = new VertexPositionColor(v2, color);
                vertices[i + 3] = new VertexPositionColor(v3, color);

                cursorX += cellSize;
                cursorZ += cellSize;
            }

            if (vertexBuffer != null)
            {
                vertexBuffer.Dispose();
            }

            vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), vertices.Length, BufferUsage.None);
            vertexBuffer.SetData<VertexPositionColor>(vertices);

            vertexBufferDirty = false;
        }

        #endregion

        public void Draw()
        {
            if (vertexBufferDirty)
            {
                InitializeVertices();
            }

            basicEffect.View = View;
            basicEffect.Projection = Projection;

            graphicsDevice.SetVertexBuffer(vertexBuffer);

            foreach (var pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawPrimitives(PrimitiveType.LineList, 0, lineCount);
            }
        }
    }
}

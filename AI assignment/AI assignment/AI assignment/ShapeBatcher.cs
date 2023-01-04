using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AI_assignment
{
    internal class ShapeBatcher
    {
        private Game1 m_Game;

        private bool m_Disposed;
        private BasicEffect m_Effect;

        private VertexPositionColor[] m_Vertices;
        private int[] m_Indices;

        private int m_ShapeCount = 0;
        private int m_VertexCount = 0;
        private int m_IndexCount = 0;

        private bool m_IsStarted = false;
        public static readonly float MIN_LINE_THICKNESS = 2f;
        public static readonly float MAX_LINE_THICKNESS = 10f;


        public ShapeBatcher(Game1 pGame)
        {
            m_Game = pGame ?? throw new ArgumentNullException(nameof(pGame));
            m_Disposed = false;
            m_Effect = new BasicEffect(m_Game.GraphicsDevice);
            m_Effect.TextureEnabled = false;
            m_Effect.FogEnabled = false;
            m_Effect.LightingEnabled = false;
            m_Effect.VertexColorEnabled = true;

            m_Effect.World = Matrix.Identity;
            m_Effect.View = Matrix.Identity;
            m_Effect.Projection = Matrix.Identity;

            const int MAX_VERTEX_COUNT = 1024;
            const int MAX_INDEX_COUNT = MAX_VERTEX_COUNT * 3;

            m_Vertices = new VertexPositionColor[MAX_VERTEX_COUNT];
            m_Indices = new int[MAX_INDEX_COUNT];
        }

        public void Dispose()
        {
            if(m_Disposed) { return; }

            m_Effect?.Dispose();
            m_Disposed = true;
        }

        public void Begin()
        {
            if(m_IsStarted)
            {
                throw new System.Exception("Batch has already begun.");
            }

            Viewport viewport = m_Game.GraphicsDevice.Viewport;
            m_Effect.Projection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, 0, viewport.Height
                , 0f, 1f);

            m_IsStarted = true;
        }

        private void Ensurestarted()
        {
            if(!m_IsStarted)
            {
                throw new System.Exception("Batch has not started.");
            }
        }

        private void Flush()
        {
            if(m_ShapeCount == 0) { return; }

            Ensurestarted();

            foreach (EffectPass pass in m_Effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                m_Game.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList,
                    m_Vertices, 0, m_VertexCount, m_Indices, 0, m_IndexCount / 3);
            }
            
            m_ShapeCount = 0;
            m_IndexCount = 0;
            m_VertexCount = 0;
        }

        public void End()
        {
            Flush();
            m_IsStarted = false;
        }

        private void EnsureSpace(int pShapeVertexCount, int pShapeIndexCount)
        {
            if(pShapeVertexCount > m_Vertices.Length)
            {
                throw new System.Exception("Maximum shape vertex is " + m_Vertices.Length);
            }

            if (pShapeIndexCount > m_Indices.Length)
            {
                throw new System.Exception("Maximum shape index is " + m_Indices.Length);
            }

            if (m_VertexCount + pShapeVertexCount > m_Vertices.Length 
                || m_IndexCount + pShapeIndexCount > m_Indices.Length)
            {
                Flush();
            }
        }

        public void DrawLine(Vector2 pPointA, Vector2 pPointB, float pThickness, Color pColour)
        {
            Ensurestarted();

            const int shapeVertexCount = 4;
            const int shapeIndexCount = 6;

            EnsureSpace(shapeVertexCount, shapeIndexCount);
            pThickness = Math.Clamp(pThickness, MIN_LINE_THICKNESS, MAX_LINE_THICKNESS);

            float halfThickness = pThickness * 0.5f;

            float e1x = pPointB.X - pPointA.X;
            float e1y = pPointB.Y - pPointA.Y;
            float invLength = halfThickness / MathF.Sqrt(e1x * e1x + e1y * e1y);
            e1x *= invLength; 
            e1y *= invLength;

            float e2x = -e1x;
            float e2y = -e1y;

            float n1x = -e1y;
            float n1y = e1x;

            float n2x = -n1x;
            float n2y = -n1y;

            m_Indices[m_IndexCount++] = 0 + m_VertexCount;
            m_Indices[m_IndexCount++] = 1 + m_VertexCount;
            m_Indices[m_IndexCount++] = 2 + m_VertexCount;
            m_Indices[m_IndexCount++] = 0 + m_VertexCount;
            m_Indices[m_IndexCount++] = 2 + m_VertexCount;
            m_Indices[m_IndexCount++] = 3 + m_VertexCount;

            m_Vertices[m_VertexCount++] = new VertexPositionColor(new Vector3(pPointA.X + n1x + e2x,
                pPointA.Y + n1y + e2y, 0f), pColour);

            m_Vertices[m_VertexCount++] = new VertexPositionColor(new Vector3(pPointB.X + n1x + e1x,
                pPointB.Y + n1y + e1y, 0f), pColour);

            m_Vertices[m_VertexCount++] = new VertexPositionColor(new Vector3(pPointA.X + n2x + e1x,
                pPointA.Y + n2y + e1y, 0f), pColour);

            m_Vertices[m_VertexCount++] = new VertexPositionColor(new Vector3(pPointB.X + n2x + e2x,
                pPointB.Y + n2y + e2y, 0f), pColour);

            m_ShapeCount++;
        }

        public void DrawCircle(Vector2 pCentre, float pRadius, int pNumberOfVertices, float pThickness, Color pColour)
        {
            const int MIN_POINTS = 3;
            const int MAX_POINTS = 256;

            pNumberOfVertices = Math.Clamp(pNumberOfVertices, MIN_POINTS, MAX_POINTS);

            float deltaAngle = MathHelper.TwoPi / pNumberOfVertices;
            float angle = 0f;

            for (int i = 0; i < pNumberOfVertices; i++)
            {
                float aX = pCentre.X + pRadius * MathF.Sin(angle);
                float aY = pCentre.Y + pRadius * MathF.Cos(angle);
                angle += deltaAngle;

                float bX = pCentre.X + pRadius * MathF.Sin(angle);
                float bY = pCentre.Y + pRadius * MathF.Cos(angle);
                DrawLine(new Vector2(aX, aY), new Vector2(bX, bY), pThickness, pColour);
            }
        }

        public void DrawArrow(Vector2 pStart, Vector2 pVector, float pThickness, float pArrowSize, Color pColour)
        {
            Vector2 lineend = pStart + pVector;
            Vector2 u = pVector * (1f / pVector.Length());
            Vector2 v = new Vector2(-u.Y, u.X);

            Vector2 arrowHead1 = lineend - pArrowSize * u + pArrowSize * v;
            Vector2 arrowHead2 = lineend - pArrowSize * u - pArrowSize * v;

            DrawLine(pStart, lineend, pThickness, pColour);
            DrawLine(pStart, arrowHead1, pThickness, pColour);
            DrawLine(pStart, arrowHead2, pThickness, pColour);
        }
    }
}

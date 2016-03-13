using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelToy.Environment
{
    class Block
    {
        private Color color;
        private Vector3 position;
        private int size;
        private VertexPositionColor[] vertices;

        public Block(Vector3 position, Color color, int size = 1)
        {
            this.color = color;
            this.position = position;
            this.size = size;

            // Create block vertices.
            vertices = new VertexPositionColor[36];

            // Vertices, labelled looking along the positive Z axis.
            Vector3 ftl = new Vector3(position.X       , position.Y + size, position.Z       ); // Front, top-left
            Vector3 ftr = new Vector3(position.X + size, position.Y + size, position.Z       ); // Front, top-right
            Vector3 fbl = new Vector3(position.X       , position.Y       , position.Z       ); // Front, bottom-left
            Vector3 fbr = new Vector3(position.X + size, position.Y       , position.Z       ); // Front, bottom-right
            Vector3 btl = new Vector3(position.X       , position.Y + size, position.Z + size); // Back,  top-left
            Vector3 btr = new Vector3(position.X + size, position.Y + size, position.Z + size); // Back, top-right
            Vector3 bbl = new Vector3(position.X       , position.Y       , position.Z + size); // Back, bottom-left
            Vector3 bbr = new Vector3(position.X + size, position.Y       , position.Z + size); // Back, bottom-right


            // Front face
            vertices[ 0].Position = fbl; // Tri 1 - Bottom left
            vertices[ 1].Position = ftl; // Tri 1 - Top left
            vertices[ 2].Position = ftr; // Tri 1 - Top right
            vertices[ 3].Position = fbl; // Tri 2 - Bottom left
            vertices[ 4].Position = ftr; // Tri 2 - Top right
            vertices[ 5].Position = fbr; // Tri 2 - Bottom right

            // Left face
            vertices[ 6].Position = bbl; // Tri 1 - Bottom left
            vertices[ 7].Position = btl; // Tri 1 - Top left
            vertices[ 8].Position = ftl; // Tri 1 - Top right
            vertices[ 9].Position = bbl; // Tri 2 - Bottom left
            vertices[10].Position = ftl; // Tri 2 - Top right
            vertices[11].Position = fbl; // Tri 2 - Bottom right

            // Back face
            vertices[12].Position = bbr; // Tri 1 - Bottom left
            vertices[13].Position = btr; // Tri 1 - Top left
            vertices[14].Position = btl; // Tri 1 - Top right
            vertices[15].Position = bbr; // Tri 2 - Bottom left
            vertices[16].Position = btl; // Tri 2 - Top right
            vertices[17].Position = bbl; // Tri 2 - Bottom right

            // Right face
            vertices[18].Position = fbr; // Tri 1 - Bottom left
            vertices[19].Position = ftr; // Tri 1 - Top left
            vertices[20].Position = btr; // Tri 1 - Top right
            vertices[21].Position = fbr; // Tri 2 - Bottom left
            vertices[22].Position = btr; // Tri 2 - Top right
            vertices[23].Position = bbr; // Tri 2 - Bottom right

            // Top face
            vertices[24].Position = ftl; // Tri 1 - Bottom left
            vertices[25].Position = btl; // Tri 1 - Top left
            vertices[26].Position = btr; // Tri 1 - Top right
            vertices[27].Position = ftl; // Tri 2 - Bottom left
            vertices[28].Position = btr; // Tri 2 - Top right
            vertices[29].Position = ftr; // Tri 2 - Bottom right

            // Bottom face
            vertices[30].Position = bbl; // Tri 1 - Bottom left
            vertices[31].Position = fbl; // Tri 1 - Top left
            vertices[32].Position = fbr; // Tri 1 - Top right
            vertices[33].Position = bbl; // Tri 2 - Bottom left
            vertices[34].Position = fbr; // Tri 2 - Top right
            vertices[35].Position = bbr; // Tri 2 - Bottom right


            // Set vertex colours
            for (int i = 0; i < 36; ++i)
            {
                vertices[i].Color = color;
            }
        }

        public void Draw(Camera camera, BasicEffect effect)
        {
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                GameServices.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, vertices, 0, 12);
            }
        }
    }
}

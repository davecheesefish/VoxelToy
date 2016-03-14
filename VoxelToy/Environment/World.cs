using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelToy.Environment
{
    class World
    {
        private int triCount = 0;
        private int primitiveCount = 0;

        private int width;
        private int height;
        private int length;
        private Block[,,] blocks;
        private BasicEffect effect;
        private VertexBuffer vertexBuffer;

        public World(int width, int height, int length)
        {
            this.width = width;
            this.height = height;
            this.length = length;

            blocks = new Block[width, height, length];
            // Initialise blocks array
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    for (int z = 0; z < length; ++z)
                    {
                        if (y <= height * (Math.Cos(0.1 * (x + z)) + 1.0) / 2.0)
                        {
                            int rand = 100 + GameServices.Random.Next(156);
                            blocks[x, y, z] = new Block(new Color(10, rand, 10));
                        }
                        else
                        {
                            blocks[x, y, z] = null;
                        }
                    }
                }
            }

            // Construct vertices
            ReconstructBlockFaceVisibility();
            ReconstructVertices();

            effect = new BasicEffect(GameServices.GraphicsDevice);
            effect.VertexColorEnabled = true;
        }

        public void Draw(Camera camera)
        {
            effect.View = camera.LookAtMatrix;
            effect.Projection = GameSettings.PfovMatrix;
            GameServices.GraphicsDevice.SetVertexBuffer(vertexBuffer);

            // Draw blocks
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GameServices.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, primitiveCount);
            }
        }

        /// <summary>
        /// Reconstructs the flags on all blocks that signify which faces are visible.
        /// </summary>
        public void ReconstructBlockFaceVisibility()
        {
            Block block;
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    for (int z = 0; z < length; ++z)
                    {
                        block = BlockAt(x, y, z);

                        if (block == null)
                        {
                            continue;
                        }

                        // First, hide all faces
                        block.SetFaceHidden(AxisDirections.All);

                        // Now show faces that are open to the air.
                        // +X face
                        if (BlockAt(x + 1, y, z) == null)
                        {
                            block.SetFaceVisible(AxisDirections.XPositive);
                        }

                        // -X face
                        if (BlockAt(x - 1, y, z) == null)
                        {
                            block.SetFaceVisible(AxisDirections.XNegative);
                        }

                        // +Y face
                        if (BlockAt(x, y + 1, z) == null)
                        {
                            block.SetFaceVisible(AxisDirections.YPositive);
                        }

                        // -Y face
                        if (BlockAt(x, y - 1, z) == null)
                        {
                            block.SetFaceVisible(AxisDirections.YNegative);
                        }

                        // +Z face
                        if (BlockAt(x, y, z + 1) == null)
                        {
                            block.SetFaceVisible(AxisDirections.ZPositive);
                        }

                        // -Z face
                        if (BlockAt(x, y, z - 1) == null)
                        {
                            block.SetFaceVisible(AxisDirections.ZNegative);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Reconstructs voxel vertices for the world, based on visible block face data.
        /// </summary>
        public void ReconstructVertices()
        {
            Block block;
            List<VertexPositionColor> vertices = new List<VertexPositionColor>();

            primitiveCount = 0;

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    for (int z = 0; z < length; ++z)
                    {
                        block = BlockAt(x, y, z);

                        if (block == null)
                        {
                            continue;
                        }

                        // Add vertices for each face to world vertices

                        // Construct vectors for all corners
                        Vector3 xyz = new Vector3(x, y, z);
                        Vector3 pxyz = new Vector3(x + 1, y, z);
                        Vector3 xpyz = new Vector3(x, y + 1, z);
                        Vector3 pxpyz = new Vector3(x + 1, y + 1, z);
                        Vector3 xypz = new Vector3(x, y, z + 1);
                        Vector3 pxypz = new Vector3(x + 1, y, z + 1);
                        Vector3 xpypz = new Vector3(x, y + 1, z + 1);
                        Vector3 pxpypz = new Vector3(x + 1, y + 1, z + 1);

                        if ((block.VisibleFaces & AxisDirections.XNegative) > 0)
                        {
                            vertices.Add(new VertexPositionColor(xypz, block.Color));
                            vertices.Add(new VertexPositionColor(xpypz, block.Color));
                            vertices.Add(new VertexPositionColor(xpyz, block.Color));

                            vertices.Add(new VertexPositionColor(xypz, block.Color));
                            vertices.Add(new VertexPositionColor(xpyz, block.Color));
                            vertices.Add(new VertexPositionColor(xyz, block.Color));

                            primitiveCount += 2;
                        }

                        if ((block.VisibleFaces & AxisDirections.XPositive) > 0)
                        {
                            vertices.Add(new VertexPositionColor(pxyz, block.Color));
                            vertices.Add(new VertexPositionColor(pxpyz, block.Color));
                            vertices.Add(new VertexPositionColor(pxpypz, block.Color));

                            vertices.Add(new VertexPositionColor(pxyz, block.Color));
                            vertices.Add(new VertexPositionColor(pxpypz, block.Color));
                            vertices.Add(new VertexPositionColor(pxypz, block.Color));

                            primitiveCount += 2;
                        }

                        if ((block.VisibleFaces & AxisDirections.YNegative) > 0)
                        {
                            vertices.Add(new VertexPositionColor(xypz, block.Color));
                            vertices.Add(new VertexPositionColor(xyz, block.Color));
                            vertices.Add(new VertexPositionColor(pxyz, block.Color));

                            vertices.Add(new VertexPositionColor(xypz, block.Color));
                            vertices.Add(new VertexPositionColor(pxyz, block.Color));
                            vertices.Add(new VertexPositionColor(pxypz, block.Color));

                            primitiveCount += 2;
                        }

                        if ((block.VisibleFaces & AxisDirections.YPositive) > 0)
                        {
                            vertices.Add(new VertexPositionColor(xpyz, block.Color));
                            vertices.Add(new VertexPositionColor(xpypz, block.Color));
                            vertices.Add(new VertexPositionColor(pxpypz, block.Color));

                            vertices.Add(new VertexPositionColor(xpyz, block.Color));
                            vertices.Add(new VertexPositionColor(pxpypz, block.Color));
                            vertices.Add(new VertexPositionColor(pxpyz, block.Color));

                            primitiveCount += 2;
                        }

                        if ((block.VisibleFaces & AxisDirections.ZNegative) > 0)
                        {
                            vertices.Add(new VertexPositionColor(xyz, block.Color));
                            vertices.Add(new VertexPositionColor(xpyz, block.Color));
                            vertices.Add(new VertexPositionColor(pxpyz, block.Color));

                            vertices.Add(new VertexPositionColor(xyz, block.Color));
                            vertices.Add(new VertexPositionColor(pxpyz, block.Color));
                            vertices.Add(new VertexPositionColor(pxyz, block.Color));

                            primitiveCount += 2;
                        }

                        if ((block.VisibleFaces & AxisDirections.ZPositive) > 0)
                        {
                            vertices.Add(new VertexPositionColor(pxypz, block.Color));
                            vertices.Add(new VertexPositionColor(pxpypz, block.Color));
                            vertices.Add(new VertexPositionColor(xpypz, block.Color));

                            vertices.Add(new VertexPositionColor(pxypz, block.Color));
                            vertices.Add(new VertexPositionColor(xpypz, block.Color));
                            vertices.Add(new VertexPositionColor(xypz, block.Color));

                            primitiveCount += 2;
                        }
                    }
                }
            }
            
            vertices.Reverse();

            vertexBuffer = new VertexBuffer(GameServices.GraphicsDevice, typeof(VertexPositionColor), vertices.Count, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColor>(vertices.ToArray());
        }

        /// <summary>
        /// Returns the block at a specified position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public Block BlockAt(int x, int y, int z)
        {
            // If the given co-ordinate is outside the world, return no block.
            if (
                x < 0 || x >= width  ||
                y < 0 || y >= height ||
                z < 0 || z >= length
            )
            {
                return null;
            }

            // Co-ordinate is within the world, return the block.
            return blocks[x, y, z];
        }
    }
}

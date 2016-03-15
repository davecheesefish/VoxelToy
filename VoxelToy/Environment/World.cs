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
        private bool isDirty = true;

        private int width;
        private int height;
        private int length;
        private Block[,,] blocks;
        private BasicEffect effect;
        private VertexBuffer vertexBuffer;
        private Texture2D blockTexture;

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
                        if (y <= height * (((Math.Cos(0.1 * (x + z)) + 1.0) / 4.0) + ((Math.Cos(0.1 * z) + 1.0) / 4.0)))
                        {
                            int rand = 80 + GameServices.Random.Next(126);
                            blocks[x, y, z] = new Block(BlockType.Get("grass"));
                        }
                        else
                        {
                            blocks[x, y, z] = null;
                        }
                    }
                }
            }

            effect = new BasicEffect(GameServices.GraphicsDevice);
            effect.VertexColorEnabled = true;
        }

        public void LoadContent()
        {
            blockTexture = GameServices.ContentManager.Load<Texture2D>(@"textures/blocks");
            effect.TextureEnabled = true;
            effect.Texture = blockTexture;
            effect.AmbientLightColor = Color.White.ToVector3();
        }

        public void Update(GameTime gameTime)
        {
            if (isDirty)
            {
                ReconstructBlockFaceVisibility();
                ReconstructVertices();
                isDirty = false;
            }
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
            List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();

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

                        Vector2[] uvCoords; // UV co-ordinates for each vertex.

                        if ((block.VisibleFaces & AxisDirections.XNegative) > 0)
                        {
                            uvCoords = block.BlockType.GetUvCoordinates(AxisDirections.XNegative, blockTexture.Width);

                            vertices.Add(new VertexPositionColorTexture(xypz, Color.White, uvCoords[0]));
                            vertices.Add(new VertexPositionColorTexture(xpypz, Color.White, uvCoords[1]));
                            vertices.Add(new VertexPositionColorTexture(xpyz, Color.White, uvCoords[2]));

                            vertices.Add(new VertexPositionColorTexture(xypz, Color.White, uvCoords[0]));
                            vertices.Add(new VertexPositionColorTexture(xpyz, Color.White, uvCoords[2]));
                            vertices.Add(new VertexPositionColorTexture(xyz, Color.White, uvCoords[3]));

                            primitiveCount += 2;
                        }

                        if ((block.VisibleFaces & AxisDirections.XPositive) > 0)
                        {
                            uvCoords = block.BlockType.GetUvCoordinates(AxisDirections.XPositive, blockTexture.Width);

                            vertices.Add(new VertexPositionColorTexture(pxyz, Color.White, uvCoords[0]));
                            vertices.Add(new VertexPositionColorTexture(pxpyz, Color.White, uvCoords[1]));
                            vertices.Add(new VertexPositionColorTexture(pxpypz, Color.White, uvCoords[2]));

                            vertices.Add(new VertexPositionColorTexture(pxyz, Color.White, uvCoords[0]));
                            vertices.Add(new VertexPositionColorTexture(pxpypz, Color.White, uvCoords[2]));
                            vertices.Add(new VertexPositionColorTexture(pxypz, Color.White, uvCoords[3]));

                            primitiveCount += 2;
                        }

                        if ((block.VisibleFaces & AxisDirections.YNegative) > 0)
                        {
                            uvCoords = block.BlockType.GetUvCoordinates(AxisDirections.YNegative, blockTexture.Width);

                            vertices.Add(new VertexPositionColorTexture(xypz, Color.White, uvCoords[0]));
                            vertices.Add(new VertexPositionColorTexture(xyz, Color.White, uvCoords[1]));
                            vertices.Add(new VertexPositionColorTexture(pxyz, Color.White, uvCoords[2]));

                            vertices.Add(new VertexPositionColorTexture(xypz, Color.White, uvCoords[0]));
                            vertices.Add(new VertexPositionColorTexture(pxyz, Color.White, uvCoords[2]));
                            vertices.Add(new VertexPositionColorTexture(pxypz, Color.White, uvCoords[3]));

                            primitiveCount += 2;
                        }

                        if ((block.VisibleFaces & AxisDirections.YPositive) > 0)
                        {
                            uvCoords = block.BlockType.GetUvCoordinates(AxisDirections.YPositive, blockTexture.Width);

                            vertices.Add(new VertexPositionColorTexture(xpyz, Color.White, uvCoords[0]));
                            vertices.Add(new VertexPositionColorTexture(xpypz, Color.White, uvCoords[1]));
                            vertices.Add(new VertexPositionColorTexture(pxpypz, Color.White, uvCoords[2]));

                            vertices.Add(new VertexPositionColorTexture(xpyz, Color.White, uvCoords[0]));
                            vertices.Add(new VertexPositionColorTexture(pxpypz, Color.White, uvCoords[2]));
                            vertices.Add(new VertexPositionColorTexture(pxpyz, Color.White, uvCoords[3]));

                            primitiveCount += 2;
                        }

                        if ((block.VisibleFaces & AxisDirections.ZNegative) > 0)
                        {
                            uvCoords = block.BlockType.GetUvCoordinates(AxisDirections.ZNegative, blockTexture.Width);

                            vertices.Add(new VertexPositionColorTexture(xyz, Color.White, uvCoords[0]));
                            vertices.Add(new VertexPositionColorTexture(xpyz, Color.White, uvCoords[1]));
                            vertices.Add(new VertexPositionColorTexture(pxpyz, Color.White, uvCoords[2]));

                            vertices.Add(new VertexPositionColorTexture(xyz, Color.White, uvCoords[0]));
                            vertices.Add(new VertexPositionColorTexture(pxpyz, Color.White, uvCoords[2]));
                            vertices.Add(new VertexPositionColorTexture(pxyz, Color.White, uvCoords[3]));

                            primitiveCount += 2;
                        }

                        if ((block.VisibleFaces & AxisDirections.ZPositive) > 0)
                        {
                            uvCoords = block.BlockType.GetUvCoordinates(AxisDirections.ZPositive, blockTexture.Width);

                            vertices.Add(new VertexPositionColorTexture(pxypz, Color.White, uvCoords[0]));
                            vertices.Add(new VertexPositionColorTexture(pxpypz, Color.White, uvCoords[1]));
                            vertices.Add(new VertexPositionColorTexture(xpypz, Color.White, uvCoords[2]));

                            vertices.Add(new VertexPositionColorTexture(pxypz, Color.White, uvCoords[0]));
                            vertices.Add(new VertexPositionColorTexture(xpypz, Color.White, uvCoords[2]));
                            vertices.Add(new VertexPositionColorTexture(xypz, Color.White, uvCoords[3]));

                            primitiveCount += 2;
                        }
                    }
                }
            }
            
            vertices.Reverse();

            vertexBuffer = new VertexBuffer(GameServices.GraphicsDevice, typeof(VertexPositionColorTexture), vertices.Count, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColorTexture>(vertices.ToArray());
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxelToy.Environment.Generators;

namespace VoxelToy.Environment
{
    class World
    {
        /// <summary>
        /// Width of the map along the X axis, in chunks.
        /// </summary>
        private int width;

        /// <summary>
        /// Length of the map along the Z axis, in chunks.
        /// </summary>
        private int length;

        private TerrainGenerator generator;

        private BasicEffect effect;
        private Texture2D blockTexture;

        private Chunk[,] chunks;


        /// <summary>
        /// Creates a new World.
        /// </summary>
        /// <param name="width">Number of chunks along the X axis.</param>
        /// <param name="length">Number of chunks along the Z axis.</param>
        /// <param name="generator">Terrain generator to use for new chunks.</param>
        public World(int width, int length, TerrainGenerator generator)
        {
            this.width = width;
            this.length = length;

            this.generator = generator;

            // Initialise chunks array
            chunks = new Chunk[width, length];
            for (int x = 0; x < width; ++x)
            {
                for (int z = 0; z < length; ++z)
                {
                    chunks[x, z] = new Chunk(x, z);
                    generator.GenerateChunk(chunks[x, z]);
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
        }

        public void Draw(Camera camera)
        {
            effect.View = camera.LookAtMatrix;
            effect.Projection = GameSettings.PfovMatrix;

            Chunk chunk;
            for (int x = 0; x < width; ++x)
            {
                for (int z = 0; z < length; ++z)
                {
                    chunk = chunks[x, z];

                    // If necessary, rebuild the chunk's geometry.
                    if (chunk.IsDirty)
                    {
                        chunk.Rebuild();
                    }

                    // Get the vertices for the chunk
                    GameServices.GraphicsDevice.SetVertexBuffer(chunk.VertexBuffer);

                    // Draw blocks
                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        GameServices.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, chunk.PrimitiveCount);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the chunk at the given position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private Chunk ChunkAt(int x, int z)
        {
            // Calculate the chunk that the given co-ordinates are in.
            int chunkX = x / Chunk.WIDTH;
            int chunkZ = z / Chunk.LENGTH;

            // If the given co-ordinate is outside of the world, return null.
            if (
                chunkX < 0 || chunkX >= width ||
                chunkZ < 0 || chunkZ >= length
            )
            {
                return null;
            }

            return chunks[chunkX, chunkZ];
        }
    }
}

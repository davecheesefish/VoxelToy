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
        /// Width of the map along the X axis, in blocks.
        /// </summary>
        public int WidthInBlocks { get { return width * Chunk.WIDTH; } }
        
        /// <summary>
        /// Width of the map along the X axis, in chunks.
        /// </summary>
        public int WidthInChunks { get { return width; } }
        private int width;

        /// <summary>
        /// Length of the map along the Z axis, in blocks.
        /// </summary>
        public int LengthInBlocks { get { return length * Chunk.LENGTH; } }

        /// <summary>
        /// Length of the map along the Z axis, in chunks.
        /// </summary>
        public int LengthInChunks { get { return length; } }
        private int length;

        /// <summary>
        /// The height of the map along the Y axis, in blocks.
        /// </summary>
        public int HeightInBlocks { get { return Chunk.HEIGHT; } }

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
            Generate();

            effect = new BasicEffect(GameServices.GraphicsDevice);
            effect.VertexColorEnabled = true;
        }

        public void LoadContent()
        {
            blockTexture = GameServices.ContentManager.Load<Texture2D>(@"textures/blocks");
            effect.TextureEnabled = true;
            effect.Texture = blockTexture;
        }

        public void Draw(Graphics.SceneRenderer renderer)
        {
            effect.View = renderer.Camera.LookAtMatrix;
            effect.Projection = GameSettings.PfovMatrix;

            Chunk chunk;

            // Draw geometry
            for (int x = 0; x < width; ++x)
            {
                for (int z = 0; z < length; ++z)
                {
                    chunk = chunks[x, z];

                    // If necessary, rebuild the chunk's geometry.
                    if (chunk.IsDirty)
                    {
                        chunk.Rebuild(renderer);
                    }
                    else
                    {
                        // Else just rebuild transparent vertices to ensure correct draw order.
                        // TODO: Perform this only when the player moves, not every frame.
                        chunk.ReconstructVertices(renderer, true);
                    }

                    // If there are opaque vertices to draw, do so.
                    if (chunk.OpaqueVertexBuffer != null)
                    {
                        renderer.DrawVertices(chunk.OpaqueVertexBuffer, effect);
                    }

                    // Draw any alpha-blended vertices.
                    if (chunk.TransparentVertexBuffer != null)
                    {
                        renderer.DrawAlphaBlendedVertices(chunk.TransparentVertexBuffer, effect, new Vector3(x * Chunk.WIDTH, 0, z * Chunk.LENGTH));
                    }
                }
            }
        }

        /// <summary>
        /// Generates new terrain for this world.
        /// </summary>
        public void Generate()
        {
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

            generator.GenerateStructures(this);
        }

        /// <summary>
        /// Replaces the block at the position specified by the co-ordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="newBlock">The new block to put in this position.</param>
        public void ReplaceBlock(int x, int y, int z, Block newBlock)
        {
            Chunk chunk = ChunkAt(x, y, z);
            if (chunk == null)
            {
                // Invalid block location
                return;
            }

            // Get co-ordinates within the chunk
            x = x % Chunk.WIDTH;
            y = y % Chunk.HEIGHT;
            z = z % Chunk.LENGTH;

            chunk.ReplaceBlock(x, y, z, newBlock);
        }

        /// <summary>
        /// Builds a structure in the world with its origin at the given position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="structure">The structure to build.</param>
        public void BuildStructure(int x, int y, int z, Structures.Structure structure)
        {
            Block[, ,] structureBlocks = structure.GetBlocks();
            
            // Move start position to account for shifted origin point.
            x -= structure.Origin.X;
            y -= structure.Origin.Y;
            z -= structure.Origin.Z;

            // Build the structure by replacing blocks
            for (int structureX = 0; structureX < structure.Width; ++structureX)
            {
                for (int structureY = 0; structureY < structure.Height; ++structureY)
                {
                    for (int structureZ = 0; structureZ < structure.Length; ++structureZ)
                    {
                        if (structureBlocks[structureX, structureY, structureZ] != null)
                        {
                            ReplaceBlock(x + structureX, y + structureY, z + structureZ, structureBlocks[structureX, structureY, structureZ]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns the block at the specified co-ordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public Block BlockAt(int x, int y, int z)
        {
            Chunk chunk = ChunkAt(x, y, z);
            if (chunk == null)
            {
                // Invalid block location
                return null;
            }

            // Get co-ordinates within the chunk
            x = x % Chunk.WIDTH;
            y = y % Chunk.HEIGHT;
            z = z % Chunk.LENGTH;

            return chunk.BlockAt(x, y, z);
        }

        /// <summary>
        /// Returns the chunk at the given position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private Chunk ChunkAt(int x, int y, int z)
        {
            // Make sure co-ordinates are positive
            if (x < 0 || y < 0 || z < 0)
            {
                return null;
            }

            // Calculate the chunk that the given co-ordinates are in.
            int chunkX = x / Chunk.WIDTH;
            int chunkZ = z / Chunk.LENGTH;

            // If the given co-ordinate is outside of the world, return null.
            if (
                chunkX >= width ||
                y > Chunk.HEIGHT ||  // Map is only 1 chunk tall
                chunkZ >= length
            )
            {
                return null;
            }

            return chunks[chunkX, chunkZ];
        }
    }
}

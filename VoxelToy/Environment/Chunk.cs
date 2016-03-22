using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelToy.Environment
{
    /// <summary>
    /// Represents a small section of the world, used for localising geometry updates.
    /// </summary>
    class Chunk
    {
        /// <summary>
        /// Side length of an individual chunk along the X axis.
        /// </summary>
        public const byte WIDTH = 16;

        /// <summary>
        /// Side length of an individual chunk along the Y axis.
        /// </summary>
        public const byte HEIGHT = 64;

        /// <summary>
        /// Side length of an individual chunk along the Z axis.
        /// </summary>
        public const byte LENGTH = 16;

        /// <summary>
        /// X co-ordinate of this chunk, in chunks.
        /// </summary>
        public int ChunkX { get { return chunkX; } }
        private int chunkX;

        /// <summary>
        /// Z co-ordinate of this chunk, in chunks.
        /// </summary>
        public int ChunkZ { get { return chunkZ; } }
        private int chunkZ;

        /// <summary>
        /// The blocks contained within this chunk.
        /// </summary>
        private Block[,,] blocks;

        /// <summary>
        /// Flag for when the geometry for this chunk needs rebuilding.
        /// </summary>
        public bool IsDirty { get { return isDirty; } }
        private bool isDirty = true;

        public VertexBuffer VertexBuffer { get { return vertexBuffer; } }
        private VertexBuffer vertexBuffer;

        public int PrimitiveCount { get { return primitiveCount; } }
        private int primitiveCount = 0;
 

        public Chunk(int x, int z)
        {
            chunkX = x;
            chunkZ = z;

            blocks = new Block[WIDTH, HEIGHT, LENGTH];
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
            // If the given co-ordinate is outside the chunk, return null.
            if (
                x < 0 || x >= WIDTH ||
                y < 0 || y >= HEIGHT ||
                z < 0 || z >= LENGTH
            )
            {
                return null;
            }

            // Co-ordinate is within the world, return the block.
            return blocks[x, y, z];
        }

        /// <summary>
        /// Replaces the block at position (x, y, z) with the new block supplied.
        /// </summary>
        /// <param name="x">X position of the block to replace.</param>
        /// <param name="y">Y position of the block to replace.</param>
        /// <param name="z">Z position of the block to replace.</param>
        /// <param name="newBlock">The new block.</param>
        public void ReplaceBlock(int x, int y, int z, Block newBlock)
        {
            blocks[x, y, z] = newBlock;
            isDirty = true;
        }

        /// <summary>
        /// Rebuilds this chunk's geometry.
        /// </summary>
        public void Rebuild()
        {
            ReconstructBlockFaceVisibility();
            ReconstructVertices();

            // Reset dirty flag.
            isDirty = false;
        }

        /// <summary>
        /// Reconstructs the flags on all blocks that signify which faces are visible.
        /// </summary>
        public void ReconstructBlockFaceVisibility()
        {
            Block block;
            for (int x = 0; x < WIDTH; ++x)
            {
                for (int y = 0; y < HEIGHT; ++y)
                {
                    for (int z = 0; z < LENGTH; ++z)
                    {
                        block = BlockAt(x, y, z);

                        if (block == null)
                        {
                            continue;
                        }

                        // First, hide all faces
                        block.SetFaceHidden(AxisDirections.All);

                        Block adjacentBlock;

                        // Now show faces that are open to the air.
                        // +X face
                        adjacentBlock = BlockAt(x + 1, y, z);
                        if (adjacentBlock == null || adjacentBlock.BlockType.IsInvisible)
                        {
                            block.SetFaceVisible(AxisDirections.XPositive);
                        }

                        // -X face
                        adjacentBlock = BlockAt(x - 1, y, z);
                        if (adjacentBlock == null || adjacentBlock.BlockType.IsInvisible)
                        {
                            block.SetFaceVisible(AxisDirections.XNegative);
                        }

                        // +Y face
                        adjacentBlock = BlockAt(x, y + 1, z);
                        if (adjacentBlock == null || adjacentBlock.BlockType.IsInvisible)
                        {
                            block.SetFaceVisible(AxisDirections.YPositive);
                        }

                        // -Y face
                        adjacentBlock = BlockAt(x, y - 1, z);
                        if (adjacentBlock == null || adjacentBlock.BlockType.IsInvisible)
                        {
                            block.SetFaceVisible(AxisDirections.YNegative);
                        }

                        // +Z face
                        adjacentBlock = BlockAt(x, y, z + 1);
                        if (adjacentBlock == null || adjacentBlock.BlockType.IsInvisible)
                        {
                            block.SetFaceVisible(AxisDirections.ZPositive);
                        }

                        // -Z face
                        adjacentBlock = BlockAt(x, y, z - 1);
                        if (adjacentBlock == null || adjacentBlock.BlockType.IsInvisible)
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
        private void ReconstructVertices()
        {
            Block block;
            List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();
            // TODO: Use an array instead of a list

            primitiveCount = 0;

            for (int x = 0; x < WIDTH; ++x)
            {
                for (int y = 0; y < HEIGHT; ++y)
                {
                    for (int z = 0; z < LENGTH; ++z)
                    {
                        block = BlockAt(x, y, z);

                        // If the block is invisible, skip geometry creation.
                        if (block.BlockType.IsInvisible)
                        {
                            continue;
                        }

                        Vector3Int worldPos = new Vector3Int(chunkX * WIDTH + x, y, chunkZ * LENGTH + z);
                        //Console.WriteLine(worldPos.ToString());
                        // Add vertices for each face to chunk vertices

                        // Construct vectors for all corners
                        Vector3 xyz = new Vector3(worldPos.X, worldPos.Y, worldPos.Z);
                        Vector3 pxyz = new Vector3(worldPos.X + 1, worldPos.Y, worldPos.Z);
                        Vector3 xpyz = new Vector3(worldPos.X, worldPos.Y + 1, worldPos.Z);
                        Vector3 pxpyz = new Vector3(worldPos.X + 1, worldPos.Y + 1, worldPos.Z);
                        Vector3 xypz = new Vector3(worldPos.X, worldPos.Y, worldPos.Z + 1);
                        Vector3 pxypz = new Vector3(worldPos.X + 1, worldPos.Y, worldPos.Z + 1);
                        Vector3 xpypz = new Vector3(worldPos.X, worldPos.Y + 1, worldPos.Z + 1);
                        Vector3 pxpypz = new Vector3(worldPos.X + 1, worldPos.Y + 1, worldPos.Z + 1);

                        Vector2[] uvCoords; // UV co-ordinates for each vertex.

                        if ((block.VisibleFaces & AxisDirections.XNegative) > 0)
                        {
                            uvCoords = block.BlockType.GetUvCoordinates(AxisDirections.XNegative);

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
                            uvCoords = block.BlockType.GetUvCoordinates(AxisDirections.XPositive);

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
                            uvCoords = block.BlockType.GetUvCoordinates(AxisDirections.YNegative);

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
                            uvCoords = block.BlockType.GetUvCoordinates(AxisDirections.YPositive);

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
                            uvCoords = block.BlockType.GetUvCoordinates(AxisDirections.ZNegative);

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
                            uvCoords = block.BlockType.GetUvCoordinates(AxisDirections.ZPositive);

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

            // For some reason the list converts to an array in the wrong order, so reverse the list.
            vertices.Reverse();

            if (primitiveCount == 0)
            {
                vertexBuffer = null;
            }
            else
            {
                vertexBuffer = new VertexBuffer(GameServices.GraphicsDevice, typeof(VertexPositionColorTexture), vertices.Count, BufferUsage.WriteOnly);
                vertexBuffer.SetData<VertexPositionColorTexture>(vertices.ToArray());
            }
        }
    }
}

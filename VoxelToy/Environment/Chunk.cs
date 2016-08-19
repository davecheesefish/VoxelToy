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

        public VertexBuffer OpaqueVertexBuffer { get { return opaqueVertexBuffer; } }
        private VertexBuffer opaqueVertexBuffer;

        public int OpaquePrimitiveCount { get { return opaquePrimitiveCount; } }
        private int opaquePrimitiveCount = 0;

        public VertexBuffer TransparentVertexBuffer { get { return transparentVertexBuffer; } }
        private VertexBuffer transparentVertexBuffer;

        public int TransparentPrimitiveCount { get { return transparentPrimitiveCount; } }
        private int transparentPrimitiveCount = 0;


        /// <summary>
        /// Structure for vertex rebuild queue items.
        /// </summary>
        private struct VertexRebuildQueueItem
        {
            public Block Block;
            public Vector3Int Position;
        }
 

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
        public void Rebuild(Graphics.SceneRenderer renderer)
        {
            ReconstructBlockFaceVisibility();
            ReconstructVertices(renderer);

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
                        if (adjacentBlock == null || adjacentBlock.BlockType.IsInvisible || adjacentBlock.BlockType.IsAlphaBlended)
                        {
                            block.SetFaceVisible(AxisDirections.XPositive);
                        }

                        // -X face
                        adjacentBlock = BlockAt(x - 1, y, z);
                        if (adjacentBlock == null || adjacentBlock.BlockType.IsInvisible || adjacentBlock.BlockType.IsAlphaBlended)
                        {
                            block.SetFaceVisible(AxisDirections.XNegative);
                        }

                        // +Y face
                        adjacentBlock = BlockAt(x, y + 1, z);
                        if (adjacentBlock == null || adjacentBlock.BlockType.IsInvisible || adjacentBlock.BlockType.IsAlphaBlended)
                        {
                            block.SetFaceVisible(AxisDirections.YPositive);
                        }

                        // -Y face
                        adjacentBlock = BlockAt(x, y - 1, z);
                        if (adjacentBlock == null || adjacentBlock.BlockType.IsInvisible || adjacentBlock.BlockType.IsAlphaBlended)
                        {
                            block.SetFaceVisible(AxisDirections.YNegative);
                        }

                        // +Z face
                        adjacentBlock = BlockAt(x, y, z + 1);
                        if (adjacentBlock == null || adjacentBlock.BlockType.IsInvisible || adjacentBlock.BlockType.IsAlphaBlended)
                        {
                            block.SetFaceVisible(AxisDirections.ZPositive);
                        }

                        // -Z face
                        adjacentBlock = BlockAt(x, y, z - 1);
                        if (adjacentBlock == null || adjacentBlock.BlockType.IsInvisible || adjacentBlock.BlockType.IsAlphaBlended)
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
        /// <param name="renderer">The SceneRenderer which will be used to render the vertices.</param>
        /// <param name="alphaBlendedOnly">If true, only alpha-blended vertices will be reconstructed.</param>
        public void ReconstructVertices(Graphics.SceneRenderer renderer, bool alphaBlendedOnly = false)
        {
            Block block;
            List<VertexRebuildQueueItem> alphaBlendedBlocksToRebuild = new List<VertexRebuildQueueItem>(); // Using a List so this can be depth-sorted.
            Queue<VertexRebuildQueueItem> opaqueBlocksToRebuild = new Queue<VertexRebuildQueueItem>();
            List<VertexPositionColorTexture> alphaBlendedVertices = new List<VertexPositionColorTexture>();
            List<VertexPositionColorTexture> opaqueVertices = new List<VertexPositionColorTexture>();
            // TODO: Use an array instead of a list

            // Queue up blocks to be processed.
            for (int x = 0; x < WIDTH; ++x)
            {
                for (int y = 0; y < HEIGHT; ++y)
                {
                    for (int z = 0; z < LENGTH; ++z)
                    {
                        block = BlockAt(x, y, z);

                        if (block.BlockType.IsInvisible)
                        {
                            // Skip invisible blocks as they have no vertices.
                            continue;
                        }
                        else if (block.BlockType.IsAlphaBlended)
                        {
                            // Add alpha blended blocks to alpha blended queue.
                            VertexRebuildQueueItem queueItem = new VertexRebuildQueueItem();
                            queueItem.Block = block;
                            queueItem.Position = new Vector3Int(x, y, z);
                            alphaBlendedBlocksToRebuild.Add(queueItem);
                        }
                        else if (!alphaBlendedOnly)
                        {
                            // Add opaque blocks to opaque queue.
                            VertexRebuildQueueItem queueItem = new VertexRebuildQueueItem();
                            queueItem.Block = block;
                            queueItem.Position = new Vector3Int(x, y, z);
                            opaqueBlocksToRebuild.Enqueue(queueItem);
                        }
                    }
                }
            }

            // Depth-sort alpha blended block list.
            alphaBlendedBlocksToRebuild = alphaBlendedBlocksToRebuild.OrderBy(q => ((q.Position.ToVector3() - renderer.Camera.Position).LengthSquared())).ToList();

            // Reset primitive counts.
            transparentPrimitiveCount = 0;
            if (!alphaBlendedOnly)
            {
                opaquePrimitiveCount = 0;
            }

            // Process alpha-blended blocks.
            foreach (VertexRebuildQueueItem currentQueueItem in alphaBlendedBlocksToRebuild)
            {
                Vector3Int worldPosition = new Vector3Int(chunkX * WIDTH + currentQueueItem.Position.X, currentQueueItem.Position.Y, chunkZ * LENGTH + currentQueueItem.Position.Z);
                alphaBlendedVertices.AddRange(currentQueueItem.Block.GetVertices(worldPosition));
            }
            transparentPrimitiveCount = alphaBlendedVertices.Count / 3;

            // Process opaque blocks.
            if (!alphaBlendedOnly)
            {
                VertexRebuildQueueItem currentQueueItem;
                while (opaqueBlocksToRebuild.Count > 0)
                {
                    currentQueueItem = opaqueBlocksToRebuild.Dequeue();
                    Vector3Int worldPosition = new Vector3Int(chunkX * WIDTH + currentQueueItem.Position.X, currentQueueItem.Position.Y, chunkZ * LENGTH + currentQueueItem.Position.Z);
                    opaqueVertices.AddRange(currentQueueItem.Block.GetVertices(worldPosition));
                }
                opaquePrimitiveCount = opaqueVertices.Count / 3;
            }

            // For some reason list converts to an array in the wrong order, so reverse the lists.
            // TODO: Fix this.
            alphaBlendedVertices.Reverse();
            if (transparentPrimitiveCount == 0)
            {
                transparentVertexBuffer = null;
            }
            else
            {
                transparentVertexBuffer = new VertexBuffer(renderer.GraphicsDevice, typeof(VertexPositionColorTexture), alphaBlendedVertices.Count, BufferUsage.WriteOnly);
                transparentVertexBuffer.SetData<VertexPositionColorTexture>(alphaBlendedVertices.ToArray());
            }

            if (!alphaBlendedOnly)
            {
                opaqueVertices.Reverse();
                if (opaquePrimitiveCount == 0)
                {
                    opaqueVertexBuffer = null;
                }
                else
                {
                    opaqueVertexBuffer = new VertexBuffer(renderer.GraphicsDevice, typeof(VertexPositionColorTexture), opaqueVertices.Count, BufferUsage.WriteOnly);
                    opaqueVertexBuffer.SetData<VertexPositionColorTexture>(opaqueVertices.ToArray());
                }
            }
        }
    }
}

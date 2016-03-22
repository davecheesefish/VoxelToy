using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxelToy.Environment.Structures
{
    /// <summary>
    /// Base class for all pre-built structures (such as trees).
    /// </summary>
    abstract class Structure
    {
        /// <summary>
        /// The number of blocks wide this structure is (along the X axis).
        /// </summary>
        public int Width { get { return width; } }
        protected int width;

        /// <summary>
        /// The number of blocks tall this structure is (along the Y axis).
        /// </summary>
        public int Height { get { return height; } }
        protected int height;

        /// <summary>
        /// The number of blocks long this structure is (along the Z axis).
        /// </summary>
        public int Length { get { return length; } }
        protected int length;

        public Vector3Int Origin { get { return origin; } }
        protected Vector3Int origin;

        /// <summary>
        /// The blocks that make up this structure.
        /// </summary>
        protected Block[, ,] blocks;

        public Structure(Vector3Int origin, int width, int height, int length)
        {
            this.width = width;
            this.height = height;
            this.length = length;

            this.origin = origin;
        }

        /// <summary>
        /// Returns the blocks that make up this structure.
        /// </summary>
        /// <returns></returns>
        public virtual Block[, ,] GetBlocks()
        {
            return blocks;
        }

        /// <summary>
        /// Creates the blocks for this structure.
        /// </summary>
        /// <param name="types">An array of the BlockTypes used in this structure. Use a block type
        /// of null to leave any originally-generated blocks in place.</param>
        /// <param name="structure">The structure itself in a 3D array. This should be an array of
        /// indices referring to the block types in the first array argument. The second dimension
        /// of the array should be reversed so that the array literal looks like slices of the
        /// structure moving in the positive direction along the X axis.</param>
        protected void CreateBlocks(BlockType[] types, byte[, ,] structure)
        {
            blocks = new Block[width, height, length];

            BlockType blockType;
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    for (int z = 0; z < length; ++z)
                    {
                        blockType = types[structure[x, y, z]];

                        if (blockType == null)
                        {
                            blocks[x, height - y - 1, z] = null;
                        }
                        else
                        {
                            blocks[x, height - y - 1, z] = new Block(types[structure[x, y, z]]);
                        }
                    }
                }
            }
        }
    }
}

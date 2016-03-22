using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxelToy.Environment.Generators
{
    /// <summary>
    /// Class used to generate the blocks within a chunk of the world.
    /// </summary>
    abstract class TerrainGenerator
    {
        /// <summary>
        /// Generates an array of block types to fill a region of the world.
        /// </summary>
        /// <param name="chunk">The chunk to generate. The current contents of the chunk will be replaced.</param>
        public abstract void GenerateChunk(Chunk chunk);

        /// <summary>
        /// Adds pre-built structures (such as trees and buildings) to the world.
        /// </summary>
        /// <param name="world"></param>
        public virtual void GenerateStructures(World world)
        {
            // Default is a no-op
        }
    }
}

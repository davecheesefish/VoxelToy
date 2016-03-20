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
    }
}

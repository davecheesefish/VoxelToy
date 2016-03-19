using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxelToy.Environment.Generators
{
    class FlatTerrainGenerator:TerrainGenerator
    {
        private const int GROUND_LEVEL = 16;
        
        public override void GenerateChunk(Chunk chunk)
        {
            for (int x = 0; x < Chunk.WIDTH; ++x)
            {
                for (int y = 0; y < Chunk.HEIGHT; ++y)
                {
                    for (int z = 0; z < Chunk.LENGTH; ++z)
                    {
                        if (y > GROUND_LEVEL)
                        {
                            chunk.ReplaceBlock(x, y, z, new Block(BlockType.Get("air")));
                        }
                        else if (y == GROUND_LEVEL)
                        {
                            chunk.ReplaceBlock(x, y, z, new Block(BlockType.Get("grass")));
                        }
                        else
                        {
                            chunk.ReplaceBlock(x, y, z, new Block(BlockType.Get("dirt")));
                        }
                    }
                }
            }
        }
    }
}

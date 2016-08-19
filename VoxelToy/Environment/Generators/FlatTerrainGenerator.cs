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

        public override void GenerateStructures(World world)
        {
            int worldWidth = world.WidthInBlocks;
            int worldHeight = world.HeightInBlocks;
            int worldLength = world.LengthInBlocks;

            for (int x = 0; x < worldWidth; ++x)
            {
                for (int z = 0; z < worldLength; ++z)
                {
                    // First, see if we should create a structure here.
                    if (x % 10 == 0 && z % 10 == 0)
                    {
                        for (int y = worldHeight - 1; y >= GROUND_LEVEL; --y)
                        {
                            world.ReplaceBlock(x, y, z, new Block(BlockType.Get("debug")));
                        }
                    }
                }
            }
        }
    }
}

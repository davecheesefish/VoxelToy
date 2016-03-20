using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxelToy.Environment.Generators
{
    class PerlinTerrainGenerator:TerrainGenerator
    {
        private Noise.PerlinNoise perlin;

        public PerlinTerrainGenerator(int seed)
        {
            perlin = new Noise.PerlinNoise(seed);
        }

        public override void GenerateChunk(Chunk chunk)
        {
            int minGroundLevel = 15;
            int maxGroundLevel = 45;
            
            int groundLevel;
            for (int x = 0; x < Chunk.WIDTH; ++x)
            {
                for (int z = 0; z < Chunk.LENGTH; ++z)
                {
                    groundLevel = (int)(minGroundLevel + (maxGroundLevel - minGroundLevel) * perlin.OctaveGenerate2D(chunk.ChunkX * Chunk.WIDTH + x, chunk.ChunkZ * Chunk.LENGTH + z, 0.02, 2, 0.7));

                    for (int y = 0; y < Chunk.HEIGHT; ++y)
                    {
                        // Generate terrain
                        if (y > groundLevel)
                        {
                            chunk.ReplaceBlock(x, y, z, new Block(BlockType.Get("air")));
                        }
                        else if (y == groundLevel)
                        {
                            chunk.ReplaceBlock(x, y, z, new Block(BlockType.Get("grass")));
                        }
                        else
                        {
                            chunk.ReplaceBlock(x, y, z, new Block(BlockType.Get("dirt")));
                        }

                        // Generate caves
                        if (perlin.Generate3D(chunk.ChunkX * Chunk.WIDTH + x, y, chunk.ChunkZ * Chunk.LENGTH + z, 0.04) > 0.75)
                        {
                            chunk.ReplaceBlock(x, y, z, new Block(BlockType.Get("air")));
                        }
                    }
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxelToy.Environment.Generators
{
    class PerlinTerrainGenerator:TerrainGenerator
    {
        private int seed;
        private Noise.PerlinNoise perlin;

        public PerlinTerrainGenerator(int seed)
        {
            this.seed = seed;
            perlin = new Noise.PerlinNoise(seed);
        }

        public override void GenerateChunk(Chunk chunk)
        {
            int minGroundLevel = 0;
            int maxGroundLevel = 63;

            //double caveSize = 0.2; //0.25;
            //double caveFrequency = 0.2;
            
            int groundLevel;
            for (int x = 0; x < Chunk.WIDTH; ++x)
            {
                for (int z = 0; z < Chunk.LENGTH; ++z)
                {
                    groundLevel = (int)(minGroundLevel + (maxGroundLevel - minGroundLevel) * perlin.OctaveGenerate2D(chunk.ChunkX * Chunk.WIDTH + x, chunk.ChunkZ * Chunk.LENGTH + z, 0.012, 4, 0.45));
                    
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
                        //if (perlin.Generate3D(chunk.ChunkX * Chunk.WIDTH + x, y, chunk.ChunkZ * Chunk.LENGTH + z, caveFrequency) < caveSize)
                        //{
                        //    chunk.ReplaceBlock(x, y, z, new Block(BlockType.Get("air")));
                        //}
                    }
                }
            }
        }

        public override void GenerateStructures(World world)
        {
            Random random = new Random(seed);
            
            int worldWidth = world.WidthInBlocks;
            int worldHeight = world.HeightInBlocks;
            int worldLength = world.LengthInBlocks;
            
            // Trees
            Block block;
            BlockType airType = BlockType.Get("air");
            BlockType groundType = BlockType.Get("grass");
            for (int x = 0; x < worldWidth; ++x)
            {
                for (int z = 0; z < worldLength; ++z)
                {
                    // First, see if we should create a tree here.
                    if (perlin.Generate2D(x, z, 0.037) > 0.5 && random.Next(100) < 20)
                    {
                        for (int y = worldHeight - 1; y >= 0; --y)
                        {
                            block = world.BlockAt(x, y, z);

                            // If air or null, search the next block down
                            if (block == null || block.BlockType.Equals(airType))
                            {
                                continue;
                            }

                            // If not ground, end search here
                            if (!block.BlockType.Equals(groundType))
                            {
                                break;
                            }

                            // If the highest block is suitable ground, create a tree.
                            world.BuildStructure(x, y, z, new Structures.Trees.BasicTree());
                        }
                    }
                }
            }
        }
    }
}

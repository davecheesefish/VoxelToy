using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxelToy.Environment.Structures.Trees
{
    class BasicTree : Structure
    {
        private static BlockType[] blockTypes = { null, BlockType.Get("leaves"), BlockType.Get("log") };
        private static byte[, ,] structure = new byte[5, 8, 5]{
            {
                {0, 0, 0, 0, 0},
                {0, 0, 1, 0, 0},
                {0, 1, 1, 1, 0},
                {1, 1, 1, 1, 1},
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0}
            },                                                  
            {
                {0, 0, 0, 0, 0},
                {0, 1, 1, 1, 0},
                {1, 1, 1, 1, 1},
                {1, 1, 1, 1, 1},
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0}
            },
            {
                {0, 0, 1, 0, 0},
                {1, 1, 1, 1, 1},
                {1, 1, 1, 1, 1},
                {1, 1, 1, 1, 1},
                {0, 0, 2, 0, 0},
                {0, 0, 2, 0, 0},
                {0, 0, 2, 0, 0},
                {0, 0, 2, 0, 0}
            },
            {
                {0, 0, 0, 0, 0},
                {0, 1, 1, 1, 0},
                {1, 1, 1, 1, 1},
                {1, 1, 1, 1, 1},
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0}
            },
            {
                {0, 0, 0, 0, 0},
                {0, 0, 1, 0, 0},
                {0, 1, 1, 1, 0},
                {1, 1, 1, 1, 1},
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0}
            }
        };

        public BasicTree()
            : base(new Vector3Int(2, 0, 2), 5, 8, 5)
        {
            CreateBlocks(blockTypes, structure);
        }
    }
}

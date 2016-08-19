using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace VoxelToy.Environment
{
    class BlockType
    {
        public string Name { get { return name; } }
        private string name;

        /// <summary>
        /// Whether this block is drawable or not. This will generally be set to false, except on
        /// air blocks and other administrative blocks.
        /// </summary>
        public bool IsInvisible = false;

        /// <summary>
        /// How much light is blocked as it passes through.
        /// 0 is fully transparent. 255 is fully opaque.
        /// </summary>
        public byte Opacity = 255;

        /// <summary>
        /// Whether this block type has a transparent texture. Defaults to false.
        /// </summary>
        public bool IsAlphaBlended = false;

        private Rectangle topTextureRect;
        private Rectangle sideTextureRect;
        private Rectangle bottomTextureRect;

        private static Dictionary<string, BlockType> blockTypeRegistry;

        public BlockType(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Contructs a new BlockType.
        /// </summary>
        /// <param name="name">Name of this block type.</param>
        /// <param name="topTextureRect">Region of the block texture to use for the top of this block type.</param>
        /// <param name="sideTextureRect">Region of the block texture to use for the sides of this block type.</param>
        /// <param name="bottomTextureRect">Region of the block texture to use for the bottom of this block type.</param>
        public BlockType(string name, Rectangle topTextureRect, Rectangle sideTextureRect, Rectangle bottomTextureRect)
        {
            this.name = name;
            this.topTextureRect = topTextureRect;
            this.sideTextureRect = sideTextureRect;
            this.bottomTextureRect = bottomTextureRect;
        }

        /// <summary>
        /// Constructs a new BlockType.
        /// </summary>
        /// <param name="name">Name of this block type.</param>
        /// <param name="topTextureRect">Region of the block texture to use for the top of this block type.</param>
        /// <param name="undersideTextureRect">Region of the block texture to use for the sides and bottom of this block type.</param>
        public BlockType(string name, Rectangle topTextureRect, Rectangle undersideTextureRect) :
            this(name, topTextureRect, undersideTextureRect, undersideTextureRect)
        {
        }

        /// <summary>
        /// Constructs a new BlockType.
        /// </summary>
        /// <param name="name">Name of this block type.</param>
        /// <param name="textureRect">Region of the block texture to use for each face of this block type.</param>
        public BlockType(string name, Rectangle textureRect) :
            this(name, textureRect, textureRect, textureRect)
        {
        }

        /// <summary>
        /// Returns the UV co-ordinates for the block type.
        /// </summary>
        /// <param name="face">The face to return the co-ordinates for.</param>
        /// <param name="textureSize">Side length of the texture file containing the block textures.</param>
        /// <returns>An array of vectors representing the UV co-ordinates of the four corners, clockwise from the bottom-left.</returns>
        public Vector2[] GetUvCoordinates(AxisDirections face)
        {
            Rectangle textureRect;
            Vector2[] uvCoords = new Vector2[4];

            float top, left, bottom, right; // UV co-ordinates of the sides of the texture region
            switch (face)
            {
                // Top
                case AxisDirections.YPositive:
                    textureRect = topTextureRect;
                    break;

                // Bottom
                case AxisDirections.YNegative:
                    textureRect = bottomTextureRect;
                    break;

                // Sides
                case AxisDirections.XNegative:
                    // Fall through
                case AxisDirections.XPositive:
                    // Fall through
                case AxisDirections.ZNegative:
                    // Fall through
                case AxisDirections.ZPositive:
                    textureRect = sideTextureRect;
                    break;

                default:
                    throw new ArgumentException("Argument must be a single side.");
            }

            top = (float)textureRect.Top / (float)GameSettings.BLOCK_ATLAS_HEIGHT;
            left = (float)textureRect.Left / (float)GameSettings.BLOCK_ATLAS_WIDTH;
            bottom = (float)textureRect.Bottom / (float)GameSettings.BLOCK_ATLAS_HEIGHT;
            right = (float)textureRect.Right / (float)GameSettings.BLOCK_ATLAS_WIDTH;

            uvCoords[0] = new Vector2(left, bottom);
            uvCoords[1] = new Vector2(left, top);
            uvCoords[2] = new Vector2(right, top);
            uvCoords[3] = new Vector2(right, bottom);

            return uvCoords;
        }

        public static void Register(BlockType blockType, string id)
        {
            // Initialise the registry, if it isn't already.
            if (blockTypeRegistry == null)
            {
                blockTypeRegistry = new Dictionary<string, BlockType>();
            }

            blockTypeRegistry.Add(id, blockType);
        }

        public static BlockType Get(string id)
        {
            return blockTypeRegistry[id];
        }

        /// <summary>
        /// Registers the standard block types in the block type registry. This should be called once per launch, before a world is loaded.
        /// </summary>
        public static void RegisterStandardBlockTypes()
        {
            // Register air block type
            BlockType airBlockType = new BlockType("Air");
            airBlockType.IsInvisible = true;
            Register(airBlockType, "air");
            
            // Register other block types
            Register(new BlockType("Grass", new Rectangle(0, 0, 8, 8), new Rectangle(8, 0, 8, 8), new Rectangle(16, 0, 8, 8)), "grass");
            Register(new BlockType("Dirt", new Rectangle(16, 0, 8, 8)), "dirt");
            Register(new BlockType("Wooden log", new Rectangle(24, 0, 8, 8)), "log");

            BlockType leavesBlockType = new BlockType("Leaves", new Rectangle(32, 0, 8, 8));
            leavesBlockType.Opacity = 128;
            leavesBlockType.IsAlphaBlended = true;
            Register(leavesBlockType, "leaves");

            BlockType debugBlockType = new BlockType("Debug", new Rectangle(0, 8, 8, 8));
            debugBlockType.IsAlphaBlended = true;
            Register(debugBlockType, "debug");
        }
    }
}

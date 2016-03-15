using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelToy.Environment
{
    class Block
    {
        /// <summary>
        /// A bitfield value representing the visible faces of this block.
        /// Values match those in the AxisDirections enum.
        /// </summary>
        public AxisDirections VisibleFaces = 0;

        /// <summary>
        /// The type of block this is, e.g. a wood block.
        /// </summary>
        public BlockType BlockType { get { return blockType; } }
        private BlockType blockType;

        public Block(BlockType blockType)
        {
            this.blockType = blockType;
        }

        public void SetFaceVisible(AxisDirections face)
        {
            VisibleFaces |= face;
        }

        public void SetFaceHidden(AxisDirections face)
        {
            VisibleFaces &= 255 - face;
        }
    }
}

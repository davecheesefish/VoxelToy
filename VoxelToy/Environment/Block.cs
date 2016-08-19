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
        /// The light level of this block due to sunlight only.
        /// </summary>
        public byte Sunlight = 0;

        /// <summary>
        /// The type of block this is, e.g. a wood block.
        /// </summary>
        public BlockType BlockType { get { return blockType; } }
        private BlockType blockType;

        public Block(BlockType blockType)
        {
            this.blockType = blockType;
        }

        /// <summary>
        /// Used when rebuilding geometry, this function sets the given face as being visible,
        /// i.e. facing a transparent block.
        /// </summary>
        /// <param name="face"></param>
        public void SetFaceVisible(AxisDirections face)
        {
            VisibleFaces |= face;
        }

        /// <summary>
        /// Used when rebuilding geometry, this function sets the given face as being hidden,
        /// i.e. facing an opaque block.
        /// </summary>
        /// <param name="face"></param>
        public void SetFaceHidden(AxisDirections face)
        {
            VisibleFaces &= 255 - face;
        }

        /// <summary>
        /// Returns the list of vertices that make up this block.
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public List<VertexPositionColorTexture> GetVertices(Vector3Int worldPosition)
        {
            List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();

            // If the block is invisible, return empty list.
            if (blockType.IsInvisible)
            {
                return vertices;
            }

            // Add vertices for each face to chunk vertices

            // Construct vectors for all corners
            Vector3 xyz = new Vector3(worldPosition.X, worldPosition.Y, worldPosition.Z);
            Vector3 pxyz = new Vector3(worldPosition.X + 1, worldPosition.Y, worldPosition.Z);
            Vector3 xpyz = new Vector3(worldPosition.X, worldPosition.Y + 1, worldPosition.Z);
            Vector3 pxpyz = new Vector3(worldPosition.X + 1, worldPosition.Y + 1, worldPosition.Z);
            Vector3 xypz = new Vector3(worldPosition.X, worldPosition.Y, worldPosition.Z + 1);
            Vector3 pxypz = new Vector3(worldPosition.X + 1, worldPosition.Y, worldPosition.Z + 1);
            Vector3 xpypz = new Vector3(worldPosition.X, worldPosition.Y + 1, worldPosition.Z + 1);
            Vector3 pxpypz = new Vector3(worldPosition.X + 1, worldPosition.Y + 1, worldPosition.Z + 1);

            Vector2[] uvCoords; // UV co-ordinates for each vertex.

            if ((VisibleFaces & AxisDirections.XNegative) > 0)
            {
                uvCoords = blockType.GetUvCoordinates(AxisDirections.XNegative);

                vertices.Add(new VertexPositionColorTexture(xypz, Color.White, uvCoords[0]));
                vertices.Add(new VertexPositionColorTexture(xpypz, Color.White, uvCoords[1]));
                vertices.Add(new VertexPositionColorTexture(xpyz, Color.White, uvCoords[2]));

                vertices.Add(new VertexPositionColorTexture(xypz, Color.White, uvCoords[0]));
                vertices.Add(new VertexPositionColorTexture(xpyz, Color.White, uvCoords[2]));
                vertices.Add(new VertexPositionColorTexture(xyz, Color.White, uvCoords[3]));
            }

            if ((VisibleFaces & AxisDirections.XPositive) > 0)
            {
                uvCoords = blockType.GetUvCoordinates(AxisDirections.XPositive);

                vertices.Add(new VertexPositionColorTexture(pxyz, Color.White, uvCoords[0]));
                vertices.Add(new VertexPositionColorTexture(pxpyz, Color.White, uvCoords[1]));
                vertices.Add(new VertexPositionColorTexture(pxpypz, Color.White, uvCoords[2]));

                vertices.Add(new VertexPositionColorTexture(pxyz, Color.White, uvCoords[0]));
                vertices.Add(new VertexPositionColorTexture(pxpypz, Color.White, uvCoords[2]));
                vertices.Add(new VertexPositionColorTexture(pxypz, Color.White, uvCoords[3]));
            }

            if ((VisibleFaces & AxisDirections.YNegative) > 0)
            {
                uvCoords = blockType.GetUvCoordinates(AxisDirections.YNegative);

                vertices.Add(new VertexPositionColorTexture(xypz, Color.White, uvCoords[0]));
                vertices.Add(new VertexPositionColorTexture(xyz, Color.White, uvCoords[1]));
                vertices.Add(new VertexPositionColorTexture(pxyz, Color.White, uvCoords[2]));

                vertices.Add(new VertexPositionColorTexture(xypz, Color.White, uvCoords[0]));
                vertices.Add(new VertexPositionColorTexture(pxyz, Color.White, uvCoords[2]));
                vertices.Add(new VertexPositionColorTexture(pxypz, Color.White, uvCoords[3]));
            }

            if ((VisibleFaces & AxisDirections.YPositive) > 0)
            {
                uvCoords = blockType.GetUvCoordinates(AxisDirections.YPositive);

                vertices.Add(new VertexPositionColorTexture(xpyz, Color.White, uvCoords[0]));
                vertices.Add(new VertexPositionColorTexture(xpypz, Color.White, uvCoords[1]));
                vertices.Add(new VertexPositionColorTexture(pxpypz, Color.White, uvCoords[2]));

                vertices.Add(new VertexPositionColorTexture(xpyz, Color.White, uvCoords[0]));
                vertices.Add(new VertexPositionColorTexture(pxpypz, Color.White, uvCoords[2]));
                vertices.Add(new VertexPositionColorTexture(pxpyz, Color.White, uvCoords[3]));
            }

            if ((VisibleFaces & AxisDirections.ZNegative) > 0)
            {
                uvCoords = blockType.GetUvCoordinates(AxisDirections.ZNegative);

                vertices.Add(new VertexPositionColorTexture(xyz, Color.White, uvCoords[0]));
                vertices.Add(new VertexPositionColorTexture(xpyz, Color.White, uvCoords[1]));
                vertices.Add(new VertexPositionColorTexture(pxpyz, Color.White, uvCoords[2]));

                vertices.Add(new VertexPositionColorTexture(xyz, Color.White, uvCoords[0]));
                vertices.Add(new VertexPositionColorTexture(pxpyz, Color.White, uvCoords[2]));
                vertices.Add(new VertexPositionColorTexture(pxyz, Color.White, uvCoords[3]));
            }

            if ((VisibleFaces & AxisDirections.ZPositive) > 0)
            {
                uvCoords = blockType.GetUvCoordinates(AxisDirections.ZPositive);

                vertices.Add(new VertexPositionColorTexture(pxypz, Color.White, uvCoords[0]));
                vertices.Add(new VertexPositionColorTexture(pxpypz, Color.White, uvCoords[1]));
                vertices.Add(new VertexPositionColorTexture(xpypz, Color.White, uvCoords[2]));

                vertices.Add(new VertexPositionColorTexture(pxypz, Color.White, uvCoords[0]));
                vertices.Add(new VertexPositionColorTexture(xpypz, Color.White, uvCoords[2]));
                vertices.Add(new VertexPositionColorTexture(xypz, Color.White, uvCoords[3]));
            }

            return vertices;
        }
    }
}

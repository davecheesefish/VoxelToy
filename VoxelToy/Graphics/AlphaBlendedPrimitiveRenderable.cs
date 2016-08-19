using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelToy.Graphics
{
    struct AlphaBlendedPrimitiveRenderable
    {
        /// <summary>
        /// The position in the world used for depth sorting.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// The vertex buffer containing the geometry for this mesh.
        /// </summary>
        public VertexBuffer VertexBuffer;

        /// <summary>
        /// The effect to use when drawing this renderable.
        /// </summary>
        public Effect Effect;
    }
}

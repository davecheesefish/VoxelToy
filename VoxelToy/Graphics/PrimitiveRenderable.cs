using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelToy.Graphics
{
    /// <summary>
    /// Represents a renderable mesh made up of primitive shapes.
    /// </summary>
    struct PrimitiveRenderable
    {
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

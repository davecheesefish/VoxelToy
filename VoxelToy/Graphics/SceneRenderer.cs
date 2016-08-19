using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelToy.Graphics
{
    class SceneRenderer
    {
        /// <summary>
        /// The camera being used to render this scene.
        /// </summary>
        public Camera Camera { get { return camera; } }
        Camera camera;

        public GraphicsDevice GraphicsDevice { get { return graphicsDevice; } }
        private GraphicsDevice graphicsDevice;

        private List<PrimitiveRenderable> opaqueRenderables;
        private List<AlphaBlendedPrimitiveRenderable> alphaBlendedRenderables;

        public SceneRenderer(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;

            opaqueRenderables = new List<PrimitiveRenderable>();
            alphaBlendedRenderables = new List<AlphaBlendedPrimitiveRenderable>();
        }

        public void Begin(Camera camera)
        {
            this.camera = camera;
            opaqueRenderables.Clear();
            alphaBlendedRenderables.Clear();
        }

        /// <summary>
        /// Draws an opaque shape defined in a vertex buffer, using the given effect.
        /// </summary>
        /// <param name="vertexBuffer"></param>
        /// <param name="position"></param>
        /// <param name="effect"></param>
        public void DrawVertices(VertexBuffer vertexBuffer, Effect effect)
        {
            PrimitiveRenderable renderable = new PrimitiveRenderable();
            renderable.VertexBuffer = vertexBuffer;
            renderable.Effect = effect;
            opaqueRenderables.Add(renderable);
        }

        /// <summary>
        /// Draws an alpha-blended shape defined in a vertex buffer, using the given effect.
        /// </summary>
        /// <param name="vertexBuffer"></param>
        /// <param name="effect"></param>
        /// /// <param name="position">The position in the world to use for depth sorting.</param>
        public void DrawAlphaBlendedVertices(VertexBuffer vertexBuffer, Effect effect, Vector3 position)
        {
            AlphaBlendedPrimitiveRenderable renderable = new AlphaBlendedPrimitiveRenderable();
            renderable.VertexBuffer = vertexBuffer;
            renderable.Effect = effect;
            renderable.Position = position;
            alphaBlendedRenderables.Add(renderable);
        }

        /// <summary>
        /// Draws the queued graphics to the GraphicsDevice.
        /// </summary>
        public void End()
        {
            // Draw opaque geometry.
            // Enable use of the depth buffer.
            graphicsDevice.DepthStencilState = DepthStencilState.Default;

            graphicsDevice.BlendState = BlendState.Opaque;

            // Draw opaque renderables
            foreach (PrimitiveRenderable renderable in opaqueRenderables)
            {
                graphicsDevice.SetVertexBuffer(renderable.VertexBuffer);

                foreach (EffectPass pass in renderable.Effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, renderable.VertexBuffer.VertexCount / 3);
                }
            }


            // Draw semi-transparent geometry.
            // Make the depth buffer read-only to draw alpha-blended objects.
            graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

            graphicsDevice.BlendState = BlendState.AlphaBlend;

            //TODO: Don't re-sort on every frame.
            alphaBlendedRenderables = alphaBlendedRenderables.OrderByDescending(r => ((r.Position - camera.Position).LengthSquared())).ToList();

            foreach (AlphaBlendedPrimitiveRenderable renderable in alphaBlendedRenderables)
            {
                graphicsDevice.SetVertexBuffer(renderable.VertexBuffer);

                foreach (EffectPass pass in renderable.Effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, renderable.VertexBuffer.VertexCount / 3);
                }
            }


            // Set graphics device settings back to default.
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.BlendState = BlendState.Opaque;
        }
    }
}

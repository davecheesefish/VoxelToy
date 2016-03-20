using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace VoxelToy
{
    static class GameSettings
    {
        /// <summary>
        /// Width of the texture atlas for blocks, in texels.
        /// </summary>
        public const int BLOCK_ATLAS_WIDTH = 256;

        /// <summary>
        /// Height of the texture atlas for blocks, in texels.
        /// </summary>
        public const int BLOCK_ATLAS_HEIGHT = 256;
        
        /// <summary>
        /// The field ov fiew used for drawing the 3D world.
        /// </summary>
        public static float FieldOfView
        {
            get
            {
                return fieldOfView;
            }
            set
            {
                fieldOfView = value;
                RecalculatePfovMatrix();
            }
        }
        private static float fieldOfView;

        /// <summary>
        /// The perspective field of view matrix produced by the current display settings.
        /// </summary>
        public static Matrix PfovMatrix { get { return pfovMatrix; } }
        private static Matrix pfovMatrix;


        public static void Initialize()
        {
            // Set up screen resolution
            GameServices.GraphicsDeviceManager.PreferredBackBufferWidth = 1280;
            GameServices.GraphicsDeviceManager.PreferredBackBufferHeight = 720;
            GameServices.GraphicsDeviceManager.ApplyChanges();

            FieldOfView = (float)Math.PI * 0.35f;
        }

        private static void RecalculatePfovMatrix()
        {
            float aspectRatio = (float)GameServices.GraphicsDevice.Viewport.Width / (float)GameServices.GraphicsDevice.Viewport.Height;

            pfovMatrix = Matrix.CreatePerspectiveFieldOfView(FieldOfView, aspectRatio, 0.001f, 1000.0f);
        }
    }
}

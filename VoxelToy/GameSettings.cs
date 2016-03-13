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

            FieldOfView = (float)MathHelper.PiOver2;
        }

        private static void RecalculatePfovMatrix()
        {
            float aspectRatio = (float)GameServices.GraphicsDevice.Viewport.Width / (float)GameServices.GraphicsDevice.Viewport.Height;

            pfovMatrix = Matrix.CreatePerspectiveFieldOfView(FieldOfView, aspectRatio, 1.0f, 1000.0f);
        }
    }
}

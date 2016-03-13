using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelToy
{
    /// <summary>
    /// Utility class for providing access to game services.
    /// </summary>
    static class GameServices
    {
        public static GraphicsDeviceManager GraphicsDeviceManager;
        
        /// <summary>
        /// GraphicsDevice the game is being drawn to.
        /// </summary>
        public static GraphicsDevice GraphicsDevice;

        /// <summary>
        /// ContentManager for game content files.
        /// </summary>
        public static ContentManager ContentManager;

        public static Random Random;
    }
}

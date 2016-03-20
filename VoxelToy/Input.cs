using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace VoxelToy
{
    class Input
    {
        private Game game;
        
        private KeyboardState keyboardState;
        private KeyboardState pastKeyboardState;

        private MouseState mouseState;
        private MouseState pastMouseState;
        private Vector2 mouseMovement = new Vector2(0, 0);

        public bool MouseIsLocked { get { return mouseIsLocked; } }
        private bool mouseIsLocked = false;

        private Point lockedMousePosition;

        public Input(Game game)
        {
            this.game = game;
            lockedMousePosition = new Point(game.GraphicsDevice.Viewport.Width / 2, game.GraphicsDevice.Viewport.Height / 2);

            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            pastKeyboardState = keyboardState;
            pastMouseState = mouseState;
        }

        public void Update()
        {
            pastKeyboardState = keyboardState;
            pastMouseState = mouseState;
            
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            if (mouseIsLocked)
            {
                mouseMovement.X = mouseState.X - lockedMousePosition.X;
                mouseMovement.Y = mouseState.Y - lockedMousePosition.Y;
                Mouse.SetPosition(lockedMousePosition.X, lockedMousePosition.Y);
            }
            else
            {
                mouseMovement.X = mouseState.X - pastMouseState.X;
                mouseMovement.Y = mouseState.Y - pastMouseState.Y;
            }
        }

        /// <summary>
        /// Locks the mouse to the game window and hides the cursor.
        /// </summary>
        public void LockMouse()
        {
            game.IsMouseVisible = false;
            Mouse.SetPosition(lockedMousePosition.X, lockedMousePosition.Y);
            mouseIsLocked = true;
        }

        /// <summary>
        /// Unlocks the mouse from the game window and shows the cursor.
        /// </summary>
        public void UnlockMouse()
        {
            game.IsMouseVisible = true;
            mouseIsLocked = false;
        }

        /// <summary>
        /// Returns the mouse movement since the last update.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetMouseMovement()
        {
            return mouseMovement;
        }

        /// <summary>
        /// Returns true if the given key is pressed down.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsKeyDown(Keys key)
        {
            return keyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Returns true if the given key was pressed in the last update.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool KeyWasPressed(Keys key)
        {
            return keyboardState.IsKeyDown(key) && pastKeyboardState.IsKeyUp(key);
        }

        /// <summary>
        /// Returns true if the given key was released in the last update.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool KeyWasReleased(Keys key)
        {
            return keyboardState.IsKeyUp(key) && pastKeyboardState.IsKeyDown(key);
        }
    }
}

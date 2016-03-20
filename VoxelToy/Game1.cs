using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using VoxelToy.Environment;

namespace VoxelToy
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        FreeCamera camera;
        World world;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            
            // Set up game services container.
            GameServices.ContentManager = Content;
            GameServices.GraphicsDeviceManager = graphics;
            GameServices.GraphicsDevice = GraphicsDevice;
            GameServices.Random = new Random();

            GameSettings.Initialize();

            BlockType.RegisterStandardBlockTypes();

            world = new World(10, 10, new Environment.Generators.PerlinTerrainGenerator(500));
            camera = new FreeCamera(new Vector3(80, 40, 80));

            // Set sampler state to PointWrap to avoid blurry textures
            GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            world.LoadContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState kbState = Keyboard.GetState();
            // Right
            if (kbState.IsKeyDown(Keys.D))
            {
                camera.TranslateX(5.0f * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            // Left
            if (kbState.IsKeyDown(Keys.A))
            {
                camera.TranslateX(5.0f * -(float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            // Up
            if (kbState.IsKeyDown(Keys.LeftShift))
            {
                camera.TranslateY(5.0f * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            // Down
            if (kbState.IsKeyDown(Keys.LeftControl))
            {
                camera.TranslateY(5.0f * -(float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            // Forward
            if (kbState.IsKeyDown(Keys.W))
            {
                camera.TranslateZ(5.0f * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            // Backward
            if (kbState.IsKeyDown(Keys.S))
            {
                camera.TranslateZ(5.0f * -(float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            //camera.RotateX(0.01f);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.SkyBlue);

            world.Draw(camera);

            base.Draw(gameTime);
        }
    }
}

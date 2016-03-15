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

        Camera camera;
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

            world = new World(400, 20, 400);
            camera = new Camera(new Vector3(0, 0, 0), new Vector3(250, 10, 250));

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

            world.Update(gameTime);

            Vector3 cameraPos = camera.Position;
            cameraPos.X = camera.Target.X + 30.0f * (float)Math.Cos(gameTime.TotalGameTime.TotalSeconds / 3.0);
            cameraPos.Y = camera.Target.Y + 5.0f + (10.0f * ((float)Math.Cos(gameTime.TotalGameTime.TotalSeconds / 3.0) + 1.0f) / 2.0f);
            cameraPos.Z = camera.Target.Z + 50.0f * (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds / 3.0);
            camera.Position = cameraPos;

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

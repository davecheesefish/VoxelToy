using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using VoxelToy.Environment;
using VoxelToy.Graphics;

namespace VoxelToy
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SceneRenderer sceneRenderer;

        FreeCamera camera;
        World world;
        Input input;

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
            this.IsMouseVisible = false;
            input = new Input(this);
            input.LockMouse();

            // Set up game services container.
            GameServices.ContentManager = Content;
            GameServices.GraphicsDeviceManager = graphics;
            GameServices.GraphicsDevice = GraphicsDevice;
            GameServices.Random = new Random();

            GameSettings.Initialize();

            BlockType.RegisterStandardBlockTypes();

            world = new World(5, 5, new Environment.Generators.PerlinTerrainGenerator(2000000)); //new Environment.Generators.FlatTerrainGenerator());
            camera = new FreeCamera(new Vector3(5, 40, 5));

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

            // Create a new SceneRenderer, which can be used to draw 3D graphics.
            sceneRenderer = new SceneRenderer(GraphicsDevice);

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
            input.Update();

            // Mouse lock
            if (input.KeyWasPressed(Keys.M))
            {
                if (input.MouseIsLocked)
                {
                    input.UnlockMouse();
                }
                else
                {
                    input.LockMouse();
                }
            }

            // Right
            if (input.IsKeyDown(Keys.D))
            {
                camera.TranslateX(8.0f * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            // Left
            if (input.IsKeyDown(Keys.A))
            {
                camera.TranslateX(8.0f * -(float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            // Forward
            if (input.IsKeyDown(Keys.W))
            {
                camera.TranslateZ(8.0f * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            // Backward
            if (input.IsKeyDown(Keys.S))
            {
                camera.TranslateZ(8.0f * -(float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            // Rotation
            if (input.MouseIsLocked)
            {
                camera.RotateY(0.3f * (float)gameTime.ElapsedGameTime.TotalSeconds * (float)-input.GetMouseMovement().X);
                camera.RotateX(0.3f * (float)gameTime.ElapsedGameTime.TotalSeconds * (float)input.GetMouseMovement().Y);
            }

            if (input.IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.SkyBlue);

            // Draw the world.
            sceneRenderer.Begin(camera);
            world.Draw(sceneRenderer);
            sceneRenderer.End();

            base.Draw(gameTime);
        }
    }
}

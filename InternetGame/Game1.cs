﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace InternetGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private NetworkConnection _networkConnection;
        private InputManager inputManager;
        SpriteFont font;
        private Color color;

        private Texture2D texture;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _networkConnection = new NetworkConnection();
            inputManager = new InputManager(_networkConnection);
            color = Color.CornflowerBlue;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            if(_networkConnection.Start())
            {
                color = Color.Green;
            }
            else
            {
                color = Color.Red;
            }
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
            texture = Content.Load<Texture2D>("player");
            font = Content.Load<SpriteFont>("font");
            // TODO: use this.Content to load your game content here
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

            _networkConnection.Update();
            inputManager.Update(gameTime.ElapsedGameTime.Milliseconds);

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(color);

            spriteBatch.Begin();

            if (_networkConnection.Active)
            {
                foreach (var player in _networkConnection.Players)
                {
                    spriteBatch.DrawString(font, player.Name, new Vector2(player.XPosition, player.YPosition-20), Color.Black);
                    spriteBatch.Draw(texture, new Rectangle(player.XPosition, player.YPosition, 20, 20), Color.White);
                }
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

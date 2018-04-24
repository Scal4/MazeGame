using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace MazeGame
{
    enum GameState
    {
        StartScreen,
        Instructions,
        Game
    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font1;
        KeyboardState oldKb;

        Map CurrentMap;
        Map firstMap;

        // Enum stuff
        GameState gameState;

        // Rectangles
        Rectangle startScreenBackground;
        Rectangle selecterArrow;
        Texture2D allPurposeTexture;
        Texture2D startScreenBackgroundTexture;
        Texture2D selecterArrowTexture;
        double ScreenWidth;
        double ScreenHeight;

        Vector2 mapSize;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            ScreenHeight = graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            ScreenWidth = graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.ApplyChanges();
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
            gameState = GameState.StartScreen;

            startScreenBackground = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            selecterArrow = new Rectangle(250, 500, 50, 50);

            oldKb = Keyboard.GetState();

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

            // TODO: use this.Content to load your game content here
            allPurposeTexture = this.Content.Load<Texture2D>("white");
            startScreenBackgroundTexture = this.Content.Load<Texture2D>("StartScreenBackground");
            selecterArrowTexture = this.Content.Load<Texture2D>("arrow");
            font1 = this.Content.Load<SpriteFont>("SpriteFont1");

            firstMap = new Map(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, allPurposeTexture, "Content/MazeGameMap.txt");
            CurrentMap = firstMap;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            // escape key missing

            // Add your update logic here
            KeyboardState kb = Keyboard.GetState();

            if(gameState == GameState.StartScreen)
            {
                if(kb.IsKeyDown(Keys.Up) && oldKb != kb)
                {
                    selecterArrow.Y = selecterArrow.Y - 100;
                }

                if (kb.IsKeyDown(Keys.Down) && oldKb != kb)
                {
                    selecterArrow.Y = selecterArrow.Y + 100;
                }

                if(selecterArrow.Y < 500)
                {
                    selecterArrow.Y = 600;
                }

                if (selecterArrow.Y > 600)
                {
                    selecterArrow.Y = 500;
                }

                if(kb.IsKeyDown(Keys.Enter) && selecterArrow.Y == 500)
                {
                    gameState = GameState.Instructions;
                }

                if (kb.IsKeyDown(Keys.Enter) && selecterArrow.Y == 600)
                {
                    gameState = GameState.Game;
                }
            }

            oldKb = kb;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            // Draws start screen
            if (gameState == GameState.StartScreen)
            {
                spriteBatch.Draw(startScreenBackgroundTexture, startScreenBackground, Color.White);
                spriteBatch.DrawString(font1, "Use the Arrow Keys to move the arrow", new Vector2(70, 200), Color.Red);
                spriteBatch.DrawString(font1, "Use the Enter Key to select an option", new Vector2(75, 250), Color.Red);
                spriteBatch.DrawString(font1, "Instructions", new Vector2(350, 500), Color.Red);
                spriteBatch.DrawString(font1, "Start Game", new Vector2(350, 600), Color.Red);
                spriteBatch.Draw(selecterArrowTexture, selecterArrow, Color.White);
            }

            // Draws game screen
            if(gameState == GameState.Game)
            {
                for (int row = 0; row < CurrentMap.tileMap.GetLength(0); row++)
                {
                    for (int column = 0; column < CurrentMap.tileMap.GetLength(1); column++)
                    {
                        if (CurrentMap.tileMap[row, column] != null)
                            spriteBatch.Draw(CurrentMap.tileMap[row, column].TileTexture, CurrentMap.tileMap[row, column].TileRect, CurrentMap.tileMap[row, column].TileColor);
                    }
                }
            }
            
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

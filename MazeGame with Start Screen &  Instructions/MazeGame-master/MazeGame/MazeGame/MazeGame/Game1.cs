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

    enum InstructionState
    {
        Page1,
        Page2,
        Page3
    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font1;
        SpriteFont bigFont;
        KeyboardState oldKb;

        // Enum stuff
        GameState gameState;
        InstructionState instructionState;

        // Rectangles
        Rectangle selecterArrow;

        Tile[,] tileMap;
        Texture2D allPurposeTexture;
        Texture2D selecterArrowTexture;
        double ScreenWidth;
        double ScreenHeight;
        double AspectRatio;

        const float stringScale = 0.5625f;

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
            instructionState = InstructionState.Page1;

            selecterArrow = new Rectangle(GraphicsDevice.Viewport.Width / 3, 500, 50, 50);

            AspectRatio = ScreenWidth/ ScreenHeight;

            oldKb = Keyboard.GetState();

            IsMouseVisible = true;

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
            selecterArrowTexture = this.Content.Load<Texture2D>("arrow");
            font1 = this.Content.Load<SpriteFont>("SpriteFont1");
            bigFont = this.Content.Load<SpriteFont>("SpriteFont2");

            mapSize = makeTileMapArray();

            tileMap = new Tile[(int)mapSize.X,(int)mapSize.Y];

            makeFileMazeMap();
        }
        //Make tileMap [,]
        private Vector2 makeTileMapArray()
        {
            StreamReader reader = new StreamReader(@"Content/MazeGameMap.txt");

            int row = 0;
            int column = 0;

            while (!reader.EndOfStream)
            {
                string s = reader.ReadLine();
                row = s.Length;
                column++;
            }

            return new Vector2(row, column);

        }

        //Take map from text file and produces it
        private void makeFileMazeMap()
        {
            StreamReader reader = new StreamReader(@"Content/MazeGameMap.txt");

            int row = 0;
            int column = 0;

            while (!reader.EndOfStream)
            {
                string s = reader.ReadLine();

                char[] c = s.ToCharArray();
                

                for (int i = 0; i < c.Length; i++)
                { // Use switch case
                    if(c[i].Equals('1'))
                    {
                        tileMap[row, column] = new Tile(new Rectangle(row * (GraphicsDevice.Viewport.Width / (int)mapSize.X), column * (GraphicsDevice.Viewport.Height / (int)mapSize.Y),
                                                   GraphicsDevice.Viewport.Width / (int)mapSize.X, GraphicsDevice.Viewport.Height / (int)mapSize.Y), allPurposeTexture, Color.Black);
                    }
                    else
                    {
                        tileMap[row, column] = new Tile(new Rectangle(row * (GraphicsDevice.Viewport.Width / (int)mapSize.X), column * (GraphicsDevice.Viewport.Height / (int)mapSize.Y),
                                                   GraphicsDevice.Viewport.Width / (int)mapSize.X, GraphicsDevice.Viewport.Height / (int)mapSize.Y), allPurposeTexture, Color.Purple);
                    }
                    row++;
                }
                column++;
                row = 0;
            }
        }

        // Makes tileMap
        private void makeTile(int row, int column, Texture2D texture, Color color)
        {
            tileMap[row, column] = new Tile(
                           new Rectangle(row * (GraphicsDevice.Viewport.Width / (int)mapSize.X), column * (GraphicsDevice.Viewport.Height / (int)mapSize.Y),
                                                GraphicsDevice.Viewport.Width / (int)mapSize.X, GraphicsDevice.Viewport.Height / (int)mapSize.Y), texture, color);
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
            // Add your update logic here
            KeyboardState kb = Keyboard.GetState();

            // Code for start screen
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
                    selecterArrow.Y = 700;
                }

                if (selecterArrow.Y > 700)
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

                if (kb.IsKeyDown(Keys.Enter) && selecterArrow.Y == 700)
                {
                    this.Exit();
                }
            }

            // Code for insturction screen
            if(gameState == GameState.Instructions)
            {
                if(kb.IsKeyDown(Keys.Escape) && oldKb != kb)
                {
                    gameState = GameState.StartScreen;
                }

                if(kb.IsKeyDown(Keys.Right) && instructionState == InstructionState.Page1 && oldKb != kb)
                {
                    instructionState = InstructionState.Page2;
                }

                else if (kb.IsKeyDown(Keys.Right) && instructionState == InstructionState.Page2 && oldKb != kb)
                {
                    instructionState = InstructionState.Page3;
                }

                else if (kb.IsKeyDown(Keys.Left) && instructionState == InstructionState.Page3 && oldKb != kb)
                {
                    instructionState = InstructionState.Page2;
                }

                else if (kb.IsKeyDown(Keys.Left) && instructionState == InstructionState.Page2 && oldKb != kb)
                {
                    instructionState = InstructionState.Page1;
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
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            // Draws start screen
            if (gameState == GameState.StartScreen)
            {
                spriteBatch.DrawString(bigFont, "Crazy Mazey Tag", new Vector2(GraphicsDevice.Viewport.Width / 5, 25), Color.Blue, 0f, Vector2.Zero, (float)AspectRatio * stringScale, SpriteEffects.None, 0f);
                spriteBatch.DrawString(bigFont, "Crazy Mazey Tag", new Vector2(GraphicsDevice.Viewport.Width / 5, 25), Color.White, 0f, new Vector2(10, 0), (float)AspectRatio * stringScale, SpriteEffects.None, 
                                        0f);
                spriteBatch.DrawString(font1, "Use the Arrow Keys to move the arrow", new Vector2(GraphicsDevice.Viewport.Width / 4 - 30, 200), Color.Red, 0f, Vector2.Zero, (float)AspectRatio * stringScale, 
                                        SpriteEffects.None, 0f);
                spriteBatch.DrawString(font1, "Use the Enter Key to select an option", new Vector2(GraphicsDevice.Viewport.Width / 4 - 30, 250), Color.Red, 0f, Vector2.Zero, (float)AspectRatio * stringScale, 
                                        SpriteEffects.None, 0f);
                spriteBatch.DrawString(font1, "Instructions", new Vector2(GraphicsDevice.Viewport.Width / 3 + 100, 500), Color.Red, 0f, Vector2.Zero, (float)AspectRatio * stringScale, SpriteEffects.None, 0f);
                spriteBatch.DrawString(font1, "Start Game", new Vector2(GraphicsDevice.Viewport.Width / 3 + 100, 600), Color.Red, 0f, Vector2.Zero, (float)AspectRatio * stringScale, SpriteEffects.None, 0f);
                spriteBatch.DrawString(font1, "Exit", new Vector2(GraphicsDevice.Viewport.Width / 3 + 100, 700), Color.Red, 0f, Vector2.Zero, (float)AspectRatio * stringScale, SpriteEffects.None, 0f);
                spriteBatch.Draw(selecterArrowTexture, selecterArrow, Color.White);
            }

            // Draws the instructions screen
            if(gameState == GameState.Instructions)
            {
                if(instructionState == InstructionState.Page1)
                {
                    spriteBatch.DrawString(bigFont, "Instructions", new Vector2((int)ScreenWidth / 4, 25), Color.Blue, 0f, Vector2.Zero, (float)AspectRatio * stringScale, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(bigFont, "Instructions", new Vector2((int)ScreenWidth / 4, 25), Color.White, 0f, new Vector2(10, 0), (float)AspectRatio * stringScale, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(font1, "Controls:\nPlayer 1: W - Up, A - Left, S - Down, D - Right\nPlayer 2: Arrow Keys", new Vector2(GraphicsDevice.Viewport.Width / 4, 200),
                                            Color.Red, 0f, Vector2.Zero, (float)AspectRatio * stringScale, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(font1, "Red Square - Tagger", new Vector2(GraphicsDevice.Viewport.Width / 4, 400), Color.Red, 0f, Vector2.Zero, (float)AspectRatio * stringScale, 
                                            SpriteEffects.None, 0f);
                    spriteBatch.DrawString(font1, "White Square - Runner", new Vector2(GraphicsDevice.Viewport.Width / 4, 450), Color.White, 0f, Vector2.Zero, (float)AspectRatio * stringScale, 
                                            SpriteEffects.None, 0f);
                    spriteBatch.DrawString(font1, "Press Escape to go back", new Vector2(GraphicsDevice.Viewport.Width / 4, 800), Color.Red, 0f, Vector2.Zero, (float)AspectRatio * stringScale, 
                                            SpriteEffects.None, 0f);
                    spriteBatch.DrawString(font1, "Next Page - Right Arrow Key\nPrevious Page - Left Arrow Key", new Vector2(GraphicsDevice.Viewport.Width / 4, 850), Color.Red, 0f, Vector2.Zero, 
                                            (float)AspectRatio * stringScale, SpriteEffects.None, 0f);
                }

                else if(instructionState == InstructionState.Page2)
                {
                    spriteBatch.DrawString(bigFont, "Instructions", new Vector2((int)ScreenWidth / 4, 25), Color.Blue, 0f, Vector2.Zero, (float)AspectRatio * stringScale, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(bigFont, "Instructions", new Vector2((int)ScreenWidth / 4, 25), Color.White, 0f, new Vector2(10, 0), (float)AspectRatio * stringScale, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(font1, "Tiles:", new Vector2(GraphicsDevice.Viewport.Width / 6, 200), Color.Red, 0f, Vector2.Zero, (float)AspectRatio * stringScale, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(font1, "Door - Opens for a limited time when player uses it", new Vector2(GraphicsDevice.Viewport.Width / 6, 250), Color.Red, 0f, Vector2.Zero, 
                                            (float)AspectRatio * stringScale, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(font1, "Speed Tiles - Has the chance to either increase or decrease\nthe player's speed temporarily", new Vector2(GraphicsDevice.Viewport.Width / 6, 300), 
                                            Color.Red, 0f, Vector2.Zero, (float)AspectRatio * stringScale, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(font1, "Slow Block - Player's speed will be decreased for a short time", new Vector2(GraphicsDevice.Viewport.Width / 6, 400), Color.Red, 0f, Vector2.Zero, 
                                            (float)AspectRatio * stringScale, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(font1, "Speed Block - Player's speed will be increased for a short time", new Vector2(GraphicsDevice.Viewport.Width / 6, 450), Color.Red, 0f, Vector2.Zero, 
                                            (float)AspectRatio * stringScale, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(font1, "Press Escape to go back", new Vector2(GraphicsDevice.Viewport.Width / 4, 800), Color.Red, 0f, Vector2.Zero, (float)AspectRatio * stringScale, 
                                            SpriteEffects.None, 0f);
                    spriteBatch.DrawString(font1, "Next Page - Right Arrow Key\nPrevious Page - Left Arrow Key", new Vector2(GraphicsDevice.Viewport.Width / 4, 850), Color.Red, 0f, Vector2.Zero, 
                                            (float)AspectRatio * stringScale, SpriteEffects.None, 0f);
                }

                else if(instructionState == InstructionState.Page3)
                {
                    spriteBatch.DrawString(bigFont, "Instructions", new Vector2((int)ScreenWidth / 4, 25), Color.Blue, 0f, Vector2.Zero, (float)AspectRatio * stringScale, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(bigFont, "Instructions", new Vector2((int)ScreenWidth / 4, 25), Color.White, 0f, new Vector2(10, 0), (float)AspectRatio * stringScale, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(font1, "Power-Ups:", new Vector2(GraphicsDevice.Viewport.Width / 6, 200), Color.Red, 0f, Vector2.Zero, (float)AspectRatio * stringScale, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(font1, "Thunderbolt - Stuns other players for one second.\nRunner and Tagger can use this", new Vector2(GraphicsDevice.Viewport.Width / 6, 250), Color.Red, 0f, 
                                            Vector2.Zero, (float)AspectRatio * stringScale, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(font1, "Ghost - Ignore tile effects. Can pass through doors at any time.\nGhost cannot be tagged. Lasts for a few seconds", 
                                            new Vector2(GraphicsDevice.Viewport.Width / 6, 350), Color.Red, 0f, Vector2.Zero, (float)AspectRatio * stringScale, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(font1, "Flicker - Screen flashes, making it more difficult for\nplayers to see for 5 seconds", new Vector2(GraphicsDevice.Viewport.Width / 6, 450), 
                                            Color.Red, 0f, Vector2.Zero, (float)AspectRatio * stringScale, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(font1, "Press Escape to go back", new Vector2(GraphicsDevice.Viewport.Width / 4, 800), Color.Red, 0f, Vector2.Zero, (float)AspectRatio * stringScale, 
                                            SpriteEffects.None, 0f);
                    spriteBatch.DrawString(font1, "Next Page - Right Arrow Key\nPrevious Page - Left Arrow Key", new Vector2(GraphicsDevice.Viewport.Width / 4, 850), Color.Red, 0f, Vector2.Zero, 
                                            (float)AspectRatio * stringScale, SpriteEffects.None, 0f);
                }
            }

            // Draws game screen
            if(gameState == GameState.Game)
            {
                for (int row = 0; row < tileMap.GetLength(0); row++)
                {
                    for (int column = 0; column < tileMap.GetLength(1); column++)
                    {
                        if (tileMap[row, column] != null)
                            spriteBatch.Draw(tileMap[row, column].TileTexture, tileMap[row, column].TileRect, tileMap[row, column].TileColor);
                    }
                }
            }
            
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

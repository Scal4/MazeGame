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

        // Enum stuff
        GameState gameState;

        // Rectangles
        Rectangle startScreenBackground;
        Rectangle selecterArrow;

        Tile[,] tileMap;
        Texture2D allPurposeTexture;
        Texture2D startScreenBackgroundTexture;
        Texture2D selecterArrowTexture;
        Rectangle p1Border;
        Rectangle p2Border;
        Vector2 mapSize;
        Player p1;
        Player p2;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferHeight = 1000;
            graphics.PreferredBackBufferWidth = 1000;
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
            //1440x900
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

            mapSize = makeTileMapArray();
            p1 = new Player(true, new Rectangle(20, 20, 20, 20), Content.Load<Texture2D>("blank"));
            p2 = new Player(true, new Rectangle(80, 80, 20, 20), Content.Load<Texture2D>("blank"));
            p1Border = new Rectangle(p1.pRect.X + 2, p1.pRect.Y + 2, p1.pRect.Width - 4, p1.pRect.Height - 4);
            p2Border = new Rectangle(p2.pRect.X + 2, p2.pRect.Y + 2, p2.pRect.Width - 4, p2.pRect.Height - 4);
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
                    if(c[i].Equals('1')) // wall
                    {
                        addTile(row, column, Color.Black);
                    }
                    else if (c[i].Equals('D')) // door
                    {

                        addTile(row, column, Color.Yellow);
                    }
                    else // nothing floor
                    {
                        addTile(row, column, Color.Purple);
                    }
                    row++;
                }
                column++;
                row = 0;
            }

        }
        // add tile to tilemap
        private void addTile(int row,int column, Color color)
        {
            tileMap[row, column] = new Tile(new Rectangle(row * (GraphicsDevice.Viewport.Width / (int)mapSize.X), column * (GraphicsDevice.Viewport.Height / (int)mapSize.Y),
                                                      GraphicsDevice.Viewport.Width / (int)mapSize.X, GraphicsDevice.Viewport.Height / (int)mapSize.Y), allPurposeTexture, color);
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
             if (gameState == GameState.Game)
            {
                p1.update(1, p2);
                p2.update(2, p1);
                p1Border = new Rectangle(p1.pRect.X +2, p1.pRect.Y +2, p1.pRect.Width -4, p1.pRect.Height -4);
                p2Border = new Rectangle(p2.pRect.X +2, p2.pRect.Y +2, p2.pRect.Width -4, p2.pRect.Height -4);
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
                for (int row = 0; row < tileMap.GetLength(0); row++)
                {
                    for (int column = 0; column < tileMap.GetLength(1); column++)
                    {
                        if (tileMap[row, column] != null)
                            spriteBatch.Draw(tileMap[row, column].TileTexture, tileMap[row, column].TileRect, tileMap[row, column].TileColor);
                    }
                }
                spriteBatch.Draw(allPurposeTexture, p1.pRect, Color.Blue);
                spriteBatch.Draw(allPurposeTexture, p2.pRect, Color.Green);
                if(p1.it==false)
                {
                    spriteBatch.Draw(p1.pText, p1.pRect, Color.Red);
                    spriteBatch.Draw(p2.pText, p2.pRect, Color.White);
                }
                else
                {
                    spriteBatch.Draw(p1.pText, p1.pRect, Color.White);
                    spriteBatch.Draw(p2.pText, p2.pRect, Color.Red);
                }
                spriteBatch.DrawString(font1, "P1 score: " + p1.points, new Vector2(5, 10), Color.White);
                spriteBatch.DrawString(font1, "P2 score: " + p2.points, new Vector2(GraphicsDevice.Viewport.Width/2, 10), Color.White);
            }
            
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

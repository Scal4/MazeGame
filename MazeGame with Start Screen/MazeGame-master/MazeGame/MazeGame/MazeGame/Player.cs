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
    class Player
    {
        public int points;
        public bool it;
        const int minSpeed = 2;
        const int maxSpeed = 6;
        public int iFrames = 0;
        public Texture2D pText;
        public Rectangle pRect;
        int speed;
        KeyboardState old;
        int itTime = 0;
        public Player(bool i, Rectangle rect, Texture2D text)
        {
            old = Keyboard.GetState();
            pText = text;
            pRect = rect;
            it = i;
            points = 400;
            updateSpeed();
        }

        public void updateSpeed()
        {
            speed = points / 100;
            if (speed < 2)
            {
                speed = 2;
            }
            if (speed > 6)
            {
                speed = 6;
            }
        }
        public void update(int pindex, Player otherP)
        {
            if (iFrames != 0)
            {
                iFrames--;
            }
            if (it)
            {
                itTime++;
                if (itTime % 10 == 0)
                {
                    points--;
                    otherP.points++;
                }
                if (pRect.Intersects(otherP.pRect) && otherP.iFrames == 0)
                {
                    it = false;
                    otherP.it = true;
                    iFrames = 40;
                }
            }
            moveP(pindex);
            updateSpeed();
        }
        private void moveP(int pindex)
        {
            GamePadState gp;
            if (pindex == 1)
            {
                gp = GamePad.GetState(PlayerIndex.One);
            }
            else
            {
                gp = GamePad.GetState(PlayerIndex.Two);
            }
            KeyboardState kb = Keyboard.GetState();
            if (iFrames == 0)
            {
                pRect.X += (int)(gp.ThumbSticks.Left.X * speed);
                pRect.Y -= (int)(gp.ThumbSticks.Left.Y * speed);
                if (pindex == 1)
                {
                    if (kb.IsKeyDown(Keys.W))
                        pRect.Y -= speed;
                    if (kb.IsKeyDown(Keys.S))
                        pRect.Y += speed;
                    if (kb.IsKeyDown(Keys.A))
                        pRect.X -= speed;
                    if (kb.IsKeyDown(Keys.D))
                        pRect.X += speed;
                }
                if (pindex == 2)
                {
                    if (kb.IsKeyDown(Keys.Up))
                        pRect.Y -= speed;
                    if (kb.IsKeyDown(Keys.Down))
                        pRect.Y += speed;
                    if (kb.IsKeyDown(Keys.Left))
                        pRect.X -= speed;
                    if (kb.IsKeyDown(Keys.Right))
                        pRect.X += speed;
                }
            }
        }
    }
}

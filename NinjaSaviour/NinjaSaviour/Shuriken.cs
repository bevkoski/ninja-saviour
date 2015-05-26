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

namespace NinjaSaviour
{
    class Shuriken
    {
        // general
        int X;
        int Y;
        bool active;
        bool flying;
        
        // graphics
        Texture2D sprite;
        const int IMAGES_PER_ROW = 2;
        int column;
        int frameWidth;
        Rectangle drawRectangle = new Rectangle();
        Rectangle sourceRectangle;

        // movement
        char direction;
        const int MOVE_AMOUNT = 10;
        const int CELL_WIDTH = 20;
        const int STEP = 150;
        int elapsedStepMS = 0;

        public Shuriken(ContentManager contentManager, string spriteName, int X, int Y, bool active)
        {
            // set draw rectangle location
            this.X = X;
            this.Y = Y;
            drawRectangle.X = X;
            drawRectangle.Y = Y;

            // initialize parameters
            column = 0;
            this.active = active;
            flying = false;

            // load content
            LoadContent(contentManager, spriteName);
        }

        public Rectangle CollisionRectangle
        {
            get { return this.drawRectangle; }
        }

        public int Xpos
        {
            get { return X; }
            set { X = value; }
        }

        public int Ypos
        {
            get { return Y; }
            set { Y = value; }
        }
       
        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        public bool Flying
        {
            get { return flying; }
            set { flying = value; }
        }

        public char Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        private void LoadContent(ContentManager contentManager, string spriteName)
        {
            // load content, calculate image width, and set remainder of draw rectangle
            sprite = contentManager.Load<Texture2D>(spriteName);
            frameWidth = sprite.Width / IMAGES_PER_ROW;
            drawRectangle.Width = frameWidth;
            drawRectangle.Height = sprite.Height;

            // set initial source rectangle
            sourceRectangle = new Rectangle(0, 0, frameWidth, sprite.Height);
        }

        public void Update(GameTime gameTime, Ninja ninja, int[,] grid)
        {
            // check for a collision between the shuriken and the ninja
            if (Math.Abs(ninja.CollisionRectangle.Center.X - this.CollisionRectangle.Center.X) < this.frameWidth / 2
                && Math.Abs(ninja.CollisionRectangle.Center.Y - this.CollisionRectangle.Center.Y) < this.frameWidth / 2
                && ninja.Offset != 3
                && active == true)
            {
                active = false;
                ninja.Offset = 3;
                ninja.Shuriken = this;
            }

            // check for a throw command
            if (flying)
            {
                // update time
                elapsedStepMS += gameTime.ElapsedGameTime.Milliseconds;

                // check the position
                int positionX = 1;
                int positionY = 1;
                if (X % 20 != 0)
                    positionX = 0;
                if (Y % 20 != 0)
                    positionY = 0;

                bool flag;

                // make the shuriken fly
                if (direction == 'U')
                {
                    // check for an obstacle
                    flag = true;
                    for (int i = 0; i < 4 - positionX; i++)
                    {
                        if (grid[(Y / CELL_WIDTH) - positionY + 1, (X / CELL_WIDTH) + i] == 1)
                            flag = false;
                    }

                    // if there is no obstacle
                    if (flag)
                    {
                        Y -= MOVE_AMOUNT;

                        // check the elased time
                        if (elapsedStepMS >= STEP)
                        {
                            elapsedStepMS = 0;

                            // change the column for the sprite sheet
                            if (column == 0)
                                column = 1;
                            else
                                column = 0;
                        }
                    }
                    else
                    {
                        // destroy the shuriken
                        flying = false;
                    }
                }
                else if (direction == 'D')
                {
                    // check for an obstacle
                    flag = true;
                    for (int i = 0; i < 4 - positionX; i++)
                    {
                        if (grid[(Y / CELL_WIDTH) + 2, (X / CELL_WIDTH) + i] == 1)
                            flag = false;
                    }

                    // if there is no obstacle
                    if (flag)
                    {
                        Y += MOVE_AMOUNT;

                        // check the elased time
                        if (elapsedStepMS >= STEP)
                        {
                            elapsedStepMS = 0;

                            // change the column for the sprite sheet
                            if (column == 0)
                                column = 1;
                            else
                                column = 0;
                        }
                    }
                    else
                    {
                        // destroy the shuriken
                        flying = false;
                    }
                }
                else if (direction == 'R')
                {
                    // check for an obstacle
                    flag = true;
                    for (int i = 0; i < 4 - positionY; i++)
                    {
                        if (grid[(Y / CELL_WIDTH) + i, (X / CELL_WIDTH) + 2] == 1)
                            flag = false;
                    }

                    // if there is no obstacle
                    if (flag)
                    {
                        X += MOVE_AMOUNT;

                        // check the elased time
                        if (elapsedStepMS >= STEP)
                        {
                            elapsedStepMS = 0;

                            // change the column for the sprite sheet
                            if (column == 0)
                                column = 1;
                            else
                                column = 0;
                        }
                    }
                    else
                    {
                        // destroy the shuriken
                        flying = false;
                    }
                }
                else if (direction == 'L')
                {
                    // check for an obstacle
                    flag = true;
                    for (int i = 0; i < 4 - positionY; i++)
                    {
                        if (grid[(Y / CELL_WIDTH) + i, (X / CELL_WIDTH) - positionX + 1] == 1)
                            flag = false;
                    }

                    // if there is no obstacle
                    if (flag)
                    {
                        X -= MOVE_AMOUNT;

                        // check the elased time
                        if (elapsedStepMS >= STEP)
                        {
                            elapsedStepMS = 0;

                            // change the column for the sprite sheet
                            if (column == 0)
                                column = 1;
                            else
                                column = 0;
                        }
                    }
                    else
                    {
                        // destroy the shuriken
                        flying = false;
                    }
                }
            }

            // update source rectangle
            sourceRectangle.X = column * frameWidth;

            // update draw rectangle
            drawRectangle.X = this.X;
            drawRectangle.Y = this.Y;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (active || flying)
                spriteBatch.Draw(sprite, drawRectangle, sourceRectangle, Color.White);
        }
    }
}

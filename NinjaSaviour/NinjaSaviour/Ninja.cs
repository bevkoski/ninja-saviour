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
    class Ninja
    {
        // general
        int X;
        int Y;

        // graphics
        Texture2D sprite;
        const int IMAGES_PER_ROW = 6;
        int row;
        int column;
        int frameWidth;
        Rectangle drawRectangle = new Rectangle();
        Rectangle sourceRectangle;
        int offset;

        // movement
        const int MOVE_AMOUNT = 4;
        const int CELL_WIDTH = 20;
        const int STEP = 300;
        int elapsedStepMS = 0;

        // shuriken
        Shuriken shuriken;

        public Ninja(ContentManager contentManager, string spriteName)
        {
            // set draw rectangle location
            X = 40;
            Y = 500;
            drawRectangle.X = X;
            drawRectangle.Y = Y;

            // initialize parameters
            row = 0;
            column = 0;
            offset = 0;
            shuriken = null;

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

        public int Offset
        {
            get { return this.offset; }
            set { this.offset = value; }
        }

        public Shuriken Shuriken
        {
            get { return shuriken; }
            set { shuriken = value; }
        }

        private void LoadContent(ContentManager contentManager, string spriteName)
        {
            // load content, calculate image width, and set remainder of draw rectangle
            sprite = contentManager.Load<Texture2D>(spriteName);
            frameWidth = sprite.Width / IMAGES_PER_ROW;
            drawRectangle.Width = frameWidth;
            drawRectangle.Height = sprite.Height/2;

            // set initial source rectangle
            sourceRectangle = new Rectangle(column*frameWidth, row*sprite.Height/2, frameWidth, sprite.Height/2);
        }

        public void Update(GameTime gameTime, KeyboardState keyboard, Level level)
        {
            // get the level data
            int[,] grid = level.getGrid;

            // check the position
            int positionX = 1;
            int positionY = 1;
            if (X % 20 != 0)
                positionX = 0;
            if (Y % 20 != 0)
                positionY = 0;

            // update time
            elapsedStepMS += gameTime.ElapsedGameTime.Milliseconds;

            bool flag;
            
            // move the ninja based on the keyboard state
            if (keyboard.IsKeyDown(Keys.Right) ||
                keyboard.IsKeyDown(Keys.D))
            {
                // check for an obstacle
                flag = true;
                for (int i = 0; i < 4 - positionY; i++)
                {
                    if (grid[(Y / CELL_WIDTH) + i, (X / CELL_WIDTH) + 3] == 1)
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
                        if (column == 0 || column == 1)
                            column = 2;
                        else
                            column = 1;
                    }
                }
                else
                {
                    // change the column for the sprite sheet
                    column = 0;
                }
            }
            else if (keyboard.IsKeyDown(Keys.Left) ||
                keyboard.IsKeyDown(Keys.A))
            {
                // check for an obstacle
                flag = true;
                for (int i = 0; i < 4 - positionY; i++)
                {
                    if (grid[(Y / CELL_WIDTH) + i, (X / CELL_WIDTH) - positionX] == 1)
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
                        if (column == 0 || column == 1)
                            column = 2;
                        else
                            column = 1;
                    }
                }
                else
                {
                    // change the column for the sprite sheet
                    column = 0;
                }
            }
            else if (keyboard.IsKeyDown(Keys.Up) ||
                keyboard.IsKeyDown(Keys.W))
            {
                // check for an obstacle
                flag = true;
                for (int i = 0; i < 4 - positionX; i++)
                {
                    if (grid[(Y / CELL_WIDTH) - positionY, (X / CELL_WIDTH) + i] == 1)
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
                        if (column == 0 || column == 1)
                            column = 2;
                        else
                            column = 1;
                    }
                }
                else
                {
                    // change the column for the sprite sheet
                    column = 0;
                }

                // change the row for the sprite sheet
                row = 1;
            }
            else if (keyboard.IsKeyDown(Keys.Down) ||
                keyboard.IsKeyDown(Keys.S))
            {
                // check for an obstacle
                flag = true;
                for (int i = 0; i <  4 - positionX; i++)
                {
                    if (grid[(Y / CELL_WIDTH) + 3, (X / CELL_WIDTH) + i] == 1)
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
                        if (column == 0 || column == 1)
                            column = 2;
                        else
                            column = 1;
                    }
                }
                else
                {
                    // change the column for the sprite sheet
                    column = 0;
                }

                // change the row for the sprite sheet
                row = 0;
            }
            else if (keyboard.GetPressedKeys().Length == 0)
            {
                column = 0;
            }

            // throw a shuriken
            if (shuriken != null && (keyboard.IsKeyDown(Keys.Space) || keyboard.IsKeyDown(Keys.Enter)))
            {
                if (keyboard.IsKeyDown(Keys.Up) || keyboard.IsKeyDown(Keys.W))
                    Throw('U');
                else if (keyboard.IsKeyDown(Keys.Down) || keyboard.IsKeyDown(Keys.S))
                    Throw('D');
                else if (keyboard.IsKeyDown(Keys.Right) || keyboard.IsKeyDown(Keys.D))
                    Throw('R');
                else if (keyboard.IsKeyDown(Keys.Left) || keyboard.IsKeyDown(Keys.A))
                    Throw('L');
            }

            // update source rectangle
            sourceRectangle.X = (column + offset) * frameWidth;
            sourceRectangle.Y = row * sprite.Height / 2;

            // update draw rectangle
            drawRectangle.X = X;
            drawRectangle.Y = Y;

            // update shuriken
            if (shuriken != null)
            {
                shuriken.Xpos = X;
                shuriken.Ypos = Y;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, drawRectangle, sourceRectangle, Color.White);
        }

        private void Throw(char direction)
        {
            shuriken.Direction = direction;
            shuriken.Flying = true;
            offset = 0;
            shuriken = null;
        }
    }
}

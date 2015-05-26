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
    class Enemy
    {
        // general
        int X;
        int Y;
        bool active;
        int moving;
        bool dying;

        // graphics
        Texture2D sprite;
        const int IMAGES_PER_ROW = 4;
        int type;
        int column;
        int frameWidth;
        Rectangle drawRectangle = new Rectangle();
        Rectangle sourceRectangle;
        const int DYING_MS = 200;
        const int DYING_STEP = 75;
        int elapsedDyingMS = 0;
        int totalDyingMS = 0;


        // movement
        int axis;
        int direction;
        const int MOVE_AMOUNT = 2;
        const int CELL_WIDTH = 20;
        const int STEP = 300;
        int elapsedStepMS = 0;

        public Enemy(ContentManager contentManager, string spriteName, int X, int Y, int type, int axis, int moving)
        {
            // set draw rectangle location
            this.X = X;
            this.Y = Y;
            drawRectangle.X = X;
            drawRectangle.Y = Y;

            // initialize parameters
            active = true;
            this.moving = moving;
            this.type = type;
            this.axis = axis;
            this.direction = 1;
            column = 0;
            dying = false;

            // load content
            LoadContent(contentManager, spriteName);
        }

        public Rectangle CollisionRectangle
        {
            get { return this.drawRectangle; }
        }

        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        public bool Dying
        {
            get { return dying; }
            set { dying = value; }
        }

        public int Moving
        {
            get { return moving; }
            set { moving = value; }
        }

        private void LoadContent(ContentManager contentManager, string spriteName)
        {
            // load content, calculate image width, and set remainder of draw rectangle
            sprite = contentManager.Load<Texture2D>(spriteName);
            frameWidth = sprite.Width / IMAGES_PER_ROW;
            drawRectangle.Width = frameWidth;
            drawRectangle.Height = sprite.Height / 3;

            // set initial source rectangle
            sourceRectangle = new Rectangle(0, 60 * type, frameWidth, sprite.Height / 3);
        }

        public void Update(GameTime gameTime, int[,] grid)
        {
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

            if (dying)
            {
                if (column == 1 || column == 2)
                {
                    column = 3;
                    elapsedDyingMS += gameTime.ElapsedGameTime.Milliseconds;
                    totalDyingMS += gameTime.ElapsedGameTime.Milliseconds;
                }
                else
                {
                    if (totalDyingMS > DYING_MS)
                        dying = false;
                    else
                    {
                        if (elapsedDyingMS > DYING_STEP)
                        {
                            if (column == 0)
                                column = 3;
                            else
                                column = 0;

                            elapsedDyingMS = 0;
                        }

                        elapsedDyingMS += gameTime.ElapsedGameTime.Milliseconds;
                        totalDyingMS += gameTime.ElapsedGameTime.Milliseconds;
                    }
                }
            }
            else if (axis == 0)
            {
                if (direction == 1)
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
                            if (column == 1)
                                column = 2;
                            else
                                column = 1;
                        }
                    }
                    else
                    {
                        // change direction
                        direction = -1;
                    }
                }
                if (direction == -1)
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
                            if (column == 1)
                                column = 2;
                            else
                                column = 1;
                        }
                    }
                    else
                    {
                        // change direction
                        direction = 1;
                    }
                }
            }
            else if (axis == 1)
            {
                if (direction == 1)
                {
                    // check for an obstacle
                    flag = true;
                    for (int i = 0; i < 4 - positionX; i++)
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
                        // change direction
                        direction = -1;
                    }
                }
                if (direction == -1)
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
                        // change direction
                        direction = 1;
                    }
                }
            }

            // update source rectangle
            sourceRectangle.X = column * frameWidth;
            sourceRectangle.Y = type * sprite.Height / 3;

            // update draw rectangle
            drawRectangle.X = X;
            drawRectangle.Y = Y;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (active || dying)
                spriteBatch.Draw(sprite, drawRectangle, sourceRectangle, Color.White);
        }
    }
}

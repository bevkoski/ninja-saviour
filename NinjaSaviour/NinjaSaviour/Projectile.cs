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
    class Projectile
    {
        // general
        int X;
        int Y;
        bool active;
        int number;

        // graphics
        Texture2D sprite;
        const int IMAGES_PER_ROW = 2;
        int column;
        int frameWidth;
        Rectangle drawRectangle = new Rectangle();
        Rectangle sourceRectangle;

        // movement
        const int MOVE_AMOUNT = 10;
        const int CELL_WIDTH = 20;
        const int STEP = 100;
        int elapsedStepMS = 0;

        public Projectile(ContentManager contentManager, string spriteName, int X, int Y, int number)
        {
            // set draw rectangle location
            this.X = X;
            this.Y = Y;
            drawRectangle.X = X;
            drawRectangle.Y = Y;

            // initialize parameters
            this.number = number;
            column = 0;
            active = false;

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

        public int Number
        {
            get { return number; }
            set { number = value; }
        }

        public bool Active
        {
            get { return active; }
            set { active = value; }
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

        public void Update(GameTime gameTime, Level level)
        {
            // check for an attack command
            if (active)
            {
                elapsedStepMS += gameTime.ElapsedGameTime.Milliseconds;

                if (X != 40)
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
                    X = 700;
                    active = false;

                    if (level != null)
                    {
                        level.setShuriken(40, Y);
                    }
                }

                // update source rectangle
                sourceRectangle.X = column * frameWidth;

                // update draw rectangle
                drawRectangle.X = this.X;
                drawRectangle.Y = this.Y;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (active)
                spriteBatch.Draw(sprite, drawRectangle, sourceRectangle, Color.White);
        }
    }
}

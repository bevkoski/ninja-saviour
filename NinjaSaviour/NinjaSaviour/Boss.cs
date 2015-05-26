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
    class Boss
    {
        // general
        int X;
        int Y;
        bool active;
        int attackTimer;
        Random random = new Random();
        int life = 3;

        // graphics
        Texture2D sprite;
        const int IMAGES_PER_ROW = 3;
        int column;
        int frameWidth;
        Rectangle drawRectangle = new Rectangle();
        Rectangle sourceRectangle;

        // movement
        int direction;
        const int MOVE_AMOUNT = 2;
        const int CELL_WIDTH = 20;
        const int STEP = 300;
        int elapsedStepMS = 0;
        int attackStepMS = 0;

        // projectiles
        List<Projectile> projectiles;
        Projectile mainProjectile;
        int turn;

        public Boss(ContentManager contentManager, string spriteName, int X, int Y)
        {
            // set draw rectangle location
            this.X = X;
            this.Y = Y;
            drawRectangle.X = X;
            drawRectangle.Y = Y;

            // initialize parameters
            active = false;
            this.direction = 1;
            column = 0;
            attackTimer = 1000 + random.Next(0, 5000);

            // initialize projectiles
            projectiles = new List<Projectile>();
            initializeProjectiles(contentManager);
            turn = random.Next(0, 3);

            // load content
            LoadContent(contentManager, spriteName);
        }

        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        public Rectangle CollisionRectangle
        {
            get { return this.drawRectangle; }
        }

        public int Life
        {
            get { return life; }
            set { life = value; }
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
            // update time
            elapsedStepMS += gameTime.ElapsedGameTime.Milliseconds;
            attackStepMS += gameTime.ElapsedGameTime.Milliseconds;
            if (elapsedStepMS == 0)
            {
                direction = 1;
                column = 0;
            }

            // check for an attack
            if (attackStepMS >= (1000 + attackTimer))
            {
                // animate attack
                attackStepMS = -2000;
                elapsedStepMS = -2000;
                direction = 0;
                column = 2;
                attackTimer = random.Next(0, 5000);

                bool flag = false;

                // throw shurikens
                foreach (Projectile projectile in projectiles)
                {
                    if (turn == 0)
                    {
                        flag = true;
                        if (projectile.Number % 2 == 0)
                        {
                            projectile.Active = true;
                        }
                    }
                    else if (turn == 1)
                    {
                        flag = true;
                        if (projectile.Number % 2 == 1)
                        {
                            projectile.Active = true;
                        }
                    }
                }
                if (turn == 2)
                {
                    flag = true;
                    mainProjectile.Active = true;
                }

                if (flag)
                    turn = random.Next(0, 3);
            }
            else
            {
                if (direction == 1)
                {
                    if (Y != 440)
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
                        // change direction
                        direction = -1;
                    }
                }
                if (direction == -1)
                {
                    if (Y != 100)
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
                        // change direction
                        direction = 1;
                    }
                }
            }

            // check for a death
            if (life == 0)
            {
                active = false;
            }

            // update projectiles
            foreach (Projectile projectile in projectiles)
            {
                projectile.Update(gameTime, null);
            }
            mainProjectile.Ypos = Y;
            mainProjectile.Update(gameTime, level);

            // update source rectangle
            sourceRectangle.X = column * frameWidth;

            // update draw rectangle
            drawRectangle.X = X;
            drawRectangle.Y = Y;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (active)
            {
                spriteBatch.Draw(sprite, drawRectangle, sourceRectangle, Color.White);

                foreach (Projectile projectile in projectiles)
                {
                    projectile.Draw(spriteBatch);
                }
                mainProjectile.Draw(spriteBatch);
            }
        }

        public void initializeProjectiles(ContentManager contentManager)
        {
            projectiles = new List<Projectile>();
            for (int i = 100; i <= 500; i = i + 100)
            {
                Projectile projectile = new Projectile(contentManager, "bossshuriken", 700, i, i / 100);
                projectiles.Add(projectile);
            }
            mainProjectile = new Projectile(contentManager, "bossshuriken", 700, Y, 6);
        }

        public bool checkCollision(Ninja ninja)
        {
            foreach (Projectile projectile in projectiles)
            {
                if (projectile.Active
                    && Math.Abs(projectile.CollisionRectangle.Center.X - ninja.CollisionRectangle.Center.X) < 40
                    && Math.Abs(projectile.CollisionRectangle.Center.Y - ninja.CollisionRectangle.Center.Y) < 40)
                    return true;
            }

            if (mainProjectile.Active
                    && Math.Abs(mainProjectile.CollisionRectangle.Center.X - ninja.CollisionRectangle.Center.X) < 40
                    && Math.Abs(mainProjectile.CollisionRectangle.Center.Y - ninja.CollisionRectangle.Center.Y) < 40)
                return true;

            return false;
        }
    }
}

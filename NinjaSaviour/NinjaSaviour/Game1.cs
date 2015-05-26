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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        const int W_WIDTH = 800;
        const int W_HEIGHT = 600;

        // game objects
        Ninja ninja;
        Boss boss;

        // level fields
        Level level;
        Level levelOne;
        Level levelTwo;
        Level levelThree;
        Level bossLevel;
        int startingX;
        int startingY;

        // state and menu fields
        string state = "menu";
        int selected = 1;
        Texture2D menuTexture;
        Texture2D menuStartTexture;
        Texture2D menuInstructionsTexture;
        Texture2D menuExitTexture;
        Texture2D instructionsTexture;
        Texture2D introTexture;
        Texture2D pauseResumeTexture;
        Texture2D pauseResetTexture;
        Texture2D pauseMenuTexture;
        Texture2D failAgainTexture;
        Texture2D failMenuTexture;
        Texture2D congratsTexture;
        Rectangle menuRectangle = new Rectangle();
        bool isKeyDown = false;

        // audio object
        SoundEffect music;
        SoundEffectInstance instance;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // set resolution
            graphics.PreferredBackBufferWidth = W_WIDTH;
            graphics.PreferredBackBufferHeight = W_HEIGHT;
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

            // create an object for the ninja
            ninja = new Ninja(Content, "ninja2");

            // create an object for the boss
            boss = new Boss(Content, "boss", 640, 270);

            // create objects for the levels
            levelOne = new Level(Content, W_WIDTH, W_HEIGHT, "level1", 1, null);
            levelTwo = new Level(Content, W_WIDTH, W_HEIGHT, "level2", 2, null);
            levelThree = new Level(Content, W_WIDTH, W_HEIGHT, "level3", 3, null);
            bossLevel = new Level(Content, W_WIDTH, W_HEIGHT, "bosslevel3", 4, boss);
            startingX = 40;
            startingY = 500;
            level = levelOne;

            // load menu images
            menuRectangle.X = 0;
            menuRectangle.Y = 0;
            menuRectangle.Width = W_WIDTH;
            menuRectangle.Height = W_HEIGHT;
            menuStartTexture = Content.Load<Texture2D>("menu_start");
            menuInstructionsTexture = Content.Load<Texture2D>("menu_instructions");
            menuExitTexture = Content.Load<Texture2D>("menu_exit");
            instructionsTexture = Content.Load<Texture2D>("instructions");
            introTexture = Content.Load<Texture2D>("intro");
            pauseResumeTexture = Content.Load<Texture2D>("pause_resume");
            pauseResetTexture = Content.Load<Texture2D>("pause_reset");
            pauseMenuTexture = Content.Load<Texture2D>("pause_menu");
            failAgainTexture = Content.Load<Texture2D>("fail_again");
            failMenuTexture = Content.Load<Texture2D>("fail_menu");
            congratsTexture = Content.Load<Texture2D>("congratulations");
            menuTexture = menuStartTexture;

            // load music
            music = Content.Load<SoundEffect>("ninjasaviour");
            instance = music.CreateInstance();
            instance.IsLooped = true;
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
            KeyboardState keyboard = Keyboard.GetState();

            // if the game is in the menu state
            if (state == "menu")
            {
                // update selection
                if ((keyboard.IsKeyDown(Keys.Down) || keyboard.IsKeyDown(Keys.S)) && !isKeyDown)
                {
                    isKeyDown = true;
                    selected++;
                    if (selected == 4)
                        selected = 3;
                }
                else if ((keyboard.IsKeyDown(Keys.Up) || keyboard.IsKeyDown(Keys.W)) && !isKeyDown)
                {
                    isKeyDown = true;
                    selected--;
                    if (selected == 0)
                        selected = 1;
                }
                else if (keyboard.GetPressedKeys().Length == 0)
                    isKeyDown = false;

                // update textures
                if (selected == 1)
                    menuTexture = menuStartTexture;
                else if (selected == 2)
                    menuTexture = menuInstructionsTexture;
                else if (selected == 3)
                    menuTexture = menuExitTexture;
                if (selected == 1 && (keyboard.IsKeyDown(Keys.Space) || keyboard.IsKeyDown(Keys.Enter)) && !isKeyDown)
                {
                    state = "intro";
                    menuTexture = introTexture;
                    isKeyDown = true;
                }
                else if (selected == 2 && (keyboard.IsKeyDown(Keys.Space) || keyboard.IsKeyDown(Keys.Enter)) && !isKeyDown)
                {
                    state = "instructions";
                    menuTexture = instructionsTexture;
                    isKeyDown = true;
                }
                else if (selected == 3 && (keyboard.IsKeyDown(Keys.Space) || keyboard.IsKeyDown(Keys.Enter)) && !isKeyDown)
                    this.Exit();
            }
            // if the game is in the instructions state
            else if (state == "instructions")
            {
                if (keyboard.IsKeyDown(Keys.Space) || keyboard.IsKeyDown(Keys.Enter))
                {
                    if (!isKeyDown)
                    {
                        state = "menu";
                        menuTexture = menuStartTexture;
                        selected = 1;
                        isKeyDown = true;
                    }
                }
                else
                    isKeyDown = false;
            }
            // if the game is in the intro state
            else if (state == "intro")
            {
                if (keyboard.IsKeyDown(Keys.Space) || keyboard.IsKeyDown(Keys.Enter))
                {
                    if (!isKeyDown)
                    {
                        state = "play";
                        menuTexture = menuStartTexture;
                        selected = 1;
                        isKeyDown = true;
                    }
                }
                else
                    isKeyDown = false;
            }
            // if the game is in the play state
            else if (state == "play")
            {
                // update the ninja
                ninja.Update(gameTime, keyboard, level);

                // update the boss if needed
                if (boss.Active)
                    boss.Update(gameTime, level);

                // update the level
                level.Update(gameTime, ninja);
                if (level == levelOne
                    && ninja.Xpos == 700
                    && ninja.Ypos == 500)
                {
                    level = levelTwo;
                    ninja.Xpos = 40;
                    ninja.Ypos = 200;
                    startingX = 40;
                    startingY = 200;
                }
                if (level == levelTwo
                    && ninja.Xpos == 400
                    && ninja.Ypos == 500)
                {
                    level = levelThree;
                    ninja.Xpos = 40;
                    ninja.Ypos = 100;
                    startingX = 40;
                    startingY = 100;
                }
                if (level == levelThree
                    && ninja.Xpos == 700
                    && ninja.Ypos == 100)
                {
                    level = bossLevel;
                    ninja.Xpos = 40;
                    ninja.Ypos = 300;
                    startingX = 40;
                    startingY = 300;
                    boss.Active = true;
                }

                // check for a death
                if (level.checkDeath(ninja, boss))
                {
                    levelOne.resetLevel(Content);
                    levelTwo.resetLevel(Content);
                    levelThree.resetLevel(Content);
                    bossLevel.resetLevel(Content);
                    if (level != bossLevel)
                        boss.Active = false;
                    boss.Life = 3;
                    ninja.Xpos = startingX;
                    ninja.Ypos = startingY;
                    ninja.Shuriken = null;
                    ninja.Offset = 0;
                    state = "fail";
                    menuTexture = failAgainTexture;
                    isKeyDown = true;
                    selected = 1;
                }

                // check for victory
                if (boss.Life == 0 && boss.Active == false)
                {
                    state = "victory";
                    menuTexture = congratsTexture;
                    selected = 1;
                    isKeyDown = true;
                    level = levelOne;
                    levelOne.resetLevel(Content);
                    levelTwo.resetLevel(Content);
                    levelThree.resetLevel(Content);
                    bossLevel.resetLevel(Content);
                    if (level != bossLevel)
                        boss.Active = false;
                    boss.Life = 3;
                    startingX = 40;
                    startingY = 500;
                    ninja.Xpos = startingX;
                    ninja.Ypos = startingY;
                    ninja.Shuriken = null;
                    ninja.Offset = 0;
                }

                // check for a pause
                if (keyboard.IsKeyDown(Keys.Escape))
                {
                    state = "pause";
                    menuTexture = pauseResetTexture;
                    selected = 1;
                }
                else
                    isKeyDown = false;
            }
            // if the game is in the pause state
            if (state == "pause")
            {
                // update selection
                if ((keyboard.IsKeyDown(Keys.Down) || keyboard.IsKeyDown(Keys.S)) && !isKeyDown)
                {
                    isKeyDown = true;
                    selected++;
                    if (selected == 4)
                        selected = 3;
                }
                else if ((keyboard.IsKeyDown(Keys.Up) || keyboard.IsKeyDown(Keys.W)) && !isKeyDown)
                {
                    isKeyDown = true;
                    selected--;
                    if (selected == 0)
                        selected = 1;
                }
                else if (keyboard.GetPressedKeys().Length == 0)
                    isKeyDown = false;

                // update textures
                if (selected == 1)
                    menuTexture = pauseResumeTexture;
                else if (selected == 2)
                    menuTexture = pauseResetTexture;
                else if (selected == 3)
                    menuTexture = pauseMenuTexture;
                if (selected == 1 && (keyboard.IsKeyDown(Keys.Space) || keyboard.IsKeyDown(Keys.Enter)) && !isKeyDown)
                    state = "play";
                else if (selected == 2 && (keyboard.IsKeyDown(Keys.Space) || keyboard.IsKeyDown(Keys.Enter)) && !isKeyDown)
                {
                    levelOne.resetLevel(Content);
                    levelTwo.resetLevel(Content);
                    levelThree.resetLevel(Content);
                    bossLevel.resetLevel(Content);
                    if (level != bossLevel)
                        boss.Active = false;
                    boss.Life = 3;
                    ninja.Xpos = startingX;
                    ninja.Ypos = startingY;
                    ninja.Shuriken = null;
                    ninja.Offset = 0;
                    state = "play";
                }
                else if (selected == 3 && (keyboard.IsKeyDown(Keys.Space) || keyboard.IsKeyDown(Keys.Enter)) && !isKeyDown)
                {
                    menuTexture = menuStartTexture;
                    selected = 1;
                    isKeyDown = true;
                    level = levelOne;
                    levelOne.resetLevel(Content);
                    levelTwo.resetLevel(Content);
                    levelThree.resetLevel(Content);
                    bossLevel.resetLevel(Content);
                    if (level != bossLevel)
                        boss.Active = false;
                    boss.Life = 3;
                    startingX = 40;
                    startingY = 500;
                    ninja.Xpos = startingX;
                    ninja.Ypos = startingY;
                    ninja.Shuriken = null;
                    ninja.Offset = 0;
                    state = "menu";
                }
            }
            // if the game is in the fail state
            if (state == "fail")
            {
                // update selection
                if ((keyboard.IsKeyDown(Keys.Down) || keyboard.IsKeyDown(Keys.S)) && !isKeyDown)
                {
                    isKeyDown = true;
                    selected++;
                    if (selected == 3)
                        selected = 2;
                }
                else if ((keyboard.IsKeyDown(Keys.Up) || keyboard.IsKeyDown(Keys.W)) && !isKeyDown)
                {
                    isKeyDown = true;
                    selected--;
                    if (selected == 0)
                        selected = 1;
                }
                else if (keyboard.GetPressedKeys().Length == 0)
                    isKeyDown = false;

                // update textures
                if (selected == 1)
                    menuTexture = failAgainTexture;
                else if (selected == 2)
                    menuTexture = failMenuTexture;
                if (selected == 1 && (keyboard.IsKeyDown(Keys.Space) || keyboard.IsKeyDown(Keys.Enter)) && !isKeyDown)
                {
                    state = "play";
                    isKeyDown = true;
                }
                else if (selected == 2 && (keyboard.IsKeyDown(Keys.Space) || keyboard.IsKeyDown(Keys.Enter)) && !isKeyDown)
                {
                    level = levelOne;
                    state = "menu";
                    menuTexture = menuStartTexture;
                    isKeyDown = true;
                    selected = 1;
                    startingX = 40;
                    startingY = 500;
                    ninja.Xpos = startingX;
                    ninja.Ypos = startingY;
                }
            }
            // if the game is in the victory state
            else if (state == "victory")
            {
                if (keyboard.IsKeyDown(Keys.Space) || keyboard.IsKeyDown(Keys.Enter))
                {
                    if (!isKeyDown)
                    {
                        state = "menu";
                        menuTexture = menuStartTexture;
                        selected = 1;
                        isKeyDown = true;
                    }
                }
                else
                    isKeyDown = false;
            }

            // play music
            if (!(instance.State == SoundState.Playing))
                instance.Play();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            if (state == "play")
            {
                level.Draw(spriteBatch);
                ninja.Draw(spriteBatch);
                boss.Draw(spriteBatch);
            }
            else
                spriteBatch.Draw(menuTexture, menuRectangle, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

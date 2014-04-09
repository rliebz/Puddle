﻿#region Using Statements
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using TiledSharp;
#endregion

namespace Puddle
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Level level;
        Player player1;
        Controls controls;
        TmxMap map;
        Texture2D background;
        Texture2D introImage;
        List<Texture2D> pauseScreens;
        SoundEffect song;
        SoundEffectInstance instance;
        bool newMapLoad;
        bool paused;
        bool intro;
        float newMapTimer;
        float introScreenTimer;
        int slideCount;
		const float LOAD_SCREEN_TIME = 1.0f;
        const float INTRO_SCREEN_TIME = 3.0f;

        public Game1()
            : base()
        {
            graphics  = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
        }

        protected override void Initialize()
        {
            string initialLevel = String.Format("Content/LevelMenu.tmx");
            map = new TmxMap(initialLevel);

            // Read Level Size From Map
            graphics.PreferredBackBufferWidth = map.Width * map.TileWidth;
            graphics.PreferredBackBufferHeight = map.Height * map.TileHeight;

            paused = false;
            intro = false;
            introImage = Content.Load<Texture2D>("Slide1.png");
            slideCount = 1;

            // Create built-in objects
			player1 = new Player(
				Convert.ToInt32(map.Properties["startX"]), 
				Convert.ToInt32(map.Properties["startY"]),
				32, 
				32
			);
            level = new Level(player1, "menu");
            controls = new Controls();
            newMapLoad = true;
            newMapTimer = LOAD_SCREEN_TIME;
            introScreenTimer = INTRO_SCREEN_TIME;
			player1.newMap = initialLevel;

            song = Content.Load<SoundEffect>("Sounds/InGame.wav");
            instance = song.CreateInstance();
            instance.IsLooped = true;
            if (instance.State == SoundState.Stopped)
                instance.Play();

            pauseScreens = new List<Texture2D>();
            pauseScreens.Add(Content.Load<Texture2D>("pause0.png"));
            pauseScreens.Add(Content.Load<Texture2D>("pause1.png"));
            pauseScreens.Add(Content.Load<Texture2D>("pause2.png"));
            pauseScreens.Add(Content.Load<Texture2D>("pause3.png"));
            base.Initialize();            
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: Load all content in level class
            player1.LoadContent(this.Content);

            foreach (Sprite item in level.items)
                item.LoadContent(this.Content);
			foreach (Enemy enemy in level.enemies)
				enemy.LoadContent(this.Content);      
            
        }

        protected void LoadMap(string name)
        {
            if (name.Equals("Content/LevelMenu.tmx"))
                intro = true;
            else
                intro = false;
            map = new TmxMap(name);
            background = Content.Load<Texture2D>("background.png");

            // Read Level Info From Map
            graphics.PreferredBackBufferWidth = map.Width * map.TileWidth;
            graphics.PreferredBackBufferHeight = map.Height * map.TileHeight;
           
            player1.spriteX = Convert.ToInt32(map.Properties["startX"]);
            player1.spriteY = Convert.ToInt32(map.Properties["startY"]);
			player1.checkpointXPos = player1.spriteX;
			player1.checkpointYPos = player1.spriteY;

			// Create new level object
            level = new Level(player1, name);

			// Create all objects from tmx and place them in level
            foreach (TmxObjectGroup group in map.ObjectGroups)
            {
                foreach (TmxObjectGroup.TmxObject obj in group.Objects)
                {
                    Type t = Type.GetType(obj.Type);
					Console.WriteLine(obj.Name);
                    object item = Activator.CreateInstance(t, obj);
					if (item is Enemy)
						level.enemies.Add((Enemy)item);
					else
                    	level.items.Add((Sprite)item);
                }
            }


            level.items.Sort((x, y) => x.CompareTo(y));


            player1.newMap = "";
			LoadContent();
            newMapLoad = false;
        }

        protected override void UnloadContent()
        {
 
        }

        protected override void Update(GameTime gameTime)
        {
            controls.Update(level);

            if (controls.onPress(Keys.Escape, Buttons.Back))
                Exit();

            if (controls.onPress(Keys.Enter, Buttons.Start))
                paused = !paused;

            player1.Update(controls, level, this.Content, gameTime);

            if (!player1.newMap.Equals(""))
            {
                newMapLoad = true;
            }
            level.Update(this.Content);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            spriteBatch.Begin();



            if (newMapLoad)
            {
                GraphicsDevice.Clear(Color.Black);
                string[] fileName = player1.newMap.Split('.');
                string levelNumber = fileName[0].Remove(0, 13);
                string levelName = String.Format("Level {0}", levelNumber);
                TextField message = new TextField(
                    levelName, 
                    new Vector2(
						(graphics.PreferredBackBufferWidth / 2) - 50, 
						(graphics.PreferredBackBufferHeight / 2) - 50
					),
                    Color.White
                    );
                message.loadContent(Content);
                message.draw(spriteBatch);

                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                newMapTimer -= elapsed;
                if (newMapTimer < 0)
                {
                    LoadMap(player1.newMap);//Timer expired, execute action
                    newMapTimer = LOAD_SCREEN_TIME;   //Reset Timer
                }
            }
            else
            {
				// Draw background
                spriteBatch.Draw(
                    background,
                    new Rectangle(
                        0, 0,
                        graphics.PreferredBackBufferWidth,
                        graphics.PreferredBackBufferHeight
                    ),
                    Color.White
                );

                if (intro)
                {
                    float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                    introScreenTimer -= elapsed;
                    spriteBatch.Draw(
                        introImage,
                        new Rectangle(
                        0, 0,
                        720,
                        540
                        ),
                        Color.White
                    );
                    if(introScreenTimer < 0 && slideCount != 5)
                    {
                        slideCount += 1;
                        introImage = Content.Load<Texture2D>(String.Format("Slide{0}.png", slideCount));
                        introScreenTimer = INTRO_SCREEN_TIME;
                    }
                }
                
                // Draw contents of the level
                level.Draw(spriteBatch);

                if (paused)
                {
                    spriteBatch.Draw(
                        pauseScreens[player1.numPowers],
                        new Rectangle(
                        0, graphics.PreferredBackBufferHeight/2 - 270,
                        720,
                        540
                        ),
                        Color.White * 0.7f
                    );
                }


				if (level.message != "")
				{
					TextField message = new TextField(
						level.message, 
						new Vector2(16, (graphics.PreferredBackBufferHeight ) - 28),
						Color.White
					);

					message.loadContent(Content);
					message.draw(spriteBatch);
				}
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

    }
}

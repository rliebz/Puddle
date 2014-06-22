#region Using Statements
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
		Controls pauseControls;
        TmxMap map;
        Texture2D background;
        Texture2D introImage;
        List<Texture2D> pauseScreens;
        SoundEffect song, bossSong, menuSong;
        SoundEffectInstance instance, bossInstance, menuInstance;
        Level levelSelect;
        bool loadingMap;
        bool paused;
        bool intro;
        string previousMap;
        float newMapTimer;
        float introScreenTimer;
        int slideCount;
		const string LEVEL_PATH = "Content/Levels/Level{0}.tmx";
		const float LOAD_SCREEN_TIME = 1.5f;
        const float INTRO_SCREEN_TIME = 3.0f;

        public Game1()
            : base()
        {
            graphics  = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";           
        }

        protected override void Initialize()
        {
			string initialLevel = "Menu";
			map = new TmxMap(String.Format(LEVEL_PATH, initialLevel));

            // Read Level Size From Map
            graphics.PreferredBackBufferWidth = map.Width * map.TileWidth;
            graphics.PreferredBackBufferHeight = map.Height * map.TileHeight;

            paused = false;
            intro = false;
			introImage = Content.Load<Texture2D>("Slides/Slide1.png");
            slideCount = 1;
            previousMap = "";

            // Create built-in objects
			player1 = new Player(
				Convert.ToInt32(map.Properties["startX"]), 
				Convert.ToInt32(map.Properties["startY"])
			);
			level = new Level(player1, "menu", this.Content);
            levelSelect = null;
            controls = new Controls();
			pauseControls = new Controls();
            loadingMap = true;
            newMapTimer = LOAD_SCREEN_TIME;
            introScreenTimer = INTRO_SCREEN_TIME;
			player1.newMap = initialLevel;

            song = Content.Load<SoundEffect>("Sounds/InGame.wav");
            instance = song.CreateInstance();
            instance.IsLooped = true;

            bossSong = Content.Load<SoundEffect>("Sounds/Boss.wav");
            bossInstance = bossSong.CreateInstance();
            bossInstance.IsLooped = true;

            menuSong = Content.Load<SoundEffect>("Sounds/Menu.wav");
            menuInstance = menuSong.CreateInstance();
            menuInstance.IsLooped = true;
            menuInstance.Play();

            pauseScreens = new List<Texture2D>();
			for (int i=0; i < 4; i++)
			{
				pauseScreens.Add(Content.Load<Texture2D>(String.Format("Slides/pause{0}.png", i)));
			}
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
			player1.ResetFields(true);

			// Save state of levelSelect after exiting
			if (previousMap.Equals("Select"))
				levelSelect = new Level(level);
						
			// Load map and background
			map = new TmxMap(String.Format(LEVEL_PATH, name));
			graphics.PreferredBackBufferWidth = map.Width * map.TileWidth;
			graphics.PreferredBackBufferHeight = map.Height * map.TileHeight;
			background = Content.Load<Texture2D>("background.png");

			// Decide if we're on the intro
			intro = name.Equals("Menu");

			// Choose music
			if (intro || name.Equals("Select"))
            {
                instance.Stop();
                bossInstance.Stop();
                menuInstance.Play();
            }
			else if (name.Equals("Boss"))
            {
                instance.Stop();
                menuInstance.Stop();
                bossInstance.Play();
            }
            else
            {
                bossInstance.Stop();
                menuInstance.Stop();
                instance.Play();
            }


			// Create new level object
			if (name.Equals("Select") && levelSelect != null)
            {
                level = levelSelect;
            }
            else
            {
				level = new Level(player1, name, this.Content);

                // Create all objects from tmx and place them in level
                foreach (TmxObjectGroup group in map.ObjectGroups)
                {
                    foreach (TmxObjectGroup.TmxObject obj in group.Objects)
                    {
                        Type t = Type.GetType(obj.Type);
                        object item = Activator.CreateInstance(t, obj);
                        if (item is Enemy)
                            level.enemies.Add((Enemy)item);
                        else
                            level.items.Add((Sprite)item);
                    }
                }
            }

			if (name.Equals("Select"))
            {
				// TODO: More adaptable start position handler
				String startPosSelect = "";
				if (previousMap.Equals("Boss"))
				{
					startPosSelect = "Boss";
				}
				else if (Char.IsNumber(previousMap[0]))
				{
					startPosSelect = previousMap[0].ToString();
				}
                else
                {
					startPosSelect = "0";
                }
				player1.worldPowerUp = null;
                player1.spriteX = Convert.ToInt32(map.Properties[String.Format("startX{0}", startPosSelect)]);
                player1.spriteY = Convert.ToInt32(map.Properties[String.Format("startY{0}", startPosSelect)]);
            }
            else
            {
                player1.spriteX = Convert.ToInt32(map.Properties["startX"]);
                player1.spriteY = Convert.ToInt32(map.Properties["startY"]);
            }

            // Reset player fields
            player1.checkpointXPos = player1.spriteX;
            player1.checkpointYPos = player1.spriteY;

            level.items.Sort((x, y) => x.CompareTo(y));

            previousMap = String.Copy(player1.newMap);
			player1.newMap = null;

			// Load new content
			foreach (Sprite item in level.items)
				item.LoadContent(this.Content);
			foreach (Enemy enemy in level.enemies)
				enemy.LoadContent(this.Content);
        }

        protected override void UnloadContent()
		{ }

        protected override void Update(GameTime gameTime)
        {
			if (player1.newMap != null)
				loadingMap = true;

			// Do not update level while loading new map
			if (loadingMap){
				float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
				newMapTimer -= elapsed;
				if (newMapTimer < 0)
				{
					LoadMap(player1.newMap);
					newMapTimer = LOAD_SCREEN_TIME;
					loadingMap = false;
				}
				return;
			}

			pauseControls.Update(level);

			// Return to level select
			if (pauseControls.onPress(Keys.Escape, Buttons.Back) && paused)
			{
				// Can only return from inside a world
				if (level.name.Equals("Select") || level.name.Equals("Menu"))
					return;

				// Reset powerup from that world to prevent content skipping
				if (player1.worldPowerUp != null)
					player1.powerup[player1.worldPowerUp] = false;

				// Go to new level
				player1.newMap = "Select";
				paused = false;
			}

			if (pauseControls.onPress(Keys.Enter, Buttons.Start))
				paused = !paused;

			if (paused)
				return;
				
            controls.Update(level);

            player1.Update(controls, level, gameTime);

            level.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            spriteBatch.Begin();

            if (loadingMap)
            {
				string levelDisplay = player1.newMap.Equals("Win") ? "Congratulations!" :
					String.Format("Level {0}", player1.newMap);

				GraphicsDevice.Clear(Color.Black);
                TextField message = new TextField(
					levelDisplay, 
                    new Vector2(
						(graphics.PreferredBackBufferWidth / 2) - 50, 
						(graphics.PreferredBackBufferHeight / 2) - 50
					),
                    Color.White
                );
                message.loadContent(Content);
                message.draw(spriteBatch);

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

				// Draw intro slides
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
						introImage = Content.Load<Texture2D>(String.Format("Slides/Slide{0}.png", slideCount));
                        introScreenTimer = INTRO_SCREEN_TIME;
                    }
                }
                
                // Draw contents of the level
                level.Draw(spriteBatch);

				if (paused && !intro)
                {
                    spriteBatch.Draw(

						pauseScreens[player1.numPowers],
                        new Rectangle(
                        0, graphics.PreferredBackBufferHeight/2 - 270,
                        720,
                        540
                        ),
						Color.White
                    );
                }

				// Display picked up messages
				if (!String.IsNullOrEmpty(level.message))
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

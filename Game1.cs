#region Using Statements
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using TiledSharp;
#endregion

namespace Puddle
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        Camera defaultCamera;
        Camera playerCamera;
        SpriteBatch spriteBatch;
        Level level;
        Player player1;
		Controls controls;
		Controls pauseControls;
        Menu menu;
        TmxMap map;
        Texture2D background;
        Texture2D introImage;
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
        int gameScale = 2;
		const string LEVEL_PATH = "Levels/Level{0}.tmx";
		const float LOAD_SCREEN_TIME = 1.5f;
        const float INTRO_SCREEN_TIME = 3.0f;
        const int GAME_BASE_WIDTH = 15;
        const int GAME_BASE_HEIGHT = 10;

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

            // Handle window sizing
            this.Window.AllowUserResizing = true;
            this.Window.ClientSizeChanged += new EventHandler<EventArgs>(WindowSizeChangeEvent);
            WindowSizeChange(
                GAME_BASE_WIDTH * Sprite.SIZE * gameScale,
                GAME_BASE_HEIGHT * Sprite.SIZE * gameScale
            );

            paused = false;
            intro = false;
            slideCount = 1;
            previousMap = "";

            // Create built-in objects
			player1 = new Player(
				Convert.ToInt32(map.Properties["startX"]), 
				Convert.ToInt32(map.Properties["startY"])
            );
            defaultCamera = new Camera(new Vector2(0, 0));
            playerCamera = new Camera(PlayerCameraCoordinates(player1, graphics, gameScale));
			level = new Level(player1, "menu", this.Content);
            levelSelect = null;
            controls = new Controls();
			pauseControls = new Controls();
            menu = new Menu();
            loadingMap = true;
            newMapTimer = LOAD_SCREEN_TIME;
            introScreenTimer = INTRO_SCREEN_TIME;
			player1.newMap = initialLevel;

            base.Initialize();            
        }

        protected void WindowSizeChange(int width, int height)
        {
            graphics.PreferredBackBufferWidth = Math.Max(
                width, Sprite.SIZE * GAME_BASE_WIDTH
            );
            graphics.PreferredBackBufferHeight = Math.Max(
                height, Sprite.SIZE * GAME_BASE_HEIGHT
            );
            graphics.ApplyChanges();
        }

        protected void WindowSizeChangeEvent(object sender, EventArgs e)
        {
            gameScale = Math.Min(
                Window.ClientBounds.Width / (Sprite.SIZE * GAME_BASE_WIDTH),
                Window.ClientBounds.Height / (Sprite.SIZE * GAME_BASE_HEIGHT)
            );

            WindowSizeChange(
                Window.ClientBounds.Width,
                Window.ClientBounds.Height
            );
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            introImage = Content.Load<Texture2D>("Slides/Slide1");

            // Sound Content
            // TODO: Should sound be played here?
            song = Content.Load<SoundEffect>("Sounds/InGame");
            instance = song.CreateInstance();
            instance.IsLooped = true;

            bossSong = Content.Load<SoundEffect>("Sounds/Boss");
            bossInstance = bossSong.CreateInstance();
            bossInstance.IsLooped = true;

            menuSong = Content.Load<SoundEffect>("Sounds/Menu");
            menuInstance = menuSong.CreateInstance();
            menuInstance.IsLooped = true;

            // TODO: Load all content in level class
            player1.LoadContent(this.Content);
            menu.LoadContent(this.Content);

            foreach (Sprite item in level.items)
                item.LoadContent(this.Content);
			foreach (Enemy enemy in level.enemies)
				enemy.LoadContent(this.Content);

            Sprite.font = Content.Load<SpriteFont>("Fonts/Puddle");
            
        }

        protected void LoadMap(string name)
        {
			player1.ResetFields(true);

			// Save state of levelSelect after exiting
			if (previousMap.Equals("Select"))
				levelSelect = new Level(level);
						
			// Load map and background
			map = new TmxMap(String.Format(LEVEL_PATH, name));
			background = Content.Load<Texture2D>("Textures/background");

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
            playerCamera.JumpToPosition(PlayerCameraCoordinates(player1, graphics, gameScale));
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
		{ 
			Content.Unload();
		}

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

            if (pauseControls.onPress(controls.Start))
            {
                paused = !paused;
                menu.LoadConfiguration();
            }

            if (paused)
            {
                if (pauseControls.onPress(controls.Down))
                { menu.cursor++; }

                if (pauseControls.onPress(controls.Up))
                { menu.cursor--; }

                if (pauseControls.onPress(controls.Confirm))
                { paused = menu.ExecuteAction(this, player1, level); }

                if (pauseControls.onPress(controls.Back))
                { menu.LoadConfiguration(); }

                return;
            }

            playerCamera.Update();
            controls.Update(level);
            player1.Update(controls, level, gameTime);
            level.Update();

            base.Update(gameTime);
        }

        private void BeginSpriteBatch(Camera camera)
        {

            Matrix scaleMatrix = Matrix.CreateScale(gameScale);
            Matrix originMatrix =  Matrix.CreateTranslation(new Vector3(camera.X, camera.Y, 0));
            Matrix transformMatrix = scaleMatrix * originMatrix;

            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                null, null, null,
                transformMatrix
            );
        }


        private Vector2 PlayerCameraCoordinates(Player player, GraphicsDeviceManager graphics, int gameScale)
        {
            Vector2 output = new Vector2();

            output.X = graphics.PreferredBackBufferWidth / 2 - player.spriteX * gameScale;
            int puddleYOffset = player.puddled ? 96 : 0;
            output.Y = graphics.PreferredBackBufferHeight * 3 / 4 - (player.spriteY + puddleYOffset) * gameScale;

            return output;
        }

        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.Black);

            playerCamera.GoToPosition(PlayerCameraCoordinates(player1, graphics, gameScale));

            // Draw
            if (loadingMap)
            {
                // Draw Level Loading Screen
                BeginSpriteBatch(defaultCamera);

				string levelDisplay = player1.newMap.Equals("Win") ? "Congratulations!" :
					String.Format("Level {0}", player1.newMap);
                spriteBatch.DrawString(
                    Sprite.font,
                    levelDisplay,
                    new Vector2(
                        graphics.PreferredBackBufferWidth / 2 / gameScale,
                        graphics.PreferredBackBufferHeight / 2 / gameScale
                    ),
                    Color.White,
                    0f,
                    Sprite.font.MeasureString(levelDisplay) * 0.5f,
                    1f,
                    SpriteEffects.None,
                    0
                );

                spriteBatch.End();
            }
            else
            {
                BeginSpriteBatch(playerCamera);

				// Draw background
                spriteBatch.Draw(
                    background,
                    new Rectangle(
                        0, 0,
                        map.Width * map.TileWidth,
                        map.Height * map.TileHeight
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
                        new Rectangle(0, 0, 21 * Sprite.SIZE, 17 * Sprite.SIZE),
                        Color.White
                    );
                    if(introScreenTimer < 0 && slideCount != 5)
                    {
                        slideCount += 1;
						introImage = Content.Load<Texture2D>(String.Format("Slides/Slide{0}", slideCount));
                        introScreenTimer = INTRO_SCREEN_TIME;
                    }
                }
                
                // Draw contents of the level
                level.Draw(spriteBatch);

                spriteBatch.End();

                // Draw UI elements
                BeginSpriteBatch(defaultCamera);
                level.DrawUI(spriteBatch, graphics, gameScale);

                // Draw pause screen
				if (paused)
                {
                    menu.DrawUI(spriteBatch, graphics, gameScale);
                }

				// Draw picked up messages
				if (!String.IsNullOrEmpty(level.message))
				{
                    spriteBatch.DrawString(
                        Sprite.font,
                        level.message,
                        new Vector2(
                            graphics.PreferredBackBufferWidth / 2 / gameScale, 
                            graphics.PreferredBackBufferHeight / gameScale - Sprite.SIZE
                        ),
                        Color.White,
                        0f,
                        Sprite.font.MeasureString(level.message) * 0.5f,
                        0.75f * Window.ClientBounds.Width / Sprite.font.MeasureString(level.message).X / gameScale,
                        SpriteEffects.None,
                        0
                    );
                }

                spriteBatch.End();
            }

            base.Draw(gameTime);
        }

    }
}

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
using TiledSharp;
#endregion

namespace Puddle
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Physics physics;
        Player player1;
        Controls controls;
        TmxMap map;
        Texture2D background;
        bool newMapLoad;
        float newMapTimer;
        const float LOAD_SCREEN_TIME = 3.0f;

        public Game1()
            : base()
        {
            graphics  = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {

            // Create built-in objects
            player1 = new Player(50, 250, 32, 32);
            physics = new Physics(player1);
            controls = new Controls();
            newMapLoad = true;
            newMapTimer = LOAD_SCREEN_TIME;
            player1.newMap = "Content/Level1.tmx";

            base.Initialize();            
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: Load all content in level class
            player1.LoadContent(this.Content);

            foreach (Block b in physics.blocks)
                b.LoadContent(this.Content);
            foreach (Sprite item in physics.items)
                item.LoadContent(this.Content);
        }

        protected void LoadMap(string name)
        {
            map = new TmxMap(name);
            background = Content.Load<Texture2D>("background.png");

            // Read Level Info From Map
            graphics.PreferredBackBufferWidth = map.Width * map.TileWidth;
            graphics.PreferredBackBufferHeight = map.Height * map.TileHeight;
           
            player1.spriteX = Convert.ToInt32(map.Properties["startX"]);
            player1.spriteY = Convert.ToInt32(map.Properties["startY"]);

            // Create map objects
            physics = new Physics(player1);

            foreach (TmxObjectGroup group in map.ObjectGroups)
            {
                foreach (TmxObjectGroup.TmxObject obj in group.Objects)
                {
                    Type t = Type.GetType(obj.Type);
                    object item = Activator.CreateInstance(t, obj);
                    if (item is Block)
                        physics.blocks.Add((Block)item);
                    else
                        physics.items.Add((Sprite)item);
                }
            }
            player1.newMap = "";
            LoadContent();
            newMapLoad = false;
            base.Initialize();
        }

        protected override void UnloadContent()
        {
 
        }

        protected override void Update(GameTime gameTime)
        {
            controls.Update();

            if (controls.onPress(Keys.Escape, Buttons.Back))
                Exit();

            // TODO: Level.Update() should cover all objects in that level
            player1.Update(controls, physics, this.Content, gameTime);
            if (!player1.newMap.Equals(""))
            {
                newMapLoad = true;
            }
            physics.Update(this.Content);

            foreach (Block b in physics.blocks)
                b.Update(physics);

            foreach (Enemy e in physics.enemies)
                e.Update(physics);

            foreach (Sprite s in physics.items)
            {
                s.Update(physics);
                s.Update(physics, this.Content);
            }
            
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
                    new Vector2((graphics.PreferredBackBufferWidth / 2) - 50, (graphics.PreferredBackBufferHeight / 2) - 50),
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
                spriteBatch.Draw(
                    background,
                    new Rectangle(
                        0, 0,
                        graphics.PreferredBackBufferWidth,
                        graphics.PreferredBackBufferHeight
                    ),
                    Color.White
                );

                // TODO: Level.Draw() should cover all this
                foreach (Enemy e in physics.enemies)
                    e.Draw(spriteBatch);
                foreach (Shot s in physics.shots)
                    s.Draw(spriteBatch);
                foreach (Block b in physics.blocks)
                    b.Draw(spriteBatch);
                foreach (Sprite item in physics.items)
                    item.Draw(spriteBatch);
                foreach (Fireball f in physics.fireballs)
                    f.Draw(spriteBatch);
                player1.Draw(spriteBatch);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

    }
}

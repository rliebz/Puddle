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

        public Game1()
            : base()
        {
            graphics  = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // Read Level Info From Map
            map = new TmxMap("Content/Demo.tmx");
            graphics.PreferredBackBufferWidth = map.Width * map.TileWidth;
            graphics.PreferredBackBufferHeight = map.Height * map.TileHeight;
            background = Content.Load<Texture2D>("background.png");

            // Create built-in objects
            player1 = new Player(50, 250, 32, 32);
            physics = new Physics(player1);
            controls = new Controls();

            // Create map objects
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

        protected override void UnloadContent()
        { }

        protected override void Update(GameTime gameTime)
        {
            controls.Update();

            if (controls.onPress(Keys.Escape, Buttons.Back))
                Exit();

            // TODO: Level.Update() should cover all objects in that level
            player1.Update(controls, physics, this.Content, gameTime);
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

            spriteBatch.End();
            base.Draw(gameTime);
        }

    }
}

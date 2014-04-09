using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using TiledSharp;

namespace Puddle
{
    class Geyser : Sprite
    {

        
        public int seed;

        //TODO: add in function passing for individual button actions
        public Geyser(TmxObjectGroup.TmxObject obj) :
            base(obj.X, obj.Y - 12, 64, 128)
        {
            this.imageFile = "geyser.png";
       
            name = obj.Name;
            // isSolid = true;


            frameWidth = 64;
            frameHeight = 128;
            
            collisionWidth = 32;
            collisionHeight = 112;

            Random rnd = new Random();
            seed = rnd.Next(64); // For animation

        }


		public void checkCollisions(Level level)
        {
            if (Intersects(level.player))
           {
				// Prevent strange collisions
				level.player.y_vel = -4;
				foreach (Sprite s in level.items)
				{
					if (s.isSolid && level.player.Intersects(s) && 
						level.player.topWall - Convert.ToInt32(level.player.y_vel) > s.bottomWall)
					{
						level.player.y_vel = 0;
					}
				}
				level.player.grounded = false;
               	if (level.player.hydration < level.player.maxHydration - 2)
                    level.player.hydration += 2;
            }

            foreach (Enemy e in level.enemies)
            {
                if (Intersects(e))
                {
                    e.y_vel = -4;
                    e.grounded = false;
                }
            }
        }
        
    

        public void Animate(Level level)
        {
            frameIndex = ((level.count + seed) / 12) % 2 * 64;
        }

        public override void Update(Level level)
        {
			checkCollisions(level);
            Animate(level);
        }



    }
}

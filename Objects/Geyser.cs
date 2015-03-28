using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using TiledSharp;

namespace Puddle
{
    class Geyser : Sprite
    {
        
        public int seed;
		public int speed;

        //TODO: add in function passing for individual button actions
        public Geyser(TmxObjectGroup.TmxObject obj) :
		base(obj.X, obj.Y - spriteSize / 2)
        {
            this.imageFile = "geyser";
       
            name = obj.Name;
			speed = -5;

            frameHeight = 4 * spriteSize;
            baseHeight = 4;
            
            baseCollisionHeight = 3.5;

            Random rnd = new Random();
            seed = rnd.Next(64); // For animation

        }


		public void checkCollisions(Level level)
        {
            if (Intersects(level.player))
           {
				level.player.yVel = speed;
				level.player.grounded = false;

               	if (level.player.hydration < level.player.maxHydration - 2)
                    level.player.hydration += 2;
            }

            foreach (Enemy e in level.enemies)
            {
                if (Intersects(e))
                {
					e.yVel = speed;
                    e.grounded = false;
                }
            }
        }

        public void Animate(Level level)
        {
            frameIndexX = ((level.count + seed) / 4) % 6 * frameWidth;
        }

        public override void Update(Level level)
        {
			checkCollisions(level);
            Animate(level);
        }



    }
}

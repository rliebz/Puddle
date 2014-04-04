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



        public void checkCollision(Level level)
        {
            if (Intersects(level.player))
           {
                    level.player.y_vel = -4;
                    //level.player.spriteY -= 1;
                    level.player.grounded = false;
                    if (level.player.hydration < level.player.maxHydration - 2)
                    {
                        level.player.hydration += 2;
                    }
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
            checkCollision(level);
            Animate(level);
        }



    }
}

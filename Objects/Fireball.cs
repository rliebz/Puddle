using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

namespace Puddle
{
    class Fireball : Sprite
    {
        public string dir;
        public int speed;
        public int x_vel;
        public int seed;

        public Fireball(Cannon c, string dir = "none")
            : base(c.spriteX + 8, c.spriteY - 16, 32, 32)
        {
            this.imageFile = "fireball.png";
            this.dir = dir;
            speed = 6;
            sizeX = 28;
            sizeY = 28;
            x_vel = 2;
            Random rnd = new Random();
            seed = rnd.Next(20);
        }

        public void Update(Physics physics)
        {
            Move();
            
            CheckCollisions(physics);
            Animate(physics);
        }

        public void Animate(Physics physics)
        {
            frameIndex = ((physics.count + seed) / 12) % 2 * 32;
        }

        public void CheckCollisions(Physics physics)
        {
            // check collisions with enemies
            if (Intersects(physics.player) && !physics.player.puddled)
            {
                this.destroyed = true;
                physics.player.Death();
            }


            // check collisions with blocks
            foreach (Block b in physics.blocks)
            {
                if (Intersects(b))
                    this.destroyed = true;
            }
        }

        public void Move()
        {
            if (dir == "down")
                spriteY += speed / 2;
            else if (dir == "up")
            {
                spriteY -= speed;
            }
            else
                spriteX += x_vel;

        }

    }
}

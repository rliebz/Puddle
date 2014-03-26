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
        public int seed;

        public Fireball(Cannon c, string dir = "none")
            : base(c.spriteX + 16, c.spriteY - 16, 32, 32)
        {
            this.imageFile = "fireball.png";
            
            this.dir = dir;
            if (this.dir == "left")
                faceLeft = true;

            speed = 2;
            collisionWidth = 28;
            collisionHeight = 28;

            Random rnd = new Random();
            seed = rnd.Next(20); // For animation
        }

        public override void Update(Physics physics)
        {
            Move();

            CheckCollisions(physics);

            Animate(physics);
        }

        public void Move()
        {
            if (dir == "down")
                spriteY += speed;
            else if (dir == "up")
                spriteY -= speed;
            else if (dir == "left")
                spriteX -= speed;
            else
                spriteX += speed;
        }

        public void CheckCollisions(Physics physics)
        {
            // check collisions with player
            if (Intersects(physics.player) && !physics.player.invulnerable)
            {
                destroyed = true;
                physics.player.Death();
            }

            // check collisions with blocks
            foreach (Block b in physics.blocks)
            {
                if (Intersects(b))
                    destroyed = true;
            }
        }

        public void Animate(Physics physics)
        {
            frameIndex = ((physics.count + seed) / 12) % 2 * 32;
        }
    }
}

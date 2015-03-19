using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace Puddle
{
    class Fireball : Sprite
    {
        public string dir;
        public int speed;
        public int seed;

		public Fireball(int x, int y, string dir = "right")
			: base(x, y, 32, 32)
        {
            collisionWidth = 30;
            collisionHeight = 28;
            this.imageFile = "fireball.png";
            
            this.dir = dir;
            if (this.dir == "up")
            {
                rotationAngle = MathHelper.PiOver2 * 3;
                collisionWidth = 30;
                collisionHeight = 32;
            }
            else if (this.dir == "down")
            {
                rotationAngle = MathHelper.PiOver2;
                collisionWidth = 30;
                collisionHeight = 32;
            }
            else if (this.dir == "left")
                faceLeft = true;

            speed = 2;

            // Fix initial position
            if (dir == "down")
                spriteY -= speed;
            else if (dir == "up")
                spriteY += speed;
            else if (dir == "left")
                spriteX += speed;
            else
                spriteX -= speed;

			depth = 1;

            Random rnd = new Random();
            seed = rnd.Next(20); // For animation
        }

        public override void Update(Level level)
        {
            Move();

            CheckCollisions(level);

            Animate(level);
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

        public void CheckCollisions(Level level)
        {
            // check collisions with player
            if (Intersects(level.player) && !level.player.invulnerable)
            {
                level.player.Death(level);
            }

            // check collisions with blocks
			foreach (Sprite s in level.items)
            {
				if (s.isSolid && Intersects(s))
                    destroyed = true;
            }

			if (offScreen)
				destroyed = true;
        }

        public void Animate(Level level)
        {
            frameIndexX = ((level.count + seed) / 12) % 2 * 32;
        }
    }
}

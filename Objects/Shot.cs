using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace Puddle
{
    class Shot : Sprite
    {
        public string dir;
        public int speed;
        public int xVel;
        public int damage;

        public Shot(Player p, string dir = "none")
            : base(p.spriteX - spriteSize / 2, p.spriteY - spriteSize / 2)
        {
            imageFile = "bubble.png";
			image = p.images["bubble"];
            
			this.dir = dir;
            baseCollisionWidth = 0.375;
            baseCollisionHeight = 0.375;
            speed = 6;
            damage = 1;
            xVel = Convert.ToInt32(p.xVel * .5 + speed * (p.faceLeft ? -1 : 1));
        }

        public override void Update(Level level)
        {
            Move();

            CheckCollisions(level);
        }

        public virtual void CheckCollisions(Level level)
        {
            // Check collisions with enemies
            foreach (Enemy e in level.enemies)
            {
                if (Intersects(e))
                {
                    destroyed = true;
					// TODO: Move enemy logic to enemy class
                    e.health -= damage;
					e.spriteColor = Color.Red;
					e.hurtPoint = level.count;
                }
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

        public void Move()
        {
            if (dir == "down")
                spriteY += speed / 2;
            else if (dir == "up")
            {
                spriteY -= speed;
            }
            else
                spriteX += xVel;

        }
    }
}

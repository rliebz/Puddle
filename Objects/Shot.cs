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
    class Shot : Sprite
    {
        public string dir;
        public int speed;
        public int x_vel;
        public int damage;

        public Shot(Player p, string dir = "none")
            : base(p.spriteX - 16, p.spriteY - 16, 24, 24)
        {
            this.imageFile = "bubble.png";
            
            this.dir = dir;
            collisionWidth = 8;
            collisionHeight = 8;
            speed = 6;
            damage = 1;
            x_vel = Convert.ToInt32(p.x_vel * .5 + speed * (p.faceLeft ? -1 : 1));
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
                    e.health -= damage;
					e.spriteColor = Color.Red;
					e.hurt_point = level.count;
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
                spriteX += x_vel;

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

namespace Puddle
{
    class Egg : Sprite
    {
        public int speed;
        public int health;
        public int hurt_point;

        public Egg(int x, int y)
            : base(x, y, 32, 32)
        {
            collisionWidth = 19;
            collisionHeight = 24;
            this.imageFile = "Egg.png";
            speed = 4;
            health = 3;
        }

        public override void Update(Level level)
        {
            Move();

            CheckCollisions(level);

            Animate(level);
        }

        public void Move()
        {
            spriteY += speed;
        }

        public void CheckCollisions(Level level)
        {
            // check collisions with player
            if (Intersects(level.player) && !level.player.invulnerable)
            {
                level.player.Death(level);
                destroyed = true;
            }

            // check collisions with blocks
            foreach (Sprite s in level.items)
            {
                if (s.isSolid && Intersects(s))
                destroyed = true;
            }

            foreach (Sprite s in level.projectiles)
            {
                if (s.isShot && Intersects(s))
                {
                    s.destroyed = true;
                    health--;
                    hurt_point = level.count;
                    spriteColor = Color.Red;
                    if (health == 0)
                    {
                        destroyed = true;
                    }
                }
            }

            if (offScreen)
                destroyed = true;
        }

        public void Animate(Level level)
        {
            if (level.count - hurt_point > 2 && spriteColor == Color.Red)
            {
                spriteColor = Color.White;
            }
        }
    }
}

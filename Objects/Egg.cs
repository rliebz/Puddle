using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Puddle
{
    class Egg : Sprite
    {
        public int speed;

        public Egg(int x, int y)
            : base(x, y, 32, 32)
        {
            collisionWidth = 30;
            collisionHeight = 28;
            this.imageFile = "Egg.png";
            speed = 4;
        }

        public override void Update(Level level)
        {
            Move();

            CheckCollisions(level);
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
    }
}

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
    class Enemy : Sprite
    {
        public bool left;
        public int x_vel;
        public int y_vel;
        public int frameIndex;
        public int seed;
        public int health;
        Random rnd;

        public Enemy(int x, int y)
            : base(x, y, 50, 50)
        {
            this.imageFile = "rat.png";
            left = true;
            x_vel = -3;
            y_vel = 0;
            health = 3;

            // Sprite business
            rnd = new Random();
            seed = rnd.Next(0, 3);
            frameIndex = 0;
        }

        public void Update(Controls controls, Physics physics, ContentManager content)
        {
            // Move
            Move(physics);

            // Update sprite
            frameIndex = ((physics.count + seed) / 8 % 4) * 32;

            // Be killed if necessary
            destroyed = (health <= 0);
        }

        public void Move(Physics physics)
        {

            // Change directions maybe
            if (spriteX > 750)
            {
                x_vel = -3;
                left = true;
            }
            else if (spriteX < 50)
            {
                x_vel = 3;
                left = false;
            }
            else if (physics.count % (30 + rnd.Next(1, 600)) == 0)
            {
                x_vel = -x_vel;
                left = !left;
            }

            // Move horizontally
            spriteX += x_vel;

            // Fall (with grace!)
            if (spriteY < physics.ground)
            {
                spriteY += y_vel;
                y_vel += physics.gravity;
            }
            else
            {
                spriteY = physics.ground;
                y_vel = 0;

                // Jump maybe
                if (rnd.NextDouble() < .01)
                {
                    spriteY--;
                    y_vel = -15;
                }
            }
        }

        public new void Draw(SpriteBatch sb)
        {
            sb.Draw(
                image,
                new Rectangle(spriteX, spriteY, spriteWidth, spriteHeight),
                new Rectangle(frameIndex, 0, 32, 32),
                Color.White,
                0,
                new Vector2(16, 16),
                left ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0
            );
        }
    }

}

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
	abstract class Enemy : Sprite
    {
		public double x_vel;
		public double y_vel;
        public int seed;
		public int speed;
        public int health;
		public bool grounded;
		protected Random rnd;

        public Enemy(int x, int y)
            : base(x, y, 32, 32)
        {
            this.imageFile = "rat.png";
			speed = 2;
			x_vel = speed;
            y_vel = 0;
            health = 3;
			grounded = false;

            // Sprite business
            rnd = new Random();
            seed = rnd.Next(0, 3);
        }

        public override void Update(Physics physics)
        {
            // Move
            Move(physics);

			// Fall
			Fall(physics);

			// Maybe jump
			Jump(physics);

            // Animate sprite
            Animate(physics);

            // Be killed if necessary
            destroyed = (health <= 0);
        }

        public void Move(Physics physics)
        {

			// Move horizontally
			spriteX += Convert.ToInt32(x_vel);

			foreach (Sprite s in physics.items)
			{
				if (s.isSolid && Intersects(s))
				{
					// Collide with block on side, turn around
					if (bottomWall > s.topWall &&
						(rightWall - Convert.ToInt32(x_vel) < s.leftWall && x_vel > 0 ||
							leftWall - Convert.ToInt32(x_vel) > s.rightWall && x_vel < 0))
					{
						x_vel = - x_vel;
						faceLeft = !faceLeft;
					}
				}
			}
        }

		public void Fall(Physics physics)
		{
			// Fall if airborne
			if (!grounded)
			{
				y_vel += physics.gravity;
				if (y_vel > physics.maxFallSpeed)
					y_vel = physics.maxFallSpeed;
				spriteY += Convert.ToInt32(y_vel);
			}
			else
			{
				y_vel = 1;
			}

			grounded = false;

			// Downward collisions
			foreach (Sprite s in physics.items)
			{
				if (Intersects(s))
				{
					// Up collision
					if (topWall - Convert.ToInt32(y_vel) > s.bottomWall)
					{
						y_vel = 0;
						while (topWall < s.bottomWall)
							spriteY++;
					}

					// Down collision
					else if (!grounded &&
						(bottomWall - Convert.ToInt32(y_vel)) < s.topWall)
					{
						grounded = true;
						while (bottomWall > s.topWall)
							spriteY--;
					}
				}
			}
		}

		public void Jump(Physics physics)
		{
			if (grounded && rnd.NextDouble() > .95){
				y_vel = -11;
				grounded = false;
			}
		}

		public virtual void Animate(Physics physics)
        {
            frameIndex = ((physics.count + seed) / 8 % 4) * 32;
        }

    }
}

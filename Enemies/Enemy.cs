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
		public double xVel;
		public double yVel;
		public int jumpHeight;
        public int seed;
		public int speed;
        public int health;
		public int hurtPoint;
		public bool grounded;
		protected Random rnd;

        public Enemy(int x, int y)
            : base(x, y, 32, 32)
        {
            this.imageFile = "rat.png";
			speed = 2;
			jumpHeight = 7;
			xVel = speed;
            yVel = 0;
            health = 3;
			grounded = false;
			hurtPoint = 0;

            // Sprite business
            rnd = new Random();
            seed = rnd.Next(0, 3);
        }

		public virtual void Move(Level level)
        {

			// Move horizontally
			spriteX += Convert.ToInt32(xVel);

			foreach (Sprite s in level.items)
			{
				if (s.isSolid && Intersects(s))
				{
					// Collide with block on side, turn around
					if (bottomWall > s.topWall &&
						(rightWall - Convert.ToInt32(xVel) < s.leftWall && xVel > 0 ||
							leftWall - Convert.ToInt32(xVel) > s.rightWall && xVel < 0))
					{
						xVel = - xVel;
						faceLeft = !faceLeft;
					}
				}
			}
        }

		public virtual void Fall(Level level)
		{
			// Fall if airborne
			if (!grounded)
			{
				yVel += level.gravity;
				if (yVel > level.maxFallSpeed)
					yVel = level.maxFallSpeed;
				spriteY += Convert.ToInt32(yVel);
			}
			else
			{
				yVel = 1;
			}

			grounded = false;

			// Downward collisions
			foreach (Sprite s in level.items)
			{
				if (s.isSolid && Intersects(s))
				{
					// Up collision
					if (topWall - Convert.ToInt32(yVel) > s.bottomWall)
					{
						yVel = 0;
						while (topWall < s.bottomWall)
							spriteY++;
					}

					// Down collision
					else if (!grounded &&
						(bottomWall - Convert.ToInt32(yVel)) < s.topWall)
					{
						grounded = true;
						while (bottomWall > s.topWall)
							spriteY--;
					}
				}
			}
		}

		public virtual void Jump(Level level)
		{
			yVel = -jumpHeight;
			grounded = false;
		}

		public virtual void Animate(Level level)
        {
			if ((level.count - hurtPoint) > 2)
				spriteColor = Color.White;
        }

    }
}

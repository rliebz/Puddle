using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using TiledSharp;

namespace Puddle
{
	class Bird : Enemy
    {
	
		private bool flapUp;

		public Bird(TmxObjectGroup.TmxObject obj)
			: this(obj.X, obj.Y)
		{ }

		public Bird(int x, int y)
			: base(x, y)
		{
			imageFile = "Bird.png";
			collisionHeight = 20;
			speed = 3;
			x_vel = speed;
			y_vel = 0;
			health = 3;
			flapUp = true;

			// Unsynchronize birds
			seed = (spriteY * 7 + spriteX * 13 + rnd.Next(101)) % 200;
			frameIndex = (seed % 3) * 32;
		}

		public override void Update(Level level, ContentManager content)
        {
            // Move
            Move(level);

			// Shoot maybe
			if ((level.count + seed) % 200 == 0)
			{
				Shoot(level, content);
				Console.WriteLine(seed);
			}
				
			// Animate sprite
			Animate(level);

            // Be killed if necessary
			if (health <= 0)
				destroyed = true;
        }
			
		// Shoot an Egg downward
		public void Shoot(Level level, ContentManager content)
		{

			Egg egg = new Egg(spriteX - 10, spriteY);
			egg.LoadContent(content);
			level.projectiles.Add((Sprite)egg);

		}

		public override void Animate(Level level)
		{
			// Animate once every several frames
			if (level.count % 6 != 0)
				return;

			// Wings go forward
			if (flapUp)
			{

				if (frameIndex < (32 * 3))
				{
					frameIndex += 32;
				}
				else
				{
					frameIndex -= 32;
					flapUp = false;
				}
			}

			// Wings go backward
			else
			{
				if (frameIndex > 0)
				{
					frameIndex -= 32;
				}
				else
				{
					frameIndex += 32;
					flapUp = true;
				}
			}

			base.Animate(level);
		}
    }
}

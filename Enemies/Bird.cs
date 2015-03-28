using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
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
			imageFile = "Enemies/bird";
			baseCollisionHeight = 0.625;
			speed = 3;
			xVel = speed;
			yVel = 0;
			health = 3;
			flapUp = true;

			// Unsynchronize birds
			seed = (spriteY * 7 + spriteX * 13 + rnd.Next(101)) % 200;
			frameIndexX = (seed % 3) * 32;
		}

		public override void Update(Level level)
        {
			base.Update(level);

            // Move
            Move(level);

			// Shoot maybe
			if ((level.count + seed) % 200 == 0)
				Shoot(level);
				
			// Animate sprite
			Animate(level);
        }
			
		// Shoot an Egg downward
		public void Shoot(Level level)
		{

			Egg egg = new Egg(spriteX - 10, spriteY);
			egg.LoadContent(level.content);
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

				if (frameIndexX < (32 * 3))
				{
					frameIndexX += 32;
				}
				else
				{
					frameIndexX -= 32;
					flapUp = false;
				}
			}

			// Wings go backward
			else
			{
				if (frameIndexX > 0)
				{
					frameIndexX -= 32;
				}
				else
				{
					frameIndexX += 32;
					flapUp = true;
				}
			}

			base.Animate(level);
		}
    }
}

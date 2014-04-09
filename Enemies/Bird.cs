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

			// Sprite business
			seed = rnd.Next(0, 3);
		}

		public override void Update(Level level, ContentManager content)
        {
            // Move
            Move(level);

			// Shoot maybe
			if (level.count % 120 == 0)
				Shoot(level, content);
				
			// Animate sprite
			Animate(level);

            // Be killed if necessary
			if (health <= 0)
				destroyed = true;
        }
			
		// Shoot a fireball downward
		public void Shoot(Level level, ContentManager content)
		{

			Fireball fireball = new Fireball(spriteX - 16, spriteY - 16, "down");
			fireball.LoadContent(content);
			level.projectiles.Add((Sprite)fireball);

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

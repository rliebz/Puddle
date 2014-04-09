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
	class Hand : Enemy
    {
		public Hand(TmxObjectGroup.TmxObject obj)
			: this(
				obj.X, obj.Y,
				obj.Properties.ContainsKey("left") ? Boolean.Parse(obj.Properties["left"]) : false
			)
		{ }

		public Hand(int x, int y, bool left=false)
			: base(x, y)
		{
			faceLeft = left;
			collisionWidth = 64;
			collisionHeight = 64;
			spriteWidth = 64;
			spriteHeight = 64;
			frameWidth = 64;
			frameHeight = 64;
			this.imageFile = "Hand.png";
			speed = 1;
			x_vel = faceLeft ? 0 : speed;
			y_vel = faceLeft ? speed : 0;
			health = 15;

			// Sprite business
			seed = rnd.Next(0, 3);
		}

		public override void Update(Level level, ContentManager content)
        {
            // Move
			Move(level);
	
            // Be killed if necessary
            destroyed = (health <= 0);

			Animate(level);
        }
			
		public override void Move(Level level)
		{
			if (level.count % 80 == 0)
			{
				if (x_vel != 0)
				{
					y_vel = x_vel;
					x_vel = 0;
				}
				else
				{
					x_vel = -y_vel;
					y_vel = 0;
				}
			}

			spriteX += Convert.ToInt32(x_vel);
			spriteY += Convert.ToInt32(y_vel);
		}

		public override void LoadContent(ContentManager content)
		{
			base.LoadContent(content);
		}

		public override void Draw(SpriteBatch sb)
		{
			base.Draw(sb);
		}

    }
}

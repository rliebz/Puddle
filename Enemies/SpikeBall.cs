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
	class SpikeBall : Enemy
    {
		public SpikeBall(TmxObjectGroup.TmxObject obj)
			: this(obj.X, obj.Y)
		{ }

		public SpikeBall(int x, int y)
			: base(x, y)
		{
			imageFile = "spikeball.png";
			collisionHeight = 26;
            collisionWidth = 26;
            spriteY -= 16;
			speed = 0;
			x_vel = speed;
			y_vel = 0;
            health = 0;
		}

		public override void Update(Level level)
        {
        }
			

		public override void Draw(SpriteBatch sb)
		{
			// Prevent damage animation
            spriteColor = Color.White;
            base.Draw(sb);
		}
    }
}

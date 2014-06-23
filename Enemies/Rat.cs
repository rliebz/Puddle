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
	class Rat : Enemy
    {
	
		public Rat(TmxObjectGroup.TmxObject obj)
			: this(obj.X, obj.Y)
		{ }

		public Rat(int x, int y)
			: base(x, y)
		{
			this.imageFile = "rat.png";
			speed = 2;
			xVel = speed;
			yVel = 0;
			health = 3;

			// Sprite business
			seed = rnd.Next(0, 3);
		}

        public override void Update(Level level)
        {
			base.Update(level);

            // Move
            Move(level);

			// Fall
			Fall(level);

			// Maybe jump
			if (grounded && rnd.NextDouble() > .99)
				Jump(level);

            // Animate sprite
            Animate(level);

        }
			

		public override void Animate(Level level)
        {
            frameIndexX = ((level.count + seed) / 8 % 4) * 32;
			base.Animate(level);
        }

    }
}

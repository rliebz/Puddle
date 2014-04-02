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
	class Rat : Enemy
    {

		public Rat(int x, int y)
            : base(x, y)
        {
            this.imageFile = "rat.png";
			speed = 2;
			x_vel = speed;
            y_vel = 0;
            health = 3;

            // Sprite business
            seed = rnd.Next(0, 3);
        }

        public override void Update(Level level)
        {
            // Move
            Move(level);

			// Fall
			Fall(level);

			// Maybe jump
			Jump(level);

            // Animate sprite
            Animate(level);

            // Be killed if necessary
            destroyed = (health <= 0);
        }
			

		public override void Animate(Level level)
        {
            frameIndex = ((level.count + seed) / 8 % 4) * 32;
        }

    }
}

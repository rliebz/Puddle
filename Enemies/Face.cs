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
	class Face : Enemy
    {
	
		public Face(TmxObjectGroup.TmxObject obj)
			: this(obj.X, obj.Y)
		{ }

		public Face(int x, int y)
			: base(x, y)
		{
			this.imageFile = "rat.png";
			speed = 0;
			x_vel = speed;
			y_vel = 0;
			health = 15;

			// Sprite business
			seed = rnd.Next(0, 3);
		}

		public override void Update(Level level, ContentManager content)
        {
            // Move
            Move(level);

			// Shoot maybe
			if (rnd.NextDouble() > .995)
				Shoot(level, content);
				
            // Animate sprite
            Animate(level);

            // Be killed if necessary
            destroyed = (health <= 0);
        }
			
		// Shoot 4 fireballs in all directions
		public void Shoot(Level level, ContentManager content)
		{
			Fireball fireball = new Fireball(spriteX - 16, spriteY - 16, "up");
			fireball.LoadContent(content);
			level.projectiles.Add((Sprite)fireball);

			fireball = new Fireball(spriteX - 16, spriteY - 16, "down");
			fireball.LoadContent(content);
			level.projectiles.Add((Sprite)fireball);

			fireball = new Fireball(spriteX - 16, spriteY - 16, "left");
			fireball.LoadContent(content);
			level.projectiles.Add((Sprite)fireball);

			fireball = new Fireball(spriteX - 16, spriteY - 16, "right");
			fireball.LoadContent(content);
			level.projectiles.Add((Sprite)fireball);
		}

		public override void Animate(Level level)
        {
            frameIndex = ((level.count + seed) / 8 % 4) * 32;
        }

    }
}

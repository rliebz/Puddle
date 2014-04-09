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
	
		public Bird(TmxObjectGroup.TmxObject obj)
			: this(obj.X, obj.Y)
		{ }

		public Bird(int x, int y)
			: base(x, y)
		{
			this.imageFile = "rat.png";
			speed = 3;
			x_vel = speed;
			y_vel = 0;
			health = 3;

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

            // Be killed if necessary
			if (health <= 0)
			{
				destroyed = true;
				foreach (Enemy e in level.enemies)
				{
					e.health = 0;
				}
				level.player.newMap = "Content/Level6.tmx";
			}
        }
			
		// Shoot a fireball downward
		public void Shoot(Level level, ContentManager content)
		{

			Fireball fireball = new Fireball(spriteX - 16, spriteY - 16, "down");
			fireball.LoadContent(content);
			level.projectiles.Add((Sprite)fireball);

		}
    }
}

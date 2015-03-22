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
	class Face : Enemy
    {
		public int maxHealth;

		public Face(TmxObjectGroup.TmxObject obj)
			: this(obj.X, obj.Y)
		{ }

		public Face(int x, int y)
			: base(x, y)
		{
			baseCollisionWidth = 3;
			baseCollisionHeight = 3;
			baseWidth = 3;
			baseHeight = 3;
			frameWidth = 3 * spriteSize;
			frameHeight = 3 * spriteSize;
			this.imageFile = "Enemies/face.png";
			speed = 2;
			xVel = speed;
			yVel = 0;

			maxHealth = 30;
			health = 30;

			// Sprite business
			seed = rnd.Next(0, 3);
		}

		public override void Update(Level level)
        {
			base.Update(level);

            // Move
            Move(level);

			// Shoot maybe
			if (level.count % 30 == 0)
				Shoot(level);

			Animate(level);

			if (destroyed)
			{
				foreach (Enemy e in level.enemies)
					e.health = 0;
			}
        }
			
		// Shoot 4 fireballs in all directions
		public void Shoot(Level level)
		{
			Fireball fireball = new Fireball(spriteX - 16, spriteY + 48, "down");
			fireball.LoadContent(level.content);
			level.projectiles.Add((Sprite)fireball);
		}

		public override void Draw(SpriteBatch sb)
		{
			base.Draw(sb);

			// Draw boss health bar
			sb.Draw(
				blankImage,
				new Rectangle(
                    tileSize * 6 + tileSize / 2, 
                    tileSize * 21 + tileSize / 4, 
                    tileSize * 9, 
                    tileSize / 2
                ),
				new Color(60, 22, 22)
			);
			sb.Draw(
				blankImage,
				new Rectangle(
                    tileSize * 13 / 2 + tileSize / 2, 
                    tileSize * 21 + tileSize / 4, 
                    tileSize * 9 * health / maxHealth, 
                    tileSize / 2
                ),
				Color.Firebrick
			);
		}

        public override void Animate(Level level)
        {
            frameIndexX = ((level.count + seed) / 8 % 4) * frameWidth;
            base.Animate(level);
        }


    }
}

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
			frameWidth = 3 * SIZE;
			frameHeight = 3 * SIZE;
			this.imageFile = "Enemies/face";
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

		public override void DrawUI(SpriteBatch sb, GraphicsDeviceManager graphics, int gameScale)
        {
            int healthWidth = SIZE * 9;
            int healthHeight = SIZE / 2;
            Vector2 position = new Vector2(
                graphics.PreferredBackBufferWidth / 2 / gameScale,
                graphics.PreferredBackBufferHeight / gameScale - Sprite.SIZE
            );
            Vector2 center = new Vector2(
                (float)healthWidth / 2, 
                (float)healthHeight / 2
            );


			// Draw boss health bar
            sb.Draw(
                blankImage,
                position,
                new Rectangle(0, 0, healthWidth + 2, healthHeight + 2),
                Color.Black,
                0,
                center + new Vector2(1, 1),
                1f,
                SpriteEffects.None,
                0
            );
			sb.Draw(
                blankImage,
                position,
                new Rectangle(0, 0, healthWidth, healthHeight),
                new Color(60, 22, 22),
                0,
                center,
                1f,
                SpriteEffects.None,
                0
			);
			sb.Draw(
				blankImage,
                position,
				new Rectangle(
                    0, 0,
                    healthWidth * health / maxHealth,
                    healthHeight
                ),
				Color.Firebrick,
                0,
                center,
                1f,
                SpriteEffects.None,
                0
			);
		}

        public override void Animate(Level level)
        {
            frameIndexX = ((level.count + seed) / 8 % 4) * frameWidth;
            base.Animate(level);
        }


    }
}

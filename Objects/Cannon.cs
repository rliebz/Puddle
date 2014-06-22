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
    class Cannon : Sprite
    {
		public string direction;
        public int speed;

        //TODO: add in function passing for individual button actions
        public Cannon(TmxObjectGroup.TmxObject obj) :
            base(obj.X + 16, obj.Y, 32, 32)
        {
            this.imageFile = "cannon.png";
            this.name = obj.Name;
			isSolid = true;
            frameWidth = 64;
            spriteWidth = 64;
            collisionWidth = 64;
            faceLeft = false;
			direction = obj.Properties["direction"];
			if (direction == "left")
                faceLeft = true;
            speed = obj.Properties.ContainsKey("speed") ? Int32.Parse(obj.Properties["speed"]) : 125;
        }

        public override void Update(Level level)
        {
			if (level.count % speed == 0)
            {
				Fireball fireball = new Fireball(
					spriteX + (faceLeft ? -63 : 31), spriteY - 16, direction
				);
				fireball.LoadContent(level.content);
				level.projectiles.Add((Sprite)fireball);
            }
        }
    }
}

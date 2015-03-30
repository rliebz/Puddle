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
    class Cannon : Sprite
    {
		public string direction;
        public int speed;

        //TODO: add in function passing for individual button actions
        public Cannon(TmxObjectGroup.TmxObject obj) :
            base(obj.X + SIZE / 2, obj.Y)
        {
            this.imageFile = "Textures/cannon";
            this.name = obj.Name;
			isSolid = true;
            frameWidth = 64;
            baseWidth = 2;
            baseCollisionWidth = 2;
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

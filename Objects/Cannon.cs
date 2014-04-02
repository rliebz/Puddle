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
            if (obj.Properties["direction"].Equals("left"))
                faceLeft = true;
        }

        public override void Update(Level level, ContentManager content)
        {
            if (level.count % 100 == 0)
            {
                Fireball fireball = new Fireball(this);
                fireball.LoadContent(content);
				level.projectiles.Add((Sprite)fireball);
            }
        }
    }
}

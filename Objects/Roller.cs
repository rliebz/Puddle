using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Audio;
using TiledSharp;

namespace Puddle
{
    class Roller : Sprite
    {
        public double speed;

        public Roller(TmxObjectGroup.TmxObject obj) :
            base(obj.X, obj.Y)
        {
            imageFile = "Textures/roller";
            name = obj.Name;

            faceLeft = obj.Properties.ContainsKey("left") && Boolean.Parse(obj.Properties["left"]);

            isSolid = true;

            baseCollisionHeight = 0.25;
            spriteY += 12;
            speed = faceLeft ? -2 : 2;
        }

        public override void Update(Level level)
        {
            Animate(level);
        }



        public void Animate(Level level)
        {
            frameIndexX = ((level.count) / 4 % 8) * 32;
        }

    }
}
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Audio;
using TiledSharp;

namespace Puddle
{
    class Roller : Sprite
    {

        public bool activated;
        private SoundEffectInstance instance;

        public Roller(TmxObjectGroup.TmxObject obj) :
            base(obj.X, obj.Y, 32, 32)
        {
            imageFile = "roller.png";
            isSolid = true;

            collisionHeight = 8;
            spriteY += 12;
        }

        public override void Update(Level level)
        {
            Animate(level);
        }



        public void Animate(Level level)
        {
            frameIndex = ((level.count) / 8 % 4) * 32;
        }

    }
}
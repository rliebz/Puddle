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
    class PowerUp : Sprite
    {
        public PowerUp(TmxObjectGroup.TmxObject obj) :
            base(obj.X, obj.Y, 32, 32)
        {
            imageFile = obj.Name + ".png";
            name = obj.Name.ToLower();
        }

    }
}

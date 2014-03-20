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

        public string name;

        public PowerUp(TmxObjectGroup.TmxObject obj) :
            base(obj.X, obj.Y, 32, 32)
        {
            this.name = obj.Name.ToLower();
            this.imageFile = obj.Name + ".png";
        }

    }
}

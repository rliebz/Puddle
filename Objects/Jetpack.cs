using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

namespace Puddle
{
    class Jetpack : Sprite
    {

        public Jetpack(int x, int y) :
            base(x, y, 32, 32)
        {
            this.imageFile = "jetpack.png";
        }

    }
}

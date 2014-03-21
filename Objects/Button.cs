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
    class Button : Sprite
    {
        public bool activating;
        public bool activated;

        //TODO: add in function passing for individual button actions
        public Button(TmxObjectGroup.TmxObject obj) :
            base(obj.X, obj.Y, 32, 32)
        {
            this.imageFile = "button.png";
            faceLeft = false;
            sizeX = 24;
            sizeY = 30;
            if (obj.Properties["direction"].Equals("left"))
            {
                faceLeft = true;
                spriteX -= 9;
            }
            else
                spriteX += 9;
            
        }

        public override void Update(Physics physics)
        {
            Animate(physics);
        }

        public void Animate(Physics physics)
        {
            if (activating && frameIndex < (32 * 7))
                frameIndex += 32;
        }

        public void Action(Physics physics)
        {
            if (activated)
                return;
            activating = true;
            activated = true;
            //Do some action
        }
    }
}

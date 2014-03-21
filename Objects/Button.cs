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
        public int frameIndex;
        public bool activating;
        public bool activated;

        //TODO: add in function passing for individual button actions
        public Button(TmxObjectGroup.TmxObject obj) :
            base(obj.X, obj.Y, 32, 32)
        {
            this.imageFile = "button.png";
            faceLeft = false;
            if (obj.Properties["direction"].Equals("left"))
            {
                faceLeft = true;
            }
            
        }

        public void Update(Physics physics)
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

/*        public new void Draw(SpriteBatch sb)
        {
            // Draw the button
            sb.Draw(
                image,
                new Rectangle(spriteX, spriteY, spriteWidth, spriteHeight),
                new Rectangle(frameIndex, 0, 32, 32),
                Color.White,
                0,
                new Vector2(spriteWidth / 2, spriteHeight / 2),
                SpriteEffects.FlipHorizontally,
                0
            );

        }
 * */
    }
}

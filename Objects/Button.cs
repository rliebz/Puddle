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
        public bool played; //If sound has been played
        public string name;

        // TODO: add in function passing for individual button actions
        public Button(TmxObjectGroup.TmxObject obj) :
            base(obj.X, obj.Y, 32, 32)
        {
            imageFile = "button.png";
            played = false;
            soundFiles.Add("button.wav");
            name = obj.Name;
            collisionWidth = 24;
            collisionHeight = 30;

            if (obj.Properties["direction"].Equals("left"))
            {
                faceLeft = true;
                spriteX -= 9;
            }
            else
            {
                faceLeft = false;
                spriteX += 9;
            }
        }

        public override void Update(Physics physics)
        {
            Animate(physics);
        }

        public void Animate(Physics physics)
        {
            if (activating && frameIndex < (32 * 7))
            {
                frameIndex += 32;
            }
            if (activating && !played)
            {
                soundList["button.wav"].Play();
                played = true;
            }
        }

        public void Action(Physics physics)
        {
            if (activated)
                return;

            activating = true;
            activated = true;

            if (this.name == "Button 1")
            {
                foreach (Block b in physics.blocks)
                {
                    if (b.name == "Block 3")
                    {
                        b.changeType("push");
                    }
                }
            }
            else if (this.name == "Button 2")
            {
                foreach (Block b in physics.blocks)
                {
                    if (b.name == "Block 2")
                    {
                        b.changeType("push");
                    }
                }
            }
            else if (this.name == "Button 3")
            {
                foreach (Block b in physics.blocks)
                {
                    if (b.name == "Block 4")
                    {
                        b.changeType("push");                        
                    }
                }
            }

        }
    }
}

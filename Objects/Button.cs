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
        public bool holdButton;

        // TODO: add in function passing for individual button actions
        public Button(TmxObjectGroup.TmxObject obj) :
            base(obj.X, obj.Y, 32, 32)
        {
            imageFile = "button.png";
            holdButton = obj.Properties.ContainsKey("hold") && Boolean.Parse(obj.Properties["hold"]);
            if (holdButton)
            {
                holdButton = true;
                spriteColor = Color.Black;
            }
            played = false;
            soundFiles.Add("Sounds/button.wav");
            name = obj.Name;
            collisionWidth = 24;
            collisionHeight = 30;

            if (obj.Properties["direction"].Equals("left"))
            {
                faceLeft = true;
                spriteX -= 9;
            }
            else if (obj.Properties["direction"].Equals("right"))
            {
                faceLeft = false;
                spriteX += 9;
            }
            else if (obj.Properties["direction"].Equals("up"))
            {
                rotationAngle = MathHelper.PiOver2;
                spriteY += 9;
                collisionHeight = 24;
                collisionWidth = 30;
            }
            else
            {
                rotationAngle = MathHelper.PiOver2 * 3;
                spriteY -= 9;
                collisionHeight = 24;
                collisionWidth = 30;
            }
        }

        public override void Update(Level level)
        {
            CheckCollisions(level);
            Animate(level);
        }

        public void Animate(Level level)
        {
            if (activated)
            {
                if (frameIndex < (32 * 7))
                {
                    frameIndex += 32;
                    if (!played)
                    {
                        soundList["Sounds/button.wav"].Play();
                        played = true;
                    }
                }
            }
            else
            {
                if (holdButton && frameIndex > 0)
                {
                    frameIndex -= 32;
                }
                else
                {
                    activated = false;
                }
            }
        }

        public bool CheckCollisions(Level level)
        {
            if (Intersects(level.player))
            {
                Action(level);
                activated = true;
                return true;
            }
            foreach (Sprite item in level.items)
            {
                if (item is Block && Intersects(item) && ((Block)(item)).blockType == "push")
                {
                    Action(level);
                    activated = true;
                    return true;
                }
            }
            if (holdButton)
                activated = false;
            return false;
        }

        public void Action(Level level)
        {
            if (activated)
                return;

            activating = true;
            activated = true;

            if (this.name == "Button 1")
            {
				foreach (Sprite s in level.items)
                {
                    if (s.name == "Block 3")
                    {
						((Block)s).changeType("push");
                    }
                }
            }
            else if (this.name == "Button 2")
            {
				foreach (Sprite s in level.items)
                {
                    if (s.name == "Block 2")
                    {
						((Block)s).changeType("push");
                    }
                }
            }
            else if (this.name == "Button 3")
            {
				foreach (Sprite s in level.items)
                {
                    if (s.name == "Block 4")
                    {
						((Block)s).changeType("push");                        
                    }
                }
            }

        }
    }
}

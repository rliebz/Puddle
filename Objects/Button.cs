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
        public bool activated;
		public bool pressed;
        public bool holdButton;

        // TODO: add in function passing for individual button actions
        public Button(TmxObjectGroup.TmxObject obj) :
            base(obj.X, obj.Y, 32, 32)
        {
            imageFile = "button.png";
            holdButton = obj.Properties.ContainsKey("hold") && Boolean.Parse(obj.Properties["hold"]);
            if (holdButton)
            {
				spriteColor = Color.Yellow;
            }

			activated = false;
			pressed = false;
            soundFiles.Add("Sounds/button.wav");
            name = obj.Name;
			collisionWidth = 20;
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
			
		public void CheckCollisions(Level level)
        {
			// Assume unpressed for hold buttons
			if (holdButton)
				pressed = false;

			// Do nothing if already pressed
			else if (pressed)
				return;

			// Press if intersecting with player
			if (Intersects(level.player))
			{
				pressed = true;
                Action(level);
            }

			// Press if intersecting with block
            foreach (Sprite item in level.items)
            {
				if (Intersects(item) && item is Block && ((Block)(item)).blockType == "push")
				{
					pressed = true;
                    Action(level);
                }
            }

			// Hold button is no longer held
			if (holdButton && activated && !pressed)
				UnAction(level);
        }

        public void Action(Level level)
        {
            if (activated)
                return;
				
			soundList["Sounds/button.wav"].Play();

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
			else if (this.name == "Button 10")
			{
				foreach (Sprite s in level.items)
				{
					if (s.name == "Block 10")
					{
						((Block)s).changeType("push");                        
					}
				}
			}

			activated = true;
        }

		public void UnAction(Level level)
		{
			activated = false;
		}

		public void Animate(Level level)
		{
			if (pressed)
			{
				if (frameIndex < (32 * 6))
				{
					frameIndex += 32;
				}
			}
			else if (frameIndex > 0)
			{
				frameIndex -= 32;
			}
		}
    }
}

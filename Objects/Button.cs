using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using TiledSharp;

namespace Puddle
{
    class Button : Sprite
    {
        public bool activated;
		public bool pressed;
        public bool holdButton;
        public bool creditScreen;
        public bool controlScreen;
        public Texture2D slideImage;

        // TODO: add in function passing for individual button actions
        public Button(TmxObjectGroup.TmxObject obj) :
            base(obj.X, obj.Y)
        {
            imageFile = "button";
            holdButton = obj.Properties.ContainsKey("hold") && Boolean.Parse(obj.Properties["hold"]);
            displayText = obj.Properties.ContainsKey("text") ? obj.Properties["text"] : "";
			displayTextY = -40;
		    spriteColor = holdButton ? Color.CornflowerBlue : Color.OrangeRed;

			activated = false;
			pressed = false;
            creditScreen = false;
            controlScreen = false;
            soundFiles.Add("Sounds/button");
            soundFiles.Add("Sounds/HoldButtonPress");
            soundFiles.Add("Sounds/HoldButtonRel");
            name = obj.Name;
			baseCollisionWidth = 0.625;
            baseCollisionHeight = 0.625;
            depth = -1;

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
                rotationAngle = MathHelper.PiOver2 * 3;
                spriteY -= 9;
            }
            else
            {
                rotationAngle = MathHelper.PiOver2;
                spriteY += 9;
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

            activated = true;

            if (holdButton)
                soundList["Sounds/HoldButtonPress"].Play();
            else
			    soundList["Sounds/button"].Play();

            if (name.Contains("Credits"))
            {
                this.creditScreen = true;
				slideImage = level.content.Load<Texture2D>("Slides/credits");
                return;
            }
            
            if (name.Contains("Controls"))
            {
                this.controlScreen = true;
				slideImage = level.content.Load<Texture2D>("Slides/pause0");
                return;
            }

			int number = int.Parse(name.Split(' ')[1]);


			foreach (Sprite s in level.items)
			{
				if (s.name.Contains("Block") && int.Parse(s.name.Split(' ')[1]) == number )
				{
					((Block)s).changeType("push");
                    foreach (Sprite s2 in level.items)
                    {
                        if (s2 is Block)
                            ((Block)s2).neighborsFound = false;
                    }
				}
				else if (s.name.Contains("Gate") && int.Parse(s.name.Split(' ')[1]) == number )
				{
					((Block)s).changeType("transparent");
				}
                else if (s.name.Contains("Invis") && int.Parse(s.name.Split(' ')[1]) == number)
                {
                    ((Block)s).changeType("temp");
                }
			}

        }

		public void UnAction(Level level)
		{
            activated = false;
            soundList["Sounds/HoldButtonRel"].Play();
            if (name.Contains("Credits"))
            {
                this.creditScreen = false;
                return;
            }
            if (name.Contains("Controls"))
            {
                this.controlScreen = false;
                return;
            }

			foreach (Sprite s in level.items)
			{
				int number = int.Parse(name.Split(' ')[1]);
				if (s.name.Contains("Gate") && int.Parse(s.name.Split(' ')[1]) == number)
				{
					((Block)s).changeType("temp");
				}
			}

		}

		public void Animate(Level level)
		{
			if (pressed)
			{
				if (frameIndexX < (32 * 6))
				{
					frameIndexX += 32;
				}
			}
			else if (frameIndexX > 0)
			{
				frameIndexX -= 32;
			}
		}

        public override void Draw(SpriteBatch sb)
        {
            if (creditScreen || controlScreen)
            {
                sb.Draw(
                    slideImage,
                    new Rectangle(0, 0, 21 * SIZE, 17 * SIZE),
                    Color.White
                );
            }
            base.Draw(sb);
        }
    }
}

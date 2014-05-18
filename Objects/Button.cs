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
        public bool creditScreen;
        public bool controlScreen;
        public Texture2D slideImage;

        // TODO: add in function passing for individual button actions
        public Button(TmxObjectGroup.TmxObject obj) :
            base(obj.X, obj.Y, 32, 32)
        {
            imageFile = "button.png";
            holdButton = obj.Properties.ContainsKey("hold") && Boolean.Parse(obj.Properties["hold"]);
            displayText = obj.Properties.ContainsKey("text") ? obj.Properties["text"] : "";
			displayTextX = -35;
			displayTextY = -40;
		    spriteColor = holdButton ? Color.CornflowerBlue : Color.OrangeRed;

			activated = false;
			pressed = false;
            creditScreen = false;
            controlScreen = false;
            soundFiles.Add("Sounds/button.wav");
            soundFiles.Add("Sounds/HoldButtonPress.wav");
            soundFiles.Add("Sounds/HoldButtonRel.wav");
            name = obj.Name;
			collisionWidth = 20;
            collisionHeight = 30;
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
                collisionHeight = 24;
                collisionWidth = 30;
            }
            else
            {
                rotationAngle = MathHelper.PiOver2;
                spriteY += 9;
                collisionHeight = 24;
                collisionWidth = 30;
            }
        }

        public override void Update(Level level, ContentManager content)
        {
            CheckCollisions(level, content);
            Animate(level);
        }
			
		public void CheckCollisions(Level level, ContentManager content)
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
                Action(level, content);
            }

			// Press if intersecting with block
            foreach (Sprite item in level.items)
            {
				if (Intersects(item) && item is Block && ((Block)(item)).blockType == "push")
				{
					pressed = true;
                    Action(level, content);
                }

            }

			// Hold button is no longer held
			if (holdButton && activated && !pressed)
				UnAction(level);
        }

        public void Action(Level level, ContentManager content)
        {
            if (activated)
                return;

            activated = true;

            if (holdButton)
                soundList["Sounds/HoldButtonPress.wav"].Play();
            else
			    soundList["Sounds/button.wav"].Play();

            if (name.Contains("Credits"))
            {
                this.creditScreen = true;
				slideImage = content.Load<Texture2D>("Slides/credits.png");
                return;
            }
            
            if (name.Contains("Controls"))
            {
                this.controlScreen = true;
				slideImage = content.Load<Texture2D>("Slides/pause0.png");
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
            soundList["Sounds/HoldButtonRel.wav"].Play();
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

        public override void Draw(SpriteBatch sb)
        {
            if (creditScreen || controlScreen)
            {
                sb.Draw(
                    slideImage,
                    new Rectangle(
                    0, 0,
                    720,
                    540
                    ),
                    Color.White
                );
            }
            base.Draw(sb);
        }
    }
}

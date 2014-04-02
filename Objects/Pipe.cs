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
    class Pipe : Sprite
    {

        public string direction;

        public Pipe(TmxObjectGroup.TmxObject obj) :
            base(obj.X, obj.Y, 32, 32)
        {
            imageFile = "pipe.png";
			isSolid = true;

            name = obj.Name;
            collisionWidth = 32;
			collisionHeight = 32;
			this.direction = obj.Properties.ContainsKey ("direction") ?
				(obj.Properties ["direction"]) : "up";

			if (this.direction == "up")
            {

				frameIndex = 0;
			
            }
			else if (this.direction == "down")
            {

				frameIndex = 32;
                
            }
			else if (this.direction == "vertical") {

				frameIndex = 32*2;

			}
			else if (this.direction == "horizontal")
			{

				frameIndex = 32*3;
			}

			else if (this.direction == "cornerLeft")
            {
                
				frameIndex = 32*4;

            }
			else if (this.direction == "cornerLeftR")
			{
			
				frameIndex = 32*5;

			}
			else if (this.direction == "cornerRightR")
			{

				frameIndex = 32*6;
			}

			else if (this.direction == "cornerRight")
            {
                
				frameIndex = 32*7;
            }
           

			else if (obj.Properties["direction"].Equals("end"))
			{
				this.direction = "end";
				frameIndex = 0;

			}


        }


		public void Action(Level level)
        {

			if (this is Pipe )
            {
                int x = 0;
                int y = 0;
				String pipeName = this.name.Remove(this.name.Length-1); 
				if(this.name.EndsWith("1")){
					foreach (Sprite i in level.items)
	                {
	                    if (i is Pipe)
	                    {

	                        Pipe p = (Pipe)i;
							if (p.name == pipeName.Insert(pipeName.Length,"2"))
	                        {
	                            x = p.spriteX;
	                            y = p.spriteY;
								Transport(level, x, y);
	                        }
	                    }
	                }
				}
				else if(this.name.EndsWith("2")){
					foreach (Sprite i in level.items)
					{
						if (i is Pipe)
						{

							Pipe p = (Pipe)i;
							if (p.name == pipeName.Insert(pipeName.Length,"1"))
							{
								x = p.spriteX;
								y = p.spriteY;
								Transport(level, x, y);
							}
						}
					}
				}
            }




        }
        public void Transport(Level level, int x, int y)
        {
            level.player.spriteX = x;
			level.player.spriteY = y - level.player.collisionHeight;

        }
		public override void Draw(SpriteBatch sb)
		{

			sb.Draw(
				image,
				new Rectangle(spriteX, spriteY, spriteWidth, spriteHeight),
				new Rectangle(frameIndex, 0, frameWidth, frameHeight),
				name.Contains("endPipe")?Color.Gold: Color.White,
				0,
				new Vector2(spriteWidth / 2, spriteHeight / 2),
				faceLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
				0
			);
		}

        	
    }
}

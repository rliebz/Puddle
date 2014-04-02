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
        //		public bool activating;
        //		public bool activated;
        public string direction;


        // TODO: add in function passing for individual button actions
        public Pipe(TmxObjectGroup.TmxObject obj) :
            base(obj.X, obj.Y, 32, 32)
        {
            imageFile = "pipe.png";

            name = obj.Name;
            collisionWidth = 32;
            collisionHeight = 24;
            //this.direction ="down";
            if (obj.Properties["direction"].Equals("down"))
            {
                this.direction = "down";
            }
            else if (obj.Properties["direction"].Equals("up"))
            {
                this.direction = "up";
                //faceLeft = false;
                //spriteX += 9;
            }
            else if (obj.Properties["direction"].Equals("cornerLeft"))
            {
                this.direction = "cornerLeft";
                frameIndex = 0;
                //frameIndexY = 32;
            }
            else if (obj.Properties["direction"].Equals("cornerRight"))
            {
                this.direction = "cornerRight";
                frameIndex = 0;
                //frameIndexY = 32;
            }
            else if (obj.Properties["direction"].Equals("horizontal"))
            {
                this.direction = "horizontal";
                frameIndex = 96;
            }

        }

        public void Action(Level level)
        {

            if (this.name == "Pipe1")
            {
                int x = 0;
                int y = 0;
                foreach (Sprite i in level.items)
                {
                    if (i is Pipe)
                    {
                        Pipe p = (Pipe)i;
                        if (p.name == "Pipe2")
                        {
                            x = p.spriteX;
                            y = p.spriteY;
                        }
                    }
                }

                Transport(level, x, y);
            }

            //if (this.end) 


        }
        public void Transport(Level level, int x, int y)
        {
            level.player.spriteX = x;
            level.player.spriteY = y - level.player.spriteHeight;

        }
        //		
    }
}

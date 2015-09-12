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
    class Pipe : Sprite
    {

        public string direction;
        public string destination;

        public Pipe(TmxObjectGroup.TmxObject obj) :
            base(obj.X, obj.Y)
        {
            imageFile = "Textures/pipe";
            isSolid = true;

            name = obj.Name;
            this.direction = obj.Properties.ContainsKey ("direction") ?
                (obj.Properties ["direction"]) : "up";

            this.destination = obj.Properties.ContainsKey("destination") ? obj.Properties["destination"] : "";
            displayText = obj.Properties.ContainsKey("text") ? obj.Properties["text"] : "";
            displayTextColor = Color.Black;

            spriteColor = name.Contains("endPipe") ? Color.Gold : Color.White;

            if (this.direction == "up")
            {
                frameIndexX = 0;
                displayTextX = 1;
                displayTextY = 7;
            }
            else if (this.direction == "left")
            {
                frameIndexX = 0;
                rotationAngle = MathHelper.PiOver2 * 3;
                displayTextX = 7;
                displayTextY = 1;
            }
            // TODO: These next two are probably incorrect
            else if (this.direction == "right")
            {
                frameIndexX = 0;
                rotationAngle = MathHelper.PiOver2;
                displayTextX = 7;
                displayTextY = 1;
            }
            else if (this.direction == "down")
            {
                frameIndexX = 32;                               
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
            level.player.spriteY = y - (level.player.collisionHeight + 1);
            level.player.movedY = 0;
        }

            
    }
}

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
    class Cannon : Sprite
    {
        public string name;

        //TODO: add in function passing for individual button actions
        public Cannon(TmxObjectGroup.TmxObject obj) :
            base(obj.X + 16, obj.Y, 32, 32)
        {
            this.imageFile = "cannon.png";
            this.name = obj.Name;
            frameWidth = 64;
            spriteWidth = 64;
            collisionWidth = 64;
            faceLeft = false;
            if (obj.Properties["direction"].Equals("left"))
                faceLeft = true;
        }

        public override void Update(Physics physics, ContentManager content)
        {
            if (physics.count % 100 == 0)
            {
                Fireball fireball = new Fireball(this);
                fireball.LoadContent(content);
                physics.fireballs.Add(fireball);
            }
        }


        /*public void Action(Physics physics)
        {
            if (activated)
                return;
            activating = true;
            activated = true;
            if (this.name == "Button 1")
            {
                foreach (Block b in physics.blocks)
                {
                    if (b.name == "Block 1")
                    {
                        b.changeType("push");
                        b.gravity = true;//Do some action
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
                        b.gravity = true;//Do some action
                    }
                }
            }
            else if (this.name == "Button 3")
            {
                foreach (Block b in physics.blocks)
                {
                    if (b.name == "Block 10")
                    {
                        b.changeType("push");
                        b.gravity = true;
                        
                    }
                    else if (b.name == "Block 11")
                    {
                        b.changeType("push");
                        b.gravity = true;
                        b.y_vel = 2;
                    }
                }
            }

        }*/
    }
}

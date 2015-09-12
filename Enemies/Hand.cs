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
    class Hand : Enemy
    {
        public Hand(TmxObjectGroup.TmxObject obj)
            : this(
                obj.X, obj.Y,
                obj.Properties.ContainsKey("left") ? Boolean.Parse(obj.Properties["left"]) : false
            )
        { }

        public Hand(int x, int y, bool left=false)
            : base(x, y, 2, 2)
        {
            depth = 1;
            faceLeft = left;
            baseCollisionWidth = 1.125;
            this.imageFile = "Enemies/hand";
            speed = 1;
            xVel = faceLeft ? 0 : speed;
            yVel = faceLeft ? speed : 0;
            health = 15;

            // Sprite business
            seed = rnd.Next(0, 3);
        }

        public override int leftWall
        {
            get
            {
                int offset = faceLeft ? collisionWidth / 4 : 0;
                return spriteX - (collisionWidth / 2 + offset);
            }
        }

        public override int rightWall
        {
            get
            {
                int offset = faceLeft ? 0 : collisionWidth / 4;
                return spriteX + (collisionWidth / 2 - 1 + offset);
            }
        }

        public override void Update(Level level)
        {
            base.Update(level);

            Move(level);
            Animate(level);
        }
            
        public override void Move(Level level)
        {
            if (level.count % 80 == 0)
            {
                if (xVel != 0)
                {
                    yVel = xVel;
                    xVel = 0;
                }
                else
                {
                    xVel = -yVel;
                    yVel = 0;
                }
            }

            spriteX += Convert.ToInt32(xVel);
            spriteY += Convert.ToInt32(yVel);
        }

    }
}

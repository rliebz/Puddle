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
    class Dying : Sprite
    {
        public double yVel;
        public Type type;

        public Dying(Enemy e)
            : base(e.spriteX, e.spriteY, e.baseWidth, e.baseHeight)
        {
            type = e.GetType();

            imageFile = e.imageFile;
            image = e.image;
            frameWidth = e.frameWidth;
            frameHeight = e.frameHeight;
            baseCollisionWidth = e.baseCollisionWidth;
            baseCollisionHeight = e.baseCollisionHeight;
            faceLeft = e.faceLeft;

            yVel = -8;
        }

        public override void Update(Level level)
        {
            // Rotate
            rotationAngle += 0.05f;

            // Fall
            yVel += level.gravity;
            if (yVel > level.maxFallSpeed)
                yVel = level.maxFallSpeed;
            spriteY += Convert.ToInt32(yVel);

            // Remove object if below screen
            if (topWall > 22 * 32)
            {
                destroyed = true;

                // Win game if boss is defeated
                if (type.Equals(typeof(Face)))
                    level.player.newMap = "Win";
            }
        }

    }
}

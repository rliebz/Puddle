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
    class Block : Sprite
    {
        public bool left;
        public bool right;
        public bool gravity;

        public bool rCol;
        public bool lCol;
        public bool dCol;
        public bool uCol;

        public double x_vel;
        public int y_vel;

        public string blockType;

        private Block uBlock;

        public int frameIndex;

        public Block(int x, int y, bool left=false, bool right=false, bool gravity=false)
            : base(x, y, 32, 32)
        {

            this.left = left;
            this.right = right;
            this.gravity = gravity;
       
            this.rCol = false;
            this.lCol = false;
            this.dCol = false;
            this.uCol = false;

            this.x_vel = 0;

            uBlock = null;
            this.blockType = "push";

            // Determine block image
            if (!this.gravity)
            {
                frameIndex = 0;
                this.blockType = "metal";
            }
            else if (right && !left)
                frameIndex = 0;
            else if (left && !right)
                frameIndex = 32;
            else if (left && right)
                frameIndex = 64;
            else
                frameIndex = 96;
        }

        public Block(TmxObjectGroup.TmxObject obj)
            : base(obj.X, obj.Y, 32, 32)
        {

            this.left = (obj.Properties.ContainsKey("left")) ? Boolean.Parse(obj.Properties["left"]) : false;
            this.right = (obj.Properties.ContainsKey("right")) ? Boolean.Parse(obj.Properties["right"]) : false;
            this.gravity = (obj.Properties.ContainsKey("gravity")) ? Boolean.Parse(obj.Properties["gravity"]) : false;

            this.blockType = "push";
     
            this.rCol = false;
            this.lCol = false;
            this.dCol = false;
            this.uCol = false;

            this.x_vel = 0;

            uBlock = null;

            // Determine block image
            if (!this.gravity)
            {
                frameIndex = 0;
                this.blockType = "metal";
            }
            else if (right && !left)
                frameIndex = 0;
            else if (left && !right)
                frameIndex = 32;
            else if (left && right)
                frameIndex = 64;
            else
                frameIndex = 96;
        }

        public void Update(Physics physics)
        {
            Move(physics);

            CheckCollisions(physics);

        }

        public void CheckCollisions(Physics physics)
        {
            // Assume no collisions
            rCol = false;
            lCol = false;
            dCol = false;
            uCol = false;

            // Check collisions with other blocks
            foreach (Block b in physics.blocks)
            {
                if (this != b)
                {
                    if (Intersects(b))
                    {
                        // Determine direction
                        // Downward Collisions
                        while (bottomWall >= b.topWall &&
                            Math.Abs(spriteY - b.spriteY) > 16)
                        {
                            spriteY--;
                            y_vel = 0;
                            dCol = true;
                        }

                        // Sideways collisions
                        if (!dCol)
                        {
                            if (rightWall >= b.leftWall)
                                rCol = true;
                            if (leftWall <= b.rightWall)
                                lCol = true;
                        }

                        dCol = false;
                    }
                }
            }
        }

        public void Move(Physics physics)
        {
            // Gravity
            if (gravity)
            {
                y_vel += physics.gravity;
                spriteY += y_vel;
            }

            // Move sideways
            spriteX += Convert.ToInt32(x_vel);
            if (uCol)
            {
                uBlock.x_vel = x_vel;
                uBlock.Move(physics);
            }

            // Reset x velocity
            x_vel = 0;
        }

        public new void LoadContent(ContentManager content)
        {
            images["push"] = content.Load<Texture2D>("push_block.png");
            images["metal"] = content.Load<Texture2D>("brick.png");
            image = images[this.blockType];
        }

        public new void Draw(SpriteBatch sb)
        {
            sb.Draw(
                image,
                new Rectangle(spriteX, spriteY, spriteWidth, spriteHeight),
                new Rectangle(frameIndex, 0, 32, 32),
                Color.White,
                0,
                new Vector2(16, 16),
                SpriteEffects.None,
                0
            );

        }
    }
}

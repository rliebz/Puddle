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
        public bool pushLeft;
        public bool pushRight;
        public bool gravity;

        public bool rCol;
        public bool lCol;
        public bool dCol;
        public bool uCol;

        public double x_vel;
		public double y_vel;

        public string blockType;

        private Block uBlock;

        public Block(int x, int y, bool left=false, bool right=false, bool gravity=false)
            : base(x, y, 32, 32)
        {

            this.pushLeft = left;
            this.pushRight = right;
            this.gravity = gravity;
       		
			this.isSolid = true;

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

            this.pushLeft = (obj.Properties.ContainsKey("left")) ? Boolean.Parse(obj.Properties["left"]) : false;
            this.pushRight = (obj.Properties.ContainsKey("right")) ? Boolean.Parse(obj.Properties["right"]) : false;
            this.gravity = (obj.Properties.ContainsKey("gravity")) ? Boolean.Parse(obj.Properties["gravity"]) : false;

            this.blockType = "push";
            this.name = obj.Name;

			this.isSolid = true;

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
            else if (pushRight && !pushLeft)
                frameIndex = 0;
            else if (pushLeft && !pushRight)
                frameIndex = 32;
            else if (pushLeft && pushRight)
                frameIndex = 64;
            else
                frameIndex = 96;
        }

        public void changeType(string newType)
        {
            this.blockType = newType;
            image = images[newType];
            if (newType == "metal")
            {
                gravity = false;
                frameIndex = 0;
            }
            else
            {
                gravity = true;

                if (pushRight && !pushLeft)
                    frameIndex = 0;
                else if (pushLeft && !pushRight)
                    frameIndex = 32;
                else if (pushLeft && pushRight)
                    frameIndex = 64;
                else
                    frameIndex = 96;
            }
        }

        public override void Update(Physics physics)
        {
            Move(physics);

            CheckCollisions(physics);

        }

        public void Move(Physics physics)
        {
            // Gravity
            if (gravity)
            {
				y_vel += physics.gravity;
				if (y_vel > physics.maxFallSpeed)
					y_vel = physics.maxFallSpeed;
				spriteY += Convert.ToInt32(y_vel);
            }

            // Move sideways
            spriteX += Convert.ToInt32(x_vel);
            if (uCol)
            {
                uBlock.x_vel = x_vel;
                uBlock.Move(physics);
            }

			x_vel = 0;
        }

        public void CheckCollisions(Physics physics)
        {
            // Assume no collisions
            rCol = false;
            lCol = false;
            dCol = false;
            uCol = false;

			if (blockType != "push")
				return;

            // Check collisions with other blocks
			foreach (Sprite s in physics.items)
            {
				if (this != s && s.isSolid && Intersects(s))
                {
					// Collide with block below
					if (spriteY < s.spriteY && bottomWall >= s.topWall) 
					{
						dCol = true;
						y_vel = 0;
						while (bottomWall >= s.topWall)
							spriteY--;
					}

					// Collide with block on right
					else if (spriteX < s.spriteX && rightWall >= s.leftWall) 
					{
						rCol = true;
						while (Intersects(s))
							spriteX--;
						while (Intersects(physics.player))
							physics.player.spriteX--;
					}

					// Collide with block on left
					else if (spriteX > s.spriteX && leftWall <= s.rightWall) 
					{
						lCol = true;
						while (Intersects(s))
							spriteX++;
						while (Intersects(physics.player))
							physics.player.spriteX++;
					}
                }
            }
        }

		public override void LoadContent(ContentManager content)
        {
            images["push"] = content.Load<Texture2D>("push_block.png");
            images["metal"] = content.Load<Texture2D>("brick.png");
            image = images[this.blockType];
        }

		public override void Draw(SpriteBatch sb)
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

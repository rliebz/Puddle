using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

namespace Puddle
{
    abstract class Sprite
    {
        public int spriteX, spriteY;
        public int spriteWidth, spriteHeight;
        public bool destroyed;
        public string imageFile;
        public int sizeX;
        public int sizeY;
        protected Texture2D image;
        protected Dictionary<string, Texture2D> images;

        public Sprite(int x, int y, int width, int height)
        {
            this.spriteX = x;
            this.spriteY = y;
            this.spriteWidth = width;
            this.spriteHeight = height;
            this.sizeX = width;
            this.sizeY = height;
            this.imageFile = "bubble.png";
            this.images = new Dictionary<string, Texture2D>();
        }

        // Properties
        public bool offScreen
        {
            get { return (spriteX < -32 || spriteY < -32 || 
                spriteX > 1000 || spriteY > 1000); }
        }

        public int leftWall
        {
            get { return spriteX - sizeX / 2; }
        }

        public int rightWall
        {
            get { return spriteX + sizeX / 2 - 1; }
        }

        public int topWall
        {
            get { return spriteY - sizeY / 2; }
        }

        public int bottomWall
        {
            get { return spriteY + sizeY / 2 - 1; }
        }

        public bool Intersects(Sprite s)
        {
            bool intersect_vertical = ( (topWall >= s.topWall && topWall <= s.bottomWall) || 
                (bottomWall >= s.topWall && bottomWall <= s.bottomWall) );
            bool intersect_horizontal = ((leftWall >= s.leftWall && leftWall <= s.rightWall) ||
                (rightWall >= s.leftWall && rightWall <= s.rightWall));

            return intersect_vertical && intersect_horizontal;
        }

        public void Update(Physics physics)
        {

        }

        public void LoadContent(ContentManager content)
        {
            image = content.Load<Texture2D>(imageFile);
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(
                image, 
                new Rectangle(spriteX, spriteY, spriteWidth, spriteHeight), 
                new Rectangle(0, 0, 32, 32),
                Color.White,
                0,
                new Vector2(spriteWidth / 2, spriteHeight / 2),
                SpriteEffects.None,
                0
            );
        }


    }
}

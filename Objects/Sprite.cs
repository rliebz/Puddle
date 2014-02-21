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
        protected Texture2D image;
        public string imageFile;

        public Sprite(int x, int y, int width, int height)
        {
            this.spriteX = x;
            this.spriteY = y;
            this.spriteWidth = width;
            this.spriteHeight = height;
            this.imageFile = "bubble.png";
        }

        public int getX(){
            return spriteX;
        }
        public int getY()
        {
            return spriteY;
        }
        public void setX(int x)
        {
            spriteX = x;
        }
        public void setY(int y)
        {
            spriteY = y;
        }

        // Properties
        public bool offscreen() 
        {
            return (spriteX < -32 || spriteY < -32 || spriteX > 800 || spriteY > 330);
        }

        public void LoadContent(ContentManager content)
        {
            image = content.Load<Texture2D>(imageFile);
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(image, new Rectangle(spriteX, spriteY, spriteWidth, spriteHeight), Color.White);
        }


    }
}

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Audio;

namespace Puddle
{
    abstract class Sprite
    {
        public int spriteX, spriteY;
        public int spriteWidth, spriteHeight;
        public int collisionWidth, collisionHeight;
        public int frameWidth, frameHeight;
        public int frameIndex;
        public bool destroyed;
        public bool faceLeft;
        public string imageFile;
        protected Texture2D image;
        protected Dictionary<string, Texture2D> images;
        protected List<string> soundFiles;
        protected Dictionary<string, SoundEffect> soundList;

        public Sprite(int x, int y, int width, int height)
        {
            this.spriteX = x + 16;
            this.spriteY = y + 16;
            this.spriteWidth = width;
            this.spriteHeight = height;
            this.frameWidth = 32;
            this.frameHeight = 32;
            this.collisionWidth = width;
            this.collisionHeight = height;
            this.imageFile = "blank.png";
            this.images = new Dictionary<string, Texture2D>();
            this.faceLeft = false;
            this.frameIndex = 0;
            this.soundFiles = new List<string>();
            this.soundList = new Dictionary<string, SoundEffect>();
        }

        // Properties
        public bool offScreen
        {
            get { return (spriteX < -32 || spriteY < -32 || 
                spriteX > 1000 || spriteY > 1000); }
        }

        public int leftWall
        {
            get { return spriteX - collisionWidth / 2; }
        }

        public int rightWall
        {
            get { return spriteX + collisionWidth / 2 - 1; }
        }

        public int topWall
        {
            get { return spriteY - collisionHeight / 2; }
        }

        public int bottomWall
        {
            get { return spriteY + collisionHeight / 2 - 1; }
        }

        public bool Intersects(Sprite s)
        {
            bool intersect_vertical = ( (topWall >= s.topWall && topWall <= s.bottomWall) || 
                (bottomWall >= s.topWall && bottomWall <= s.bottomWall) );
            bool intersect_horizontal = ((leftWall >= s.leftWall && leftWall <= s.rightWall) ||
                (rightWall >= s.leftWall && rightWall <= s.rightWall));

            return intersect_vertical && intersect_horizontal;
        }

        public virtual void Update(Physics physics)
        { }

        public virtual void Update(Physics physics, ContentManager content)
        { }

        public virtual void LoadContent(ContentManager content)
        {
            image = content.Load<Texture2D>(imageFile);
            foreach (string file in soundFiles)
            {
                if (!soundList.ContainsKey(file))
                {
                    SoundEffect effect = content.Load<SoundEffect>(file);
                    soundList.Add(file, effect);
                }               
            }
        }

        public virtual void Draw(SpriteBatch sb)
        {
            sb.Draw(
                image,
                new Rectangle(spriteX, spriteY, spriteWidth, spriteHeight),
                new Rectangle(frameIndex, 0, frameWidth, frameHeight),
                Color.White,
                0,
                new Vector2(spriteWidth / 2, spriteHeight / 2),
                faceLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0
            );
        }


    }
}

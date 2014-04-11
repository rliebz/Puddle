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
        public int depth;
        public bool destroyed;
        public bool faceLeft;
		public bool isSolid;
        public string imageFile;
		public string name;
        public float rotationAngle;
        public Color spriteColor;
        protected Texture2D image;
		public Texture2D blankImage;
        protected TextField displayMessage;
        public bool isShot;
        protected int displayTextX;
        protected int displayTextY;
        protected Color displayTextColor;
        
		protected bool displayHitBox;
        protected string displayText;
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
            this.isSolid = false;
			this.displayHitBox = false;
            this.spriteColor = Color.White;
            this.rotationAngle = 0;
            this.depth = 0;
            this.displayText = "";
            this.isShot = false;
            this.displayTextX = 0;
            this.displayTextY = 0;
            this.displayTextColor = Color.White;

            this.soundFiles = new List<string>();
            this.soundList = new Dictionary<string, SoundEffect>();
        }

        // Properties
        public bool offScreen
        {
            get { return (spriteX < -32 || spriteY < -32 || 
				spriteX > 32 * 22 || spriteY > 32 * 22); }
        }

        public int CompareTo(Sprite b)
        {
            if (this.depth > b.depth)
                return 1;
            else if (this.depth < b.depth)
                return -1;
            else
                return 0;
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
            bool intersect_vertical = ( 
				(topWall >= s.topWall && topWall <= s.bottomWall) || 
				(bottomWall >= s.topWall && bottomWall <= s.bottomWall) ||
				(s.topWall >= topWall && s.topWall <= bottomWall) || 
				(s.bottomWall >= topWall && s.bottomWall <= bottomWall) 
			);

            bool intersect_horizontal = (
				(leftWall >= s.leftWall && leftWall <= s.rightWall) ||
				(rightWall >= s.leftWall && rightWall <= s.rightWall) ||
				(s.leftWall >= leftWall && s.leftWall <= rightWall) ||
				(s.rightWall >= leftWall && s.rightWall <= rightWall)
			);

            return intersect_vertical && intersect_horizontal;
        }

        public virtual void Update(Level level)
        { }

        public virtual void Update(Level level, ContentManager content)
        { }

        public virtual void LoadContent(ContentManager content)
        {
			blankImage = content.Load<Texture2D>("blank.png");
            image = content.Load<Texture2D>(imageFile);
            if (displayText != "")
            {
                displayMessage = new TextField(
                    displayText,
                    new Vector2(spriteX - displayTextX, spriteY - displayTextY),
                    displayTextColor
                );
                displayMessage.loadContent(content);
            }
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
			if (displayHitBox)
			{
				sb.Draw(
					blankImage,
					new Rectangle(leftWall, topWall, rightWall - leftWall, bottomWall - topWall),
					Color.Navy
				);
			}


            sb.Draw(
                image,
                new Rectangle(spriteX, spriteY, spriteWidth, spriteHeight),
                new Rectangle(frameIndex, 0, frameWidth, frameHeight),
                spriteColor,
                rotationAngle,
                new Vector2(spriteWidth / 2, spriteHeight / 2),
                faceLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0
            );
            if (displayText != "")
            {
                displayMessage.draw(sb);
            }
        }


    }
}

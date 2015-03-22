using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Audio;

namespace Puddle
{
    abstract class Sprite
    {
        public static int spriteSize = 32;
        public static int tileSize = 32;
        public static SpriteFont font;

        public int spriteX, spriteY;
        public double baseWidth, baseHeight;
        public double baseCollisionWidth, baseCollisionHeight;
        public int frameWidth, frameHeight;
        public int frameIndexX;
        public int frameIndexY;
        public int depth;
        public bool destroyed;
        public bool faceLeft;
		public bool isSolid;
        public string imageFile;
		public string name;
        public float rotationAngle;
        public Color spriteColor;
		public Texture2D image;
		public Texture2D blankImage;
        protected int displayTextX;
        protected int displayTextY;
        protected Color displayTextColor;
        
		protected bool displayHitBox;
        protected string displayText;
		public Dictionary<string, Texture2D> images;
		public List<string> soundFiles;
		public Dictionary<string, SoundEffect> soundList;

		public Sprite(int x, int y, double width=1, double height=1)
        {
			this.name = "";
            this.spriteX = x + spriteSize / 2; // TODO: x + spriteWidth / 2
			this.spriteY = y + spriteSize / 2; // TODO: y + spriteHeight / 2
            this.baseWidth = width;
            this.baseHeight = height;
            this.baseCollisionWidth = width;
            this.baseCollisionHeight = height;
            this.frameWidth = 32;
            this.frameHeight = 32;
            this.imageFile = "blank.png";
            this.images = new Dictionary<string, Texture2D>();
            this.faceLeft = false;
            this.frameIndexX = 0;
            this.frameIndexY = 0;
            this.isSolid = false;
			this.displayHitBox = false;
            this.spriteColor = Color.White;
            this.rotationAngle = 0;
            this.depth = 0;
            this.displayText = "";
            this.displayTextX = 0;
            this.displayTextY = 0;
            this.displayTextColor = Color.White;

            this.soundFiles = new List<string>();
            this.soundList = new Dictionary<string, SoundEffect>();
        }

        // Properties
        public int spriteWidth
        { get { return (int)(this.baseWidth * spriteSize); } }

        public int spriteHeight
        { get { return (int)(this.baseHeight * spriteSize); } }

        public int displayWidth
        { get { return (int)(this.baseWidth * tileSize); } }

        public int displayHeight
        { get { return (int)(this.baseHeight * tileSize); } }

        public int collisionWidth
        { get { return (int)(this.baseCollisionWidth * spriteSize); } }

        public int collisionHeight
        { get { return (int)(this.baseCollisionHeight * spriteSize); } }

        public bool offScreen
        {
			get 
			{ 
				return (rightWall < 0 || leftWall > spriteSize * 22 || 
						bottomWall < 0 || topWall > spriteSize * 22); 
			}
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

		public virtual int leftWall
        {
            get { return spriteX - collisionWidth / 2; }
        }

		public virtual int rightWall
        {
            get { return spriteX + collisionWidth / 2 - 1; }
        }

		public virtual int topWall
        {
            get { return spriteY - collisionHeight / 2; }
        }

		public virtual int bottomWall
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

//        public virtual void Update(Level level, ContentManager content)
//        { }

        public virtual void LoadContent(ContentManager content)
        {
			blankImage = content.Load<Texture2D>("blank.png");
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
			if (displayHitBox)
			{
				sb.Draw(
					blankImage,
					new Rectangle(
                        leftWall * tileSize / spriteSize, 
                        topWall * tileSize / spriteSize,
                        (rightWall - leftWall) * tileSize / spriteSize,
                        (bottomWall - topWall) * tileSize / spriteSize
                    ),
					Color.Navy
				);
			}


            sb.Draw(
                image,
                new Rectangle(
                    spriteX * tileSize / spriteSize, 
                    spriteY * tileSize / spriteSize, 
                    displayWidth, 
                    displayHeight
                ),
                new Rectangle(frameIndexX, frameIndexY, frameWidth, frameHeight),
                spriteColor,
                rotationAngle,
                new Vector2(spriteWidth / 2, spriteHeight / 2),
                faceLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0
            );
            if (displayText != "")
            {
                sb.DrawString(
                    font,
                    displayText,
                    new Vector2(
                        (spriteX + displayTextX) * tileSize / spriteSize,
                        (spriteY + displayTextY) * tileSize / spriteSize
                    ),
                    displayTextColor,
                    0f,
                    font.MeasureString(displayText) * 0.5f,
                    (float)Sprite.tileSize / Sprite.spriteSize,
                    SpriteEffects.None,
                    0
                );
            }
        }


    }
}

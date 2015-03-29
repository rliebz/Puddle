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
    class Menu
    {
        public string title;
        public List<string> options;
        public Texture2D image;
        public string imageFile;
        public int width;
        public int height;
        public float textSize;

        public Menu()
        {
            imageFile = "metal_block";

            title = "Puddle";
            options = new List<string>()
            {
                "Start - Resume Level",
                "Select (Pause) - Level Select",
                "",
                "Left/Right - Movement",
                "Down - Puddle", 
                "A - Jump/Jetpack",
                "Right Bumper - Shoot",
                "Right Trigger - Power Shoot",
                "Up + Shoot - Shoot Upward"
            };

            width = 14;
            height = 9;
            textSize = 1.5f;
        }

        public void LoadContent(ContentManager content)
        {
            image = content.Load<Texture2D>(imageFile);
        }

        private void DrawBlock(SpriteBatch sb, int xPos, int yPos, int frameIndexX, int frameIndexY)
        {
            sb.Draw(
                image,
                new Rectangle(xPos, yPos, Sprite.SIZE, Sprite.SIZE),
                new Rectangle(frameIndexX, frameIndexY, Sprite.SIZE, Sprite.SIZE),
                Color.Goldenrod
            );
        }

        private void DrawBackground(SpriteBatch sb, int startX, int startY)
        {
            // Top Row
            DrawBlock(
                sb,
                startX, startY, 
                96, 64
            );
            for (int i = 1; i < width - 1; i++)
            {
                DrawBlock(
                    sb,
                    startX + Sprite.SIZE * i, startY, 
                    0, 96
                );
            }
            DrawBlock(
                sb,
                startX + Sprite.SIZE * (width - 1), startY,
                0, 64
            );

            // Middle Rows
            for (int row = 1; row < height - 1; row++)
            {
                DrawBlock(
                    sb,
                    startX, startY + Sprite.SIZE * row,
                    64, 96
                );
                for (int i = 1; i < width - 1; i++)
                {
                    DrawBlock(
                        sb,
                        startX + Sprite.SIZE * i, startY + Sprite.SIZE * row,
                        32, 0
                    );
                }
                DrawBlock(
                    sb,
                    startX + Sprite.SIZE * (width - 1), startY + Sprite.SIZE * row,
                    96, 96
                );
            }

            // Bottom Row
            DrawBlock(
                sb,
                startX, startY + Sprite.SIZE * (height - 1),
                64, 64
            );
            for (int i = 1; i < width - 1; i++)
            {
                DrawBlock(
                    sb,
                    startX + Sprite.SIZE * i, startY + Sprite.SIZE * (height - 1),
                    32, 96
                );
            }
            DrawBlock(
                sb,
                startX + Sprite.SIZE * (width - 1), startY + Sprite.SIZE * (height - 1),
                32, 64
            );
        }

        public void DrawUI(SpriteBatch sb, GraphicsDeviceManager graphics, int gameScale)
        {

            int startX = graphics.PreferredBackBufferWidth / 2 / gameScale - width * Sprite.SIZE / 2;
            int startY = graphics.PreferredBackBufferHeight / 2 / gameScale - height * Sprite.SIZE / 2;

            DrawBackground(sb, startX, startY);

            // Draw Title
            sb.DrawString(
                Sprite.font,
                title,
                new Vector2(
                    graphics.PreferredBackBufferWidth / 2 / gameScale,
                    startY +  Sprite.SIZE
                ),
                Color.Black,
                0,
                Sprite.font.MeasureString(title) * 0.5f,
                textSize * 2,
                SpriteEffects.None,
                0
            );

            // Draw Options
            int count = 0;
            foreach (string option in options) {

                sb.DrawString(
                    Sprite.font,
                    option,
                    new Vector2(
                        graphics.PreferredBackBufferWidth / 2 / gameScale,
                        startY + (count + 3) * Sprite.SIZE * textSize / 2
                    ),
                    Color.Black,
                    0,
                    Sprite.font.MeasureString(option) * 0.5f,
                    textSize,
                    SpriteEffects.None,
                    0
                );
                count++;
            }
        }


    }
}

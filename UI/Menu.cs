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
        public List<string> optionsRight;
        public Texture2D image;
        public string imageFile;
        private int width;
        private int height;
        private float textSize;
        private bool centerAlign;
        private bool allowCursor;
        private int _cursor;

        public Menu(string configuration=null)
        {
            imageFile = "Textures/metal_block";
            options = new List<string>();
            optionsRight = new List<string>();
            width = 14;
            height = 9;
            centerAlign = false;
            allowCursor = false;

            LoadConfiguration(configuration);

        }

        public int cursor
        {
            get {
                return allowCursor ? _cursor : -1; 
            }
            set {
                if (allowCursor)
                {
                    _cursor = value < 0 ? options.Count - 1 : value % (options.Count);
                }
            }
        }

        public void LoadConfiguration(string configuration=null)
        {
            cursor = 0;

            switch (configuration) {
                case "controls":
                    centerAlign = false;
                    allowCursor = false;
                    textSize = 1.5f;
                    title = "Controls";
                    options = new List<string>()
                    {
                        "Pause",
                        "Movement",
                        "Puddle", 
                        "Jump",
                        "Jetpack",
                        "Shoot",
                        "Power Shoot"
                    };
                    optionsRight = new List<string>()
                    {
                        "Start",
                        "Left/Right",
                        "Down", 
                        "A",
                        "Hold A",
                        "R. Bumper",
                        "R. Trigger"
                    };
                    break;

                case "credits":
                    centerAlign = true;
                    allowCursor = false;
                    textSize = 2;
                    title = "Credits";
                    options = new List<string>()
                    {
                        "Robert Liebowitz",
                        "Jacob Rosenberg",
                        "Trevor Barnard", 
                        "Gift Sinthong",
                        "Alan Hua"
                    };
                    optionsRight.Clear();
                    break;

                case "confirmExit":
                    centerAlign = false;
                    allowCursor = true;
                    textSize = 2f;
                    title = "Really?";
                    options = new List<string>()
                    {
                        "Really Exit",
                        "Back To Menu"
                    };
                    optionsRight.Clear();
                    break;

                default:
                    centerAlign = false;
                    allowCursor = true;
                    textSize = 2.25f;
                    title = "Puddle";
                    options = new List<string>()
                    {
                        "Return",
                        "Controls",
                        "Level Select",
                        "Credits",
                        "Exit Game"
                    };
                    optionsRight.Clear();
                    break;
           }
        }

        // Return false to close menu
        public bool ExecuteAction(Game1 game, Player player, Level level)
        {
            if (cursor == -1)
                return true;

            string action = options[cursor];

            switch (action)
            {
                case "Return":
                    // TODO: Lingering input from selection key
                    return false;

                case "Controls":
                    LoadConfiguration("controls");
                    return true;

                case "Level Select":
                    // Can only return from inside a world
                    if (level.name.Equals("Select") || level.name.Equals("Menu"))
                        return true;
                    // Reset powerup from that world to prevent content skipping
                    if (player.worldPowerUp != null)
                        player.powerup[player.worldPowerUp] = true;
                    // Go to new level
                    player.newMap = "Select";
                    return false;

                case "Credits":
                    LoadConfiguration("credits");
                    return true;

                case "Really Exit":
                    game.Exit();
                    return true;

                case "Back To Menu":
                    LoadConfiguration();
                    return true;

                case "Exit Game":
                    LoadConfiguration("confirmExit");
                    return true;

                default:
                    return true;
            }
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
                    startY + Sprite.SIZE * textSize / 2
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

                string prefix = (cursor == count) ? ">" : "";
                Vector2 stringOrigin = Sprite.font.MeasureString(option) * 0.5f;

                sb.DrawString(
                    Sprite.font,
                    prefix + option,
                    new Vector2(
                        centerAlign ? graphics.PreferredBackBufferWidth / 2 / gameScale : startX + Sprite.SIZE,
                        startY + (count + 3) * Sprite.SIZE * textSize / 2
                    ),
                    Color.Black,
                    0,
                    new Vector2(centerAlign ? stringOrigin.X : 0, stringOrigin.Y),
                    textSize,
                    SpriteEffects.None,
                    0
                );
                count++;
            }
            // Draw OptionsRight
            if (optionsRight.Count > 0)
            {
                count = 0;
                foreach (string option in optionsRight)
                {
                    Vector2 stringOrigin = Sprite.font.MeasureString(option) * 0.5f;

                    sb.DrawString(
                        Sprite.font,
                        option,
                        new Vector2(
                            startX + (width - 1 ) * Sprite.SIZE,
                            startY + (count + 3) * Sprite.SIZE * textSize / 2
                        ),
                        Color.Black,
                        0,
                        new Vector2(stringOrigin.X * 2, stringOrigin.Y),
                        textSize,
                        SpriteEffects.None,
                        0
                    );
                    count++;
                }
            }

        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Puddle
{
    class TextField
    {
        public SpriteFont Font { get; set; }
        public string Message { get; set; }
        public Vector2 Location { get; set; }
        public Color Color { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="msg">String that the TextField will display</param>
        /// <param name="loc">Vector2 with Coordinates where the TextField will display</param>
        /// <param name="c">Color that the Font will be in</param>
        public TextField(string msg, Vector2 loc, Color c)
        {
            Message = msg;
            Location = loc;
            Color = c;
        }

        /// <summary>
        /// Initial load of TextField
        /// Default font is Arial.  Can be changed.
        /// </summary>
        /// <param name="content">content</param>
        public void loadContent(ContentManager content)
        {
            Font = content.Load<SpriteFont>("Arial");
        }

        /// <summary>
        /// Draws with current font, string, position, and font color
        /// </summary>
        /// <param name="sb">spritebatch</param>
        public void draw(SpriteBatch sb)
        {
            sb.DrawString(Font, Message, Location, Color);
        }
    }
}
﻿using SFML.Graphics;

namespace Arkanoid_SFML.Text
{
    public class FontText
    {
        public FontText(Font font, string stringText, Color textColour)
        {
            this.Font = font;
            this.StringText = stringText;
            this.TextColour = textColour;
        }

        public float Scale = 1;

        public Font Font;

        public string StringText;

        public Color TextColour;
    }
}

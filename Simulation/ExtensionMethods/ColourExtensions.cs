using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkanoid_SFML.ExtensionMethods
{
    public static class ColourExtensions
    {
        public static Color Darken(this Color color, double amount)
        {
            var colour = new Color();
            colour.R = color.R < amount ? (byte)0 : (byte)(color.R - amount);
            colour.G = color.G < amount ? (byte)0 : (byte)(color.G - amount);
            colour.B = color.B < amount ? (byte)0 : (byte)(color.B - amount);
            colour.A = byte.MaxValue;

            return colour;
        }
    }
}

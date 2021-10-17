using System;
using System.Collections.Generic;
using System.Drawing;

namespace Cbs.Svg
{
    public static class ColourUtility
    {
        public static List<Color> StandardColour() => new() { Color.Red, Color.Green, Color.Blue, Color.Brown, Color.DarkGray, Color.Pink, Color.Purple };

        public static Color RandomColour()
        {
            List<Color> colours = StandardColour();
            int index = new Random().Next(0, colours.Count - 1);
            return colours[index];
        }
    }
}

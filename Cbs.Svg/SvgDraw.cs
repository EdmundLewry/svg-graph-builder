using Svg;
using Svg.Pathing;
using System.Drawing;

namespace Cbs.Svg
{
    public static class SvgDraw
    {
        public static SvgPath Line(PointF start, PointF end)
        {
            return new SvgPath()
            {
                PathData = new SvgPathSegmentList() {
                    new SvgMoveToSegment(start),
                    new SvgLineSegment(start, end)
                },
                Stroke = new SvgColourServer(Color.Black),
                StrokeWidth = new SvgUnit(SvgUnitType.Pixel, 2),
            };
        }

        public static SvgText Text(string text, PointF position, int fontSize)
        {
            return new SvgText(text)
            {
                X = new SvgUnitCollection { new SvgUnit(position.X) },
                Y = new SvgUnitCollection { new SvgUnit(position.Y) },
                TextAnchor = SvgTextAnchor.Middle,
                FontSize = fontSize
            };
        }

        public static SvgRectangle Rect(PointF position, Size size, Color colour)
        {
            return new SvgRectangle()
            {
                Width = size.Width,
                Height = size.Height,
                X = new SvgUnit(position.X),
                Y = new SvgUnit(position.Y),
                Fill = new SvgColourServer(colour)
            };
        }
    }
}

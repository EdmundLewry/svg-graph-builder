using Svg;
using Svg.Pathing;
using Svg.Transforms;
using System.Drawing;

namespace Cbs.Svg
{
    public static class SvgDraw
    {
        public static SvgPath Line(PointF start, PointF end, Color? colour = null)
        {
            Color strokeColour = colour ?? Color.Black;

            return new SvgPath()
            {
                PathData = new SvgPathSegmentList() {
                    new SvgMoveToSegment(start),
                    new SvgLineSegment(start, end)
                },
                Stroke = new SvgColourServer(strokeColour),
                StrokeWidth = new SvgUnit(SvgUnitType.Pixel, 2),
            };
        }

        public static SvgText Text(string text, PointF position, int fontSize, Color? colour = null)
        {
            Color strokeColour = colour ?? Color.Black;

            return new SvgText(text)
            {
                X = new SvgUnitCollection { new SvgUnit(position.X) },
                Y = new SvgUnitCollection { new SvgUnit(position.Y) },
                TextAnchor = SvgTextAnchor.Middle,
                FontSize = fontSize,
                Fill = new SvgColourServer(strokeColour)
            };
        }
        
        public static SvgText VerticalText(string text, PointF position, int fontSize, Color? colour = null)
        {
            Color strokeColour = colour ?? Color.Black;

            return new SvgText(text)
            {
                TextAnchor = SvgTextAnchor.Middle,
                FontSize = fontSize,
                Fill = new SvgColourServer(strokeColour),
                Transforms = new SvgTransformCollection()
                {
                    new SvgTranslate(position.X, position.Y),
                    new SvgRotate(90)
                }
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

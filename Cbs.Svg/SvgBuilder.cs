using Svg;
using System.Drawing;

namespace Cbs.Svg
{
    public class SvgBuilder
    {
        public SvgDocument Document { get; }

        public SvgBuilder(float width, float height)
        {
            Document = new SvgDocument
            {
                Width = width,
                Height = height
            };
        }

        public void DrawLine(PointF start, PointF end, Color? colour = null)
        {
            SvgPath line = SvgDraw.Line(start, end, colour);

            Document.Children.Add(line);
        }

        public void DrawText(string text, PointF position, int fontSize, Color? colour = null)
        {
            SvgText svgText = SvgDraw.Text(text, position, fontSize, colour);

            Document.Children.Add(svgText);
        }
        
        public void DrawVerticalText(string text, PointF position, int fontSize, Color? colour = null)
        {
            SvgText svgText = SvgDraw.VerticalText(text, position, fontSize, colour);

            Document.Children.Add(svgText);
        }

        public void DrawRect(PointF position, Size size, Color colour)
        {
            SvgRectangle svgRectangle = SvgDraw.Rect(position, size, colour);

            Document.Children.Add(svgRectangle);
        }
    }

}

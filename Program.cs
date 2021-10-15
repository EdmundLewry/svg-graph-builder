using Svg;
using Svg.Pathing;
using Svg.Transforms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace svg_graph_builder
{
    public static class Program
    {
        private const string OutputFile = "graph.svg";
        static void Main(string[] args)
        {
            /* Take a yaml file as argument
             * For each graph in yaml file
             * If type is bar graph
             * Create title at top centre
             * Create x label at bottom centre
             * Create y label at left centre (rotated)
             * Create line from origin up
             * Create line from origin right
             * Create scale for y
             * Create scale for x
             * Add rectangle of random colour at x,0 to percentage of max height calculated from value
             * 
             */
            List<DataPoint> graphData = RetrieveGraphData();

            //Take data and construct axes data
            var axesData = CalculateAxesData(graphData);

            //With axes data construct axesGraphicalData
            int width = 1000;
            int height = 1000;
            Size canvasSize = new(width, height);

            //With graphData, axes data and axesGraphicalData construct graph image
            SvgDocument svg = BuildGraphImage(canvasSize, graphData, axesData);

            OutputSvg(svg);
        }

        private static List<DataPoint> RetrieveGraphData()
        {
            return new List<DataPoint>()
            {
                new DataPoint("Hello", 1),
                new DataPoint("Goodbye", 2.5),
                new DataPoint("Another", 1.2),
                new DataPoint("One", 0.2)
            };
        }

        private static AxesData CalculateAxesData(List<DataPoint> graphData)
        {
            const int SCALE_COUNT = 4;

            IEnumerable<float> yAxisData = graphData.Select(datum => float.Parse(datum.Y.ToString()));
            float max = yAxisData.Max();
            max = MathF.Ceiling(max);
            float interval = max / SCALE_COUNT;

            List<float> yAxis = new List<float>();
            for (float i = 0; i <= max; i += interval)
                yAxis.Add(i);

            //Assuming a bar graph, the xAxis points are discrete data, so can just be added in order received
            List<string> xAxis = graphData.Select(datum => datum.X.ToString()).ToList();

            return new AxesData(xAxis, yAxis);
        }

        private static SvgDocument BuildGraphImage(Size canvasSize, List<DataPoint> graphData, AxesData axesData)
        {
            SvgBuilder builder = new SvgBuilder(canvasSize.Width, canvasSize.Height);

            int margin = 100;
            GraphBounds graphBounds = new GraphBounds
            {
                OriginX = new SvgUnit(SvgUnitType.Pixel, margin),
                OriginY = new SvgUnit(SvgUnitType.Pixel, canvasSize.Height - margin),
                Width = new SvgUnit(SvgUnitType.Pixel, canvasSize.Width - (2 * margin)),
                Height = new SvgUnit(SvgUnitType.Pixel, canvasSize.Height - (2 * margin))
            };

            AxesGraphicalData axesGraphicalData = CalculateAxesGraphicalData(axesData, graphBounds);

            DrawGraphAxes(builder, graphBounds, axesData, axesGraphicalData);
            DrawBars(builder, graphBounds, graphData, axesData, axesGraphicalData);

            SvgDocument svg = builder.Document;
            return svg;
        }

        private static AxesGraphicalData CalculateAxesGraphicalData(AxesData axesData, GraphBounds graphBounds)
        {
            //Based on the actual graph size, calculate the points on axes lines to draw the data
            int xSectionCount = axesData.XAxisPoints.Count + 1;
            float xSectionLength = graphBounds.Width / xSectionCount;

            List<float> xScale = new();
            for (int i = 1; i < xSectionCount; ++i)
            {
                xScale.Add(i * xSectionLength);
            }


            int ySectionCount = axesData.YAxisPoints.Count;
            float ySectionLength = graphBounds.Height / (ySectionCount-1);

            List<float> yScale = new();
            for (int i = 0; i < ySectionCount; ++i)
            {
                yScale.Add(i * ySectionLength);
            }

            return new AxesGraphicalData(xScale, yScale);
        }

        private static void DrawGraphAxes(SvgBuilder builder, GraphBounds graphBounds, AxesData data, AxesGraphicalData axesGraphicalData)
        {
            DrawXAxis(builder, graphBounds, data, axesGraphicalData.XScale);
            DrawYAxis(builder, graphBounds, data, axesGraphicalData.YScale);
        }


        private static void DrawXAxis(SvgBuilder builder, GraphBounds graphBounds, AxesData data, List<float> scale)
        {
            builder.DrawLine(new PointF(graphBounds.OriginX, graphBounds.OriginY), new PointF(graphBounds.OriginX + graphBounds.Width, graphBounds.OriginY));
            
            const float pipHeight = 5;
            const float scaleLabelMargin = 5;
            const int fontSize = 16;

            for(int i=0; i<scale.Count; ++i)
            {
                PointF pipStart = new PointF(graphBounds.OriginX + scale[i], graphBounds.OriginY);
                PointF pipEnd = new PointF(graphBounds.OriginX + scale[i], graphBounds.OriginY + pipHeight);
                builder.DrawLine(pipStart, pipEnd);

                PointF textPosition = new PointF(pipEnd.X, pipEnd.Y + scaleLabelMargin + fontSize);
                builder.DrawText(data.XAxisPoints[i], textPosition, fontSize);
            }
        }

        private static void DrawYAxis(SvgBuilder builder, GraphBounds graphBounds, AxesData data, List<float> scale)
        {
            builder.DrawLine(new PointF(graphBounds.OriginX, graphBounds.OriginY), new PointF(graphBounds.OriginX, graphBounds.OriginY - graphBounds.Height));

            const float pipWidth = 5;
            const int fontSize = 16;

            for (int i = 0; i < scale.Count; ++i)
            {
                PointF pipStart = new PointF(graphBounds.OriginX, graphBounds.OriginY - scale[i]);
                PointF pipEnd = new PointF(graphBounds.OriginX - pipWidth, graphBounds.OriginY - scale[i]);
                builder.DrawLine(pipStart, pipEnd);
                builder.DrawLine(pipStart, new PointF(graphBounds.OriginX + graphBounds.Width, pipStart.Y));

                PointF textPosition = new PointF(pipEnd.X - 20, pipEnd.Y + fontSize / 4);
                builder.DrawText(data.YAxisPoints[i].ToString(), textPosition, fontSize);
            }
        }

        private static void DrawBars(SvgBuilder builder, GraphBounds graphBounds, List<DataPoint> graphData, AxesData axesData, AxesGraphicalData axesGraphicalData)
        {
            int xInterval = (int)(axesGraphicalData.XScale[0]);
            int barWidth = xInterval / 2;
            float yMax = axesData.YAxisPoints.Max();
            for (int i=0; i<axesGraphicalData.XScale.Count; ++i)
            {
                int height = (int)Math.Round((float.Parse(graphData[i].Y.ToString()) / yMax) * graphBounds.Height);
                Size size = new Size(barWidth, height);
                PointF position = new PointF(graphBounds.OriginX + axesGraphicalData.XScale[i] - barWidth/2, graphBounds.OriginY - height - 1);
                builder.DrawRect(position, size);
            }
        }

        private static void OutputSvg(SvgDocument svg)
        {
            Console.WriteLine(svg.GetXML());
            File.WriteAllText(OutputFile, svg.GetXML());
        }
    }

    public struct GraphBounds
    {
        public SvgUnit OriginX { get; init; }
        public SvgUnit OriginY { get; init; }
        public SvgUnit Height { get; init; }
        public SvgUnit Width { get; init; }
    }

    public record DataPoint(object X, object Y);

    public record AxesData(List<string> XAxisPoints, List<float> YAxisPoints);
    public record AxesGraphicalData(List<float> XScale, List<float> YScale);

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

        public void DrawLine(PointF start, PointF end)
        {
            SvgPath line = new SvgPath()
            {
                PathData = new SvgPathSegmentList() {
                new SvgMoveToSegment(start),
                new SvgLineSegment(start, end)
            },
                Stroke = new SvgColourServer(Color.Black),
                StrokeWidth = new SvgUnit(SvgUnitType.Pixel, 2),
            };

            Document.Children.Add(line);
        }
        
        public void DrawText(string text, PointF position, int fontSize)
        {
            SvgText svgText = new SvgText(text)
            {
                X = new SvgUnitCollection { new SvgUnit(position.X) },
                Y = new SvgUnitCollection { new SvgUnit(position.Y) },
                TextAnchor = SvgTextAnchor.Middle,
                FontSize = fontSize
            };

            Document.Children.Add(svgText);
        }

        public void DrawRect(PointF position, Size size)
        {
            SvgRectangle svgRectangle = new SvgRectangle()
            {
                Width = size.Width,
                Height = size.Height,
                X = new SvgUnit(position.X),
                Y = new SvgUnit(position.Y),
                Fill = new SvgColourServer(Color.Red)
            };

            Document.Children.Add(svgRectangle);
        }
    }

}

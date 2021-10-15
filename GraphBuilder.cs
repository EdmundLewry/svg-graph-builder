using Svg;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Cbs.Svg;

namespace svg_graph_builder
{
    public static class GraphBuilder
    {
        public static SvgDocument Build(int width, int height, List<DataPoint> graphData)
        {
            AxesData axesData = CalculateAxesData(graphData);

            Size canvasSize = new(width, height);
            SvgDocument svg = BuildGraphImage(canvasSize, graphData, axesData);

            return svg;
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
            float ySectionLength = graphBounds.Height / (ySectionCount - 1);

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

            for (int i = 0; i < scale.Count; ++i)
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
            Color colour = ColourUtility.RandomColour();
            for (int i = 0; i < axesGraphicalData.XScale.Count; ++i)
            {
                int height = (int)Math.Round((float.Parse(graphData[i].Y.ToString()) / yMax) * graphBounds.Height);
                Size size = new Size(barWidth, height);
                PointF position = new PointF(graphBounds.OriginX + axesGraphicalData.XScale[i] - barWidth / 2, graphBounds.OriginY - height - 1);
                builder.DrawRect(position, size, colour);
            }
        }
    }
}

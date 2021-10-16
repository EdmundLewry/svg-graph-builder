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
        public static SvgDocument Build(int width, int height, Graph graph)
        {
            AxesData axesData = CalculateAxesData(graph.Data);

            Size canvasSize = new(width, height);
            SvgDocument svg = BuildGraphImage(canvasSize, graph, axesData);

            return svg;
        }

        private static AxesData CalculateAxesData(List<GraphDatum> graphData)
        {
            List<float> yAxis = CaclculateYAxis(graphData);
            List<string> xAxis = CalculateXAxis(graphData);

            return new AxesData(xAxis, yAxis);
        }

        private static List<float> CaclculateYAxis(List<GraphDatum> graphData)
        {
            float max = FindMaxYScale(graphData);
            float interval = CalculateYScaleInterval(max);

            List<float> yAxis = new List<float>();
            for (float i = 0; i <= max; i += interval)
                yAxis.Add(i);

            return yAxis;
        }

        private static float FindMaxYScale(List<GraphDatum> graphData)
        {
            IEnumerable<float> yAxisData = graphData.Select(datum => float.Parse(datum.Y.ToString()));
            float max = yAxisData.Max();
            max = MathF.Ceiling(max);
            return max;
        }
        private static float CalculateYScaleInterval(float max)
        {
            const int SCALE_COUNT = 4;
            float interval = max / SCALE_COUNT;
            return interval;
        }

        //Assuming a bar graph, the xAxis points are discrete data, so can just be added in order received
        private static List<string> CalculateXAxis(List<GraphDatum> graphData) => graphData.Select(datum => datum.X.ToString()).ToList();

        private static SvgDocument BuildGraphImage(Size canvasSize, Graph graph, AxesData axesData)
        {
            SvgBuilder builder = new SvgBuilder(canvasSize.Width, canvasSize.Height);
            
            GraphBounds graphBounds = CalculateGraphBounds(canvasSize);
            AxesGraphicalData axesGraphicalData = CalculateAxesGraphicalData(axesData, graphBounds);

            DrawGraphAxes(builder, graphBounds, axesData, axesGraphicalData);
            DrawBars(builder, graphBounds, graph.Data, axesData, axesGraphicalData);

            SvgDocument svg = builder.Document;
            return svg;
        }

        private static GraphBounds CalculateGraphBounds(Size canvasSize)
        {
            const int margin = 100;
            GraphBounds graphBounds = new GraphBounds
            {
                OriginX = new SvgUnit(SvgUnitType.Pixel, margin),
                OriginY = new SvgUnit(SvgUnitType.Pixel, canvasSize.Height - margin),
                Width = new SvgUnit(SvgUnitType.Pixel, canvasSize.Width - (2 * margin)),
                Height = new SvgUnit(SvgUnitType.Pixel, canvasSize.Height - (2 * margin))
            };
            return graphBounds;
        }

        private static AxesGraphicalData CalculateAxesGraphicalData(AxesData axesData, GraphBounds graphBounds)
        {
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
                //builder.DrawLine(pipStart, pipEnd);

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
                //builder.DrawLine(pipStart, new PointF(graphBounds.OriginX + graphBounds.Width, pipStart.Y));

                PointF textPosition = new PointF(pipEnd.X - 20, pipEnd.Y + fontSize / 4);
                builder.DrawText(data.YAxisPoints[i].ToString(), textPosition, fontSize);
            }
        }

        private static void DrawBars(SvgBuilder builder, GraphBounds graphBounds, List<GraphDatum> graphData, AxesData axesData, AxesGraphicalData axesGraphicalData)
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

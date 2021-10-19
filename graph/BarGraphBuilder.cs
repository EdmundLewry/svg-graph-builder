using Svg;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Cbs.Svg;

namespace svg_graph_builder
{
    public class BarGraphBuilder : GraphBuilder
    {
        private const float PipLength = 5;

        public BarGraphBuilder(Configuration configuration) : base(configuration) {}

        protected override AxesData CalculateAxesData(List<GraphDatum> graphData)
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
            const int SCALE_COUNT = 5;
            float interval = max / SCALE_COUNT;
            return interval;
        }

        //For a bar graph, the xAxis points are discrete data, so can just be added in order received
        private static List<string> CalculateXAxis(List<GraphDatum> graphData) => graphData.Select(datum => datum.X.ToString()).ToList();

        protected override SvgDocument BuildGraphImage(Size canvasSize, Graph graph, AxesData axesData)
        {
            SvgBuilder builder = new SvgBuilder(canvasSize.Width, canvasSize.Height);

            GraphBounds graphBounds = CalculateGraphBounds(canvasSize);
            AxesGraphicalData axesGraphicalData = CalculateAxesGraphicalData(axesData, graphBounds);

            DrawGraphAxes(builder, graphBounds, axesData, axesGraphicalData);
            DrawBars(builder, graphBounds, graph.Data, axesData, axesGraphicalData);
            DrawLabels(builder, graphBounds, graph);

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
            List<float> xScale = CalculateXAxisGraphicalData(axesData, graphBounds);
            List<float> yScale = CalculateYAxisGraphicalData(axesData, graphBounds);

            return new AxesGraphicalData(xScale, yScale);
        }

        private static List<float> CalculateXAxisGraphicalData(AxesData axesData, GraphBounds graphBounds)
        {
            int xSectionCount = axesData.XAxisPoints.Count + 1;
            float xSectionLength = graphBounds.Width / xSectionCount;

            List<float> xScale = new();
            for (int i = 1; i < xSectionCount; ++i)
            {
                xScale.Add(i * xSectionLength);
            }

            return xScale;
        }

        private static List<float> CalculateYAxisGraphicalData(AxesData axesData, GraphBounds graphBounds)
        {
            int ySectionCount = axesData.YAxisPoints.Count;
            float ySectionLength = graphBounds.Height / (ySectionCount - 1);

            List<float> yScale = new();
            for (int i = 0; i < ySectionCount; ++i)
            {
                yScale.Add(i * ySectionLength);
            }

            return yScale;
        }

        private void DrawGraphAxes(SvgBuilder builder, GraphBounds graphBounds, AxesData data, AxesGraphicalData axesGraphicalData)
        {
            DrawYAxis(builder, graphBounds, data, axesGraphicalData.YScale);
            DrawXAxis(builder, graphBounds, data, axesGraphicalData.XScale);
        }

        private void DrawXAxis(SvgBuilder builder, GraphBounds graphBounds, AxesData data, List<float> scale)
        {
            DrawXAxisLine(builder, graphBounds);

            for (int i = 0; i < scale.Count; ++i)
            {
                DrawXAxisScaleElement(builder, graphBounds, data.XAxisPoints[i], scale[i]);
            }
        }

        private static void DrawXAxisLine(SvgBuilder builder, GraphBounds graphBounds)
        {
            builder.DrawLine(new PointF(graphBounds.OriginX, graphBounds.OriginY), new PointF(graphBounds.OriginX + graphBounds.Width, graphBounds.OriginY));
        }

        private void DrawXAxisScaleElement(SvgBuilder builder, GraphBounds graphBounds, string scaleLabel, float scaleOffset)
        {
            const float scaleLabelMargin = 5;
            
            PointF pipEnd = new PointF(graphBounds.OriginX + scaleOffset, graphBounds.OriginY + PipLength);
            PointF textPosition = new PointF(pipEnd.X, pipEnd.Y + scaleLabelMargin + Configuration.FontSize);

            builder.DrawText(scaleLabel, textPosition, Configuration.FontSize);
        }

        private void DrawYAxis(SvgBuilder builder, GraphBounds graphBounds, AxesData data, List<float> scale)
        {
            DrawYAxisLine(builder, graphBounds);

            for (int i = 0; i < scale.Count; ++i)
            {
                DrawYAxisScaleElement(builder, graphBounds, data.YAxisPoints[i].ToString(), scale[i]);
            }
        }

        private static void DrawYAxisLine(SvgBuilder builder, GraphBounds graphBounds)
        {
            builder.DrawLine(new PointF(graphBounds.OriginX, graphBounds.OriginY), new PointF(graphBounds.OriginX, graphBounds.OriginY - graphBounds.Height));
        }

        private void DrawYAxisScaleElement(SvgBuilder builder, GraphBounds graphBounds, string scaleLabel, float scaleHeight)
        {
            int centerOffset = Configuration.FontSize / 4;
            const int textOffset = 20;

            PointF pipStart = new PointF(graphBounds.OriginX, graphBounds.OriginY - scaleHeight);
            PointF pipEnd = new PointF(graphBounds.OriginX - PipLength, graphBounds.OriginY - scaleHeight);
            PointF textPosition = new PointF(pipEnd.X - textOffset, pipEnd.Y + centerOffset);

            builder.DrawLine(pipStart, pipEnd);
            builder.DrawLine(pipStart, new PointF(graphBounds.OriginX + graphBounds.Width, pipStart.Y), Color.LightGray);
            builder.DrawText(scaleLabel, textPosition, Configuration.FontSize);
        }

        private void DrawBars(SvgBuilder builder, GraphBounds graphBounds, List<GraphDatum> graphData, AxesData axesData, AxesGraphicalData axesGraphicalData)
        {
            float yMax = axesData.YAxisPoints.Max();
            BarGraphicsData commonBarData = ConstructCommonBarData(axesGraphicalData);

            for (int i = 0; i < axesGraphicalData.XScale.Count; ++i)
            {
                string value = graphData[i].Y.ToString();
                float scaleOffset = axesGraphicalData.XScale[i];

                BarGraphicsData barData = ConstructBarData(graphBounds, scaleOffset, yMax, commonBarData, value);
                DrawBar(builder, value, barData);
            }
        }

        private static BarGraphicsData ConstructCommonBarData(AxesGraphicalData axesGraphicalData)
        {
            int xInterval = (int)(axesGraphicalData.XScale[0]);
            int barWidth = xInterval / 2;
            Color colour = ColourUtility.RandomColour();

            BarGraphicsData commonBarData = new BarGraphicsData(0, 0, barWidth, 0, colour);
            return commonBarData;
        }

        private static BarGraphicsData ConstructBarData(GraphBounds graphBounds, float scaleOffset, float yMax, BarGraphicsData commonBarData,  string value)
        {
            int height = CalculateBarHeight(graphBounds, yMax, value);
            float x = graphBounds.OriginX + scaleOffset - commonBarData.BarWidth / 2;
            float y = graphBounds.OriginY - height;

            BarGraphicsData barData = commonBarData with { X = x, Y = y, BarHeight = height };
            return barData;
        }

        private void DrawBar(SvgBuilder builder, string value, BarGraphicsData barData)
        {
            Size size = new Size(barData.BarWidth, barData.BarHeight);
            PointF position = new PointF(barData.X, barData.Y);
            SvgElement barElement = SvgDraw.Rect(position, size, barData.Colour);
            barElement.CustomAttributes.Add("class", "bar");
            builder.DrawElement(barElement);

            int fontSize = Configuration.H3FontSize;
            PointF labelPosition = new PointF(position.X + barData.BarWidth / 2, position.Y + fontSize);
            builder.DrawText(value, labelPosition, fontSize, Color.White);
        }

        private static int CalculateBarHeight(GraphBounds graphBounds, float yMax, string value)
        {
            return (int)Math.Round((float.Parse(value) / yMax) * graphBounds.Height);
        }
    }
}

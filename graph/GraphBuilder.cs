using Svg;
using System.Collections.Generic;
using System.Drawing;
using Cbs.Svg;

namespace svg_graph_builder
{
    public abstract class GraphBuilder
    {
        public Configuration Configuration { get; }

        protected GraphBuilder(Configuration configuration)
        {
            Configuration = configuration;
        }

        public SvgDocument Build(int width, int height, Graph graph)
        {
            //Configuration class as constructor argument to hold font sizes

            AxesData axesData = CalculateAxesData(graph.Data);

            Size canvasSize = new(width, height);
            SvgDocument svg = BuildGraphImage(canvasSize, graph, axesData);

            return svg;
        }

        protected abstract AxesData CalculateAxesData(List<GraphDatum> graphData);
        protected abstract SvgDocument BuildGraphImage(Size canvasSize, Graph graph, AxesData axesData);

        protected void DrawLabels(SvgBuilder builder, GraphBounds graphBounds, Graph graph)
        {
            DrawTitle(builder, graphBounds, graph.Title);
            DrawXLabel(builder, graphBounds, graph.XAxisLabel);
            DrawYLabel(builder, graphBounds, graph.YAxisLabel);
        }

        private void DrawTitle(SvgBuilder builder, GraphBounds graphBounds, string title)
        {
            int fontSize = Configuration.H1FontSize;
            PointF position = new PointF(graphBounds.OriginX + graphBounds.Width / 2, fontSize + 10);
            builder.DrawText(title, position, fontSize);
        }

        private void DrawXLabel(SvgBuilder builder, GraphBounds graphBounds, string xAxisLabel)
        {
            int fontSize = Configuration.H2FontSize;
            PointF position = new PointF(graphBounds.OriginX + graphBounds.Width / 2, graphBounds.OriginY + 10 + (fontSize * 2));
            builder.DrawText(xAxisLabel, position, fontSize);
        }

        private void DrawYLabel(SvgBuilder builder, GraphBounds graphBounds, string yAxisLabel)
        {
            int fontSize = Configuration.H2FontSize;
            PointF position = new PointF(fontSize, graphBounds.OriginX + graphBounds.Width / 2);
            builder.DrawVerticalText(yAxisLabel, position, fontSize);
        }
    }
}

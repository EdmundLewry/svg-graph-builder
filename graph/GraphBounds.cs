using Svg;

namespace svg_graph_builder
{
    public struct GraphBounds
    {
        public SvgUnit OriginX { get; init; }
        public SvgUnit OriginY { get; init; }
        public SvgUnit Height { get; init; }
        public SvgUnit Width { get; init; }
    }

}

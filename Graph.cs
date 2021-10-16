using System.Collections.Generic;

namespace svg_graph_builder
{
    public struct GraphCollection
    {
        public List<Graph> Graphs { get; init; }
    }

    public struct Graph
    {
        public string Type { get; init; }
        public string Title {get; init; }
        public string XAxisLabel {get; init; }
        public string YAxisLabel {get; init; }
        public List<GraphDatum> Data {get; init; }
    }

    public struct GraphDatum
    {
        public object X { get; init; }
        public object Y { get; init; }
    }
}
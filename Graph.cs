using System.Collections.Generic;

namespace svg_graph_builder
{
    public record GraphCollection(List<Graph> Graphs);
    public record Graph(string Type, string Title, string XAxisLabel, string YAxisLabel, List<GraphDatum> Data);
    public record GraphDatum(object X, object Y);
}
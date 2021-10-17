using System;

namespace svg_graph_builder
{
    public static class GraphBuilderFactory
    {
        public static GraphBuilder Create(string type, Configuration configuration)
        {
            return type switch
            {
                "bar" => new BarGraphBuilder(configuration),
                _ => throw new NotImplementedException()
            };
        }
    }
}

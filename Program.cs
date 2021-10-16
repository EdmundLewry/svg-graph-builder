using Svg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace svg_graph_builder
{
    public static class Program
    {
        private const string OutputFile = "graph.svg";
        static void Main(string[] args)
        {
            if (args.Length == 0)
                return;

            string filename = args[0];
            int width = GetIArgOrDefault(args, 1, 1000);
            int height = GetIArgOrDefault(args, 2, 1000);

            List<Graph> graphs = RetrieveGraphData(filename);

            graphs.ForEach(graph =>
            {
                GenerateGraphFile(width, height, graph);
            });
        }

        private static int GetIArgOrDefault(string[] args, int index, int defaultValue)
        {
            if (index >= args.Length)
                return defaultValue;

            bool success = int.TryParse(args[index], out int value);
            return success ? value : defaultValue;
        }

        private static List<Graph> RetrieveGraphData(string filename)
        {
            string text = File.ReadAllText(filename);
            var serializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            GraphCollection graphCollection = serializer.Deserialize<GraphCollection>(text);

            return graphCollection.Graphs.ToList();
        }

        private static void GenerateGraphFile(int width, int height, Graph graph)
        {
            SvgDocument svg = GraphBuilder.Build(width, height, graph);

            OutputSvg(svg);
        }

        private static void OutputSvg(SvgDocument svg)
        {
            Console.WriteLine(svg.GetXML());
            File.WriteAllText(OutputFile, svg.GetXML());
        }
    }
}

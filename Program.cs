using Svg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace svg_graph_builder
{
    public static class Program
    {
        private const string OutputFile = "graph.svg";
        private const string ConfigurationFile = "config.yaml";

        static void Main(string[] args)
        {
            if (args.Length == 0)
                return;
            
            Configuration configuration = ReadConfiguration();

            string filename = args[0];
            int width = GetIArgOrDefault(args, 1, configuration.DefaultWidth);
            int height = GetIArgOrDefault(args, 2, configuration.DefaultHeight);

            List<Graph> graphs = RetrieveGraphData(filename);

            graphs.ForEach(graph =>
            {
                GenerateGraphFile(width, height, graph, configuration);
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
            GraphCollection graphCollection = ReadYamlFile<GraphCollection>(filename);
            return graphCollection.Graphs.ToList();
        }

        private static Configuration ReadConfiguration()
        {
            if (File.Exists(ConfigurationFile))
                return ReadYamlFile<Configuration>(ConfigurationFile);

            return Configuration.Default();
        }

        private static T ReadYamlFile<T>(string filename)
        {
            string text = File.ReadAllText(filename);
            return YamlSerialization.DeserializeObject<T>(text);
        }

        private static void GenerateGraphFile(int width, int height, Graph graph, Configuration configuration)
        {
            GraphBuilder builder = GraphBuilderFactory.Create(graph.Type, configuration);
            SvgDocument svg = builder.Build(width, height, graph);

            OutputSvg(svg, graph.Title);
        }

        private static void OutputSvg(SvgDocument svg, string filename)
        {
            filename = filename.Replace(" ", "_");
            string prefix = DateTimeOffset.UtcNow.ToString("ddMMyy");
            Console.WriteLine(svg.GetXML());
            File.WriteAllText($"{prefix}-{filename}-{OutputFile}", svg.GetXML());
        }
    }
}

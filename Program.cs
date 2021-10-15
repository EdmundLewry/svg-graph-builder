using Svg;
using System;
using System.Collections.Generic;
using System.IO;

namespace svg_graph_builder
{
    public static class Program
    {
        private const string OutputFile = "graph.svg";
        static void Main(string[] args)
        {
            /* Take a yaml file as argument
             * For each graph in yaml file
             * If type is bar graph
             * Create title at top centre
             * Create x label at bottom centre
             * Create y label at left centre (rotated)
            */
            int width = 1000;
            int height = 1000;
            List<DataPoint> graphData = RetrieveGraphData();

            SvgDocument svg = GraphBuilder.Build(width, height, graphData);

            OutputSvg(svg);
        }

        private static List<DataPoint> RetrieveGraphData()
        {
            return new List<DataPoint>()
            {
                new DataPoint("Hello", 1),
                new DataPoint("Goodbye", 2.5),
                new DataPoint("Another", 1.2),
                new DataPoint("One", 0.2)
            };
        }

        private static void OutputSvg(SvgDocument svg)
        {
            Console.WriteLine(svg.GetXML());
            File.WriteAllText(OutputFile, svg.GetXML());
        }
    }
}

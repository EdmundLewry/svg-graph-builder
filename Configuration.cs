namespace svg_graph_builder
{
    public struct Configuration
    {
        public int FontSize { get; init; }
        public int H1FontSize { get; init; }
        public int H2FontSize { get; init; }
        public int H3FontSize { get; init; }

        public int DefaultHeight { get; set; }
        public int DefaultWidth { get; set; }

        public static Configuration Default() 
            => new Configuration()
            {
                H1FontSize = 32,
                H2FontSize = 24,
                H3FontSize = 20,
                FontSize = 16,
                DefaultWidth = 1000,
                DefaultHeight = 1000,
            };
    }
}

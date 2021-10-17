using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace svg_graph_builder
{
    public static class YamlSerialization
    {
        public static T DeserializeObject<T>(string yaml)
        {
            var serializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            T yamlObject = serializer.Deserialize<T>(yaml);

            return yamlObject;
        }
    }
}

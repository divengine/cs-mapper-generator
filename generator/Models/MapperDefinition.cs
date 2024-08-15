namespace Divengine.CSMapperGenerator.Models
{
    public class MapperDefinition
    {
        public string? Mapper { get; set; }
        public string? OutputFolder { get; set; }
        public string? Namespace { get; set; }
        public List<string[]>? ClassPairs { get; set; }
    }
}



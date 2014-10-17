using ElasticsearchCRUD.Mapping;

namespace ElasticsearchCRUD
{
	public class ElasticSerializationResult
	{
		public InitMappings InitMappings { get; set; }

		public string Content { get; set; }
	}
}

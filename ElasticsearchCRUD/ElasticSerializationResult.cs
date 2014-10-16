using System.Collections.Generic;
using ElasticsearchCRUD.Mapping;

namespace ElasticsearchCRUD
{
	public class ElasticSerializationResult
	{
		public List<MappingCommand> CommandsForAllEntities = new List<MappingCommand>();

		public string Content { get; set; }
	}
}

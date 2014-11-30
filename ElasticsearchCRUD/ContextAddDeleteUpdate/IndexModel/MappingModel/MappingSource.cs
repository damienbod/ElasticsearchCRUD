using System.Collections.Generic;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.MappingModel
{
	public class MappingSource
	{
		/// <summary>
		/// The _source field is an automatically generated field that stores the actual JSON that was used as the indexed document. 
		/// It is not indexed (searchable), just stored. When executing "fetch" requests, like get or search, the _source field is returned by default.
		/// Though very handy to have around, the source field does incur storage overhead within the index. For this reason, it can be disabled.
		/// {
		///	  "tweet" : {
		///		"_source" : {"enabled" : false}
		///   }
		/// }
		/// </summary>
		public bool Enabled { get; set; }

		/// <summary>
		/// {
		///    "my_type" : {
		///       "_source" : {
		///			"includes" : ["path1.*", "path2.*"],
		///			"excludes" : ["path3.*"]
		///		}
		///	}
		///}
		/// </summary>
		public List<string> Includes { get; set; }

		public List<string> Excludes { get; set; }

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			//WriteValue("number_of_shards", _numberOfShards, elasticsearchCrudJsonWriter, _numberOfShardsSet);

		}

		private void WriteValue(string key, object valueObj, ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, bool writeValue = true)
		{
			if (writeValue)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(key);
				elasticsearchCrudJsonWriter.JsonWriter.WriteValue(valueObj);
			}
		}

		private void WriteListValue(string key, List<string> valueObj, ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, bool writeValue = true)
		{
			if (writeValue)
			{
				foreach (var obj in valueObj)
				{
					//elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(key + obj.Key);
					//elasticsearchCrudJsonWriter.JsonWriter.WriteValue(obj.Value);
				}

			}
		}
	}
}
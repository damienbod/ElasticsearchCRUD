using System.Collections.Generic;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.MappingModel
{
	public class MappingSource
	{
		private bool _enabled;
		private bool _enabledSet;

		private List<string> _includes;
		private bool _includesSet;
		private List<string> _excludes;
		private bool _excludesSet;

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
		
		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				_enabled = value;
				_enabledSet = true;
			}
		}

		/// <summary>
		/// {
		///    "my_type" : {
		///       "_source" : {
		///			"includes" : ["path1.*", "path2.*"],
		///			"excludes" : ["path3.*"]
		///		}
		///	 }
		/// }
		/// </summary>
		public List<string> Includes
		{
			get { return _includes; }
			set
			{
				_includes = value;
				_includesSet = true;
			}
		}

		public List<string> Excludes
		{
			get { return _excludes; }
			set
			{
				_excludes = value;
				_excludesSet = true;
			}
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			WriteSourceEnabledIfSet(elasticsearchCrudJsonWriter);

		}

		private void WriteSourceEnabledIfSet(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			if (_enabledSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("_source");
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("enabled");
				elasticsearchCrudJsonWriter.JsonWriter.WriteValue(_enabled);
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}
			else if (_includesSet || _excludesSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("_source");
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

				WriteListValue("includes", _includes, elasticsearchCrudJsonWriter, _includesSet);
				WriteListValue("excludes", _excludes, elasticsearchCrudJsonWriter, _excludesSet);

				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}
		}

		private void WriteListValue(string key, IEnumerable<string> valueObj, ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, bool writeValue = true)
		{
			if (writeValue)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(key);
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartArray();

				foreach (var obj in valueObj)
				{
					
					elasticsearchCrudJsonWriter.JsonWriter.WriteValue(obj);
				}
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndArray();

			}
		}
	}
}
using System.Collections.Generic;
using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.CharFilters
{
	public class MappingCharFilter : AnalysisCharFilterBase
	{
		private string _mappingsPath;
		private bool _mappingsPathSet;
		private List<string> _mappings;
		private bool _mappingsSet;

		/// <summary>
		/// A char filter of type mapping replacing characters of an analyzed text with given mapping.
		/// "char_filter" : {
		///	  "my_mapping" : {
		///		"type" : "mapping",
		///		"mappings" : ["ph=>f", "qu=>k"]
		///	  }
		/// },
		/// </summary>
		/// <param name="name">name for the custom mapping char filter</param>
		public MappingCharFilter(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultCharFilters.Mapping;
		}

		/// <summary>
		/// mappings
		/// </summary>
		public List<string> Mappings
		{
			get { return _mappings; }
			set
			{
				_mappings = value;
				_mappingsSet = true;
			}
		}

		/// <summary>
		/// mappings_path
		/// </summary>
		public string MappingsPath
		{
			get { return _mappingsPath; }
			set
			{
				_mappingsPath = value;
				_mappingsPathSet = true;
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("type", Type, elasticsearchCrudJsonWriter);
			if (_mappingsSet)
			{
				JsonHelper.WriteListValue("mappings", _mappings, elasticsearchCrudJsonWriter, _mappingsSet);				
			}
			else
			{
				JsonHelper.WriteValue("mappings_path", _mappingsPath, elasticsearchCrudJsonWriter, _mappingsPathSet);
			}
			
		}
	}
}

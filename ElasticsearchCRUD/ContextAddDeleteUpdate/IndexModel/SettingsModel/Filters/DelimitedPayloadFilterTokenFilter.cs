using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
	public class DelimitedPayloadFilterTokenFilter : AnalysisFilterBase
	{
		private string _delimiter;
		private bool _delimiterSet;
		private EncodingDelimitedPayloadFilter _encoding;
		private bool _encodingSet;

		/// <summary>
		/// Named delimited_payload_filter. Splits tokens into tokens and payload whenever a delimiter character is found.
		/// Example: "the|1 quick|2 fox|3" is split per default int to tokens fox, quick and the with payloads 1, 2 and 3 respectively.
		/// </summary>
		/// <param name="name">name for the custom filter</param>
		public DelimitedPayloadFilterTokenFilter(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultTokenFilters.DelimitedPayloadFilter;
		}

		/// <summary>
		/// delimiter
		/// Character used for splitting the tokens. Default is |. 
		/// </summary>
		public string Delimiter
		{
			get { return _delimiter; }
			set
			{
				_delimiter = value;
				_delimiterSet = true;
			}
		}

		/// <summary>
		/// encoding
		/// The type of the payload. int for integer, float for float and identity for characters. Default is float. 
		/// </summary>
		public EncodingDelimitedPayloadFilter Encoding
		{
			get { return _encoding; }
			set
			{
				_encoding = value;
				_encodingSet = true;
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("type", Type, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("delimiter", _delimiter, elasticsearchCrudJsonWriter, _delimiterSet);
			JsonHelper.WriteValue("encoding", _encoding.ToString(), elasticsearchCrudJsonWriter, _encodingSet);
		}
	}

	public enum EncodingDelimitedPayloadFilter
	{
		@int,
		@float,
		identity
	}
}
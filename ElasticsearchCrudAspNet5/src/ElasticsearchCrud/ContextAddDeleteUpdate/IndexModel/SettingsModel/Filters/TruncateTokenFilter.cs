using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
	public class TruncateTokenFilter : AnalysisFilterBase
	{
		private int _length;
		private bool _lengthSet;

		/// <summary>
		/// The truncate token filter can be used to truncate tokens into a specific length. This can come in handy with keyword (single token) based mapped fields that are used for sorting in order to reduce memory usage.
		/// </summary>
		/// <param name="name">name for the custom filter</param>
		public TruncateTokenFilter(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultTokenFilters.Truncate;
		}

		/// <summary>
		/// length
		/// It accepts a length parameter which control the number of characters to truncate to, defaults to 10.
		/// </summary>
		public int Length
		{
			get { return _length; }
			set
			{
				_length = value;
				_lengthSet = true;
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("type", Type, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("length", _length, elasticsearchCrudJsonWriter, _lengthSet);
		}
	}
}
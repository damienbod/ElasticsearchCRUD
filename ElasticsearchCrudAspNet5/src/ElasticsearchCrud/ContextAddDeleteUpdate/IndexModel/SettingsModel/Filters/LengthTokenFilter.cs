using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
	public class LengthTokenFilter : AnalysisFilterBase
	{
		private int _min;
		private bool _minSet;
		private int _max;
		private bool _maxSet;

		/// <summary>
		/// A token filter of type length that removes words that are too long or too short for the stream.
		/// </summary>
		/// <param name="name">name for the custom filter</param>
		public LengthTokenFilter(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultTokenFilters.Length;
		}

		public int Min
		{
			get { return _min; }
			set
			{
				_min = value;
				_minSet = true;
			}
		}

		public int Max
		{
			get { return _max; }
			set
			{
				_max = value;
				_maxSet = true;
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("type", Type, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("min", _min, elasticsearchCrudJsonWriter, _minSet);
			JsonHelper.WriteValue("max", _min, elasticsearchCrudJsonWriter, _maxSet);
		}
	}
}
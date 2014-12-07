using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
	public class EdgeNGramTokenFilter : AnalysisFilterBase
	{
		private int _minGram;
		private bool _minGramSet;
		private int _maxGram;
		private bool _maxGramSet;
		private Side _side;
		private bool _sideSet;

		public EdgeNGramTokenFilter(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultTokenFilters.EdgeNGram;
		}

		/// <summary>
		/// min_gram Minimum size in codepoints of a single n-gram
		/// </summary>
		public int MinGram
		{
			get { return _minGram; }
			set
			{
				_minGram = value;
				_minGramSet = true;
			}
		}

		/// <summary>
		///  max_gram Maximum size in codepoints of a single n-gram
		/// </summary>
		public int MaxGram
		{
			get { return _maxGram; }
			set
			{
				_maxGram = value;
				_maxGramSet = true;
			}
		}

		/// <summary>
		/// Either front or back. Defaults to front.
		/// </summary>
		public Side Side
		{
			get { return _side; }
			set
			{
				_side = value;
				_sideSet = true;
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("type", Type, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("min_gram", _minGram, elasticsearchCrudJsonWriter, _minGramSet);
			JsonHelper.WriteValue("max_gram", _maxGram, elasticsearchCrudJsonWriter, _maxGramSet);
			JsonHelper.WriteValue("side", _side.ToString(), elasticsearchCrudJsonWriter, _sideSet);
			
		}
	}

	public enum Side
	{
		front,
		back
	}
}
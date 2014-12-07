using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
	public class NGramTokenFilter : AnalysisFilterBase
	{
		private int _minGram;
		private bool _minGramSet;
		private int _maxGram;
		private bool _maxGramSet;

		public NGramTokenFilter(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultTokenFilters.NGram;
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

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("type", Type, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("min_gram", _minGram, elasticsearchCrudJsonWriter, _minGramSet);
			JsonHelper.WriteValue("max_gram", _maxGram, elasticsearchCrudJsonWriter, _maxGramSet);
		}
	}
}
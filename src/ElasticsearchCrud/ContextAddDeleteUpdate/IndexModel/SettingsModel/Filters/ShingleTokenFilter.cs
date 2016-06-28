using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
	public class ShingleTokenFilter : AnalysisFilterBase
	{
		private int _maxShingleSize;
		private bool _maxShingleSizeSet;
		private int _minShingleSize;
		private bool _minShingleSizeSet;
		private bool _outputUnigrams;
		private bool _outputUnigramsSet;
		private bool _outputUnigramsIfNoShingles;
		private bool _outputUnigramsIfNoShinglesSet;
		private string _tokenSeparator;
		private bool _tokenSeparatorSet;
		private string _fillerToken;
		private bool _fillerTokenSet;

		/// <summary>
		/// A token filter of type shingle that constructs shingles (token n-grams) from a token stream. 
		/// In other words, it creates combinations of tokens as a single token. For example, the sentence "please divide this sentence 
		/// into shingles" might be tokenized into shingles "please divide", "divide this", "this sentence", "sentence into", and "into shingles".
		/// This filter handles position increments > 1 by inserting filler tokens (tokens with termtext "_"). 
		/// It does not handle a position increment of 0.
		/// </summary>
		/// <param name="name">name for the custom filter</param>
		public ShingleTokenFilter(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultTokenFilters.Shingle;
		}

		public int MaxShingleSize
		{
			get { return _maxShingleSize; }
			set
			{
				_maxShingleSize = value;
				_maxShingleSizeSet = true;
			}
		}

		public int MinShingleSize
		{
			get { return _minShingleSize; }
			set
			{
				_minShingleSize = value;
				_minShingleSizeSet = true;
			}
		}

		/// <summary>
		/// If true the output will contain the input tokens (unigrams) as well as the shingles. Defaults to true.
		/// </summary>
		public bool OutputUnigrams
		{
			get { return _outputUnigrams; }
			set
			{
				_outputUnigrams = value;
				_outputUnigramsSet = true;
			}
		}

		/// <summary>
		/// If output_unigrams is false the output will contain the input tokens (unigrams) if no shingles are available. 
		/// Note if output_unigrams is set to true this setting has no effect. Defaults to false.
		/// </summary>
		public bool OutputUnigramsIfNoShingles
		{
			get { return _outputUnigramsIfNoShingles; }
			set
			{
				_outputUnigramsIfNoShingles = value;
				_outputUnigramsIfNoShinglesSet = true;
			}
		}

		/// <summary>
		/// The string to use when joining adjacent tokens to form a shingle. Defaults to " ".
		/// </summary>
		public string TokenSeparator
		{
			get { return _tokenSeparator; }
			set
			{
				_tokenSeparator = value;
				_tokenSeparatorSet = true;
			}
		}

		/// <summary>
		/// The string to use as a replacement for each position at which there is no actual token in the stream. 
		/// For instance this string is used if the position increment is greater than one 
		/// when a stop filter is used together with the shingle filter. Defaults to "_"
		/// </summary>
		public string FillerToken
		{
			get { return _fillerToken; }
			set
			{
				_fillerToken = value;
				_fillerTokenSet = true;
			}
		}
		
		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("type", Type, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("max_shingle_size", _maxShingleSize, elasticsearchCrudJsonWriter, _maxShingleSizeSet);
			JsonHelper.WriteValue("min_shingle_size", _minShingleSize, elasticsearchCrudJsonWriter, _minShingleSizeSet);
			JsonHelper.WriteValue("output_unigrams", _outputUnigrams, elasticsearchCrudJsonWriter, _outputUnigramsSet);
			JsonHelper.WriteValue("output_unigrams_if_no_shingles", _outputUnigramsIfNoShingles, elasticsearchCrudJsonWriter, _outputUnigramsIfNoShinglesSet);
			JsonHelper.WriteValue("token_separator", _tokenSeparator, elasticsearchCrudJsonWriter, _tokenSeparatorSet);
			JsonHelper.WriteValue("filler_token", _fillerToken, elasticsearchCrudJsonWriter, _fillerTokenSet);
			
		}
	}
}
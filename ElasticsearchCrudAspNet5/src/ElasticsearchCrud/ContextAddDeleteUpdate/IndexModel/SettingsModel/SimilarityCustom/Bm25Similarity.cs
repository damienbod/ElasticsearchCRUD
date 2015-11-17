using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.SimilarityCustom
{
	public class Bm25Similarity : SimilarityBase
	{
		private bool _discountOverlaps;
		private bool _discountOverlapsSet;
		private double _k1;
		private bool _k1Set;
		private double _b;
		private bool _bSet;

		/// <summary>
		/// Another TF/IDF based similarity that has built-in tf normalization and is supposed to work better for short fields (like names). 
		/// See Okapi_BM25 for more details. 
		/// http://en.wikipedia.org/wiki/Okapi_BM25
		/// 
		/// The most interesting competitor to TF/IDF and the vector space model is called Okapi BM25, which is considered to be a state-of-the-art ranking function. 
		/// BM25 originates from the probabilistic relevance model, rather than the vector space model, yet the algorithm has a lot in common with Lucene’s practical scoring function.
		/// Both use of term frequency, inverse document frequency, and field-length normalization, but the definition of each of these factors is a little different. 
		/// Rather than explaining the BM25 formula in detail, we will focus on the practical advantages that BM25 offers.
		/// 
		/// http://www.elasticsearch.org/guide/en/elasticsearch/guide/current/pluggable-similarites.html
		/// https://www.found.no/foundation/similarity/
		/// </summary>
		/// <param name="name">name for the custom similarity</param>
		public Bm25Similarity(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultSimilarities.Bm25;
		}

		/// <summary>
		/// k1
		/// Controls non-linear term frequency normalization (saturation).
		/// This parameter controls how quickly an increase in term frequency results in term-frequency saturation.
		/// The default value is 1.2. Lower values result in quicker saturation, and higher values in slower saturation. 
		/// </summary>
		public double K1
		{
			get { return _k1; }
			set
			{
				_k1 = value;
				_k1Set = true;
			}
		}

		/// <summary>
		/// b
		/// Controls to what degree document length normalizes tf values.
		/// This parameter controls how much effect field-length normalization should have. 
		/// A value of 0.0 disables normalization completely, and a value of 1.0 normalizes fully. The default is 0.75. 
		/// </summary>
		public double B
		{
			get { return _b; }
			set
			{
				_b = value;
				_bSet = true;
			}
		}
		
		/// <summary>
		/// Determines whether overlap tokens (Tokens with 0 position increment) are ignored when computing norm. By default this is true, 
		/// meaning overlap tokens do not count when computing norms. 
		/// </summary>
		public bool DiscountOverlaps
		{
			get { return _discountOverlaps; }
			set
			{
				_discountOverlaps = value;
				_discountOverlapsSet = true;
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("type", Type, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("k1", _k1, elasticsearchCrudJsonWriter, _k1Set);
			JsonHelper.WriteValue("b", _b, elasticsearchCrudJsonWriter, _bSet);
			JsonHelper.WriteValue("discount_overlaps", _discountOverlaps, elasticsearchCrudJsonWriter, _discountOverlapsSet);
		}
	}
}
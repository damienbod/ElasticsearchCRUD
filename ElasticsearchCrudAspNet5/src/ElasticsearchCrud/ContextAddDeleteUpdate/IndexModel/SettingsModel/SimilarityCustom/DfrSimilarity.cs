using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.SimilarityCustom
{
	public class DfrSimilarity : SimilarityBase
	{
		private DfrBasicModel _basicModel;
		private bool _basicModelSet;
		private DfrAfterEffect _afterEffect;
		private bool _afterEffectSet;
		private DfrIbNormalization _normalization;
		private bool _normalizationSet;

		/// <summary>
		/// Similarity that implements the divergence from randomness framework.
		/// http://lucene.apache.org/core/4_1_0/core/org/apache/lucene/search/similarities/DFRSimilarity.html
		/// </summary>
		/// <param name="name"></param>
		public DfrSimilarity(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultSimilarities.Dfr;
		}

		/// <summary>
		/// basic_model
		/// Possible values: be, d, g, if, in, ine and p
		/// </summary>
		public DfrBasicModel BasicModel
		{
			get { return _basicModel; }
			set
			{
				_basicModel = value;
				_basicModelSet = true;
			}
		}

		/// <summary>
		/// after_effect
		/// Possible values: no, b and l.
		/// </summary>
		public DfrAfterEffect AfterEffect
		{
			get { return _afterEffect; }
			set
			{
				_afterEffect = value;
				_afterEffectSet = true;
			}
		}

		/// <summary>
		/// normalization
		/// Possible values: no, h1, h2, h3 and z. 
		/// </summary>
		public DfrIbNormalization Normalization
		{
			get { return _normalization; }
			set
			{
				_normalization = value;
				_normalizationSet = true;
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("type", Type, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("basic_model", _basicModel.ToString(), elasticsearchCrudJsonWriter, _basicModelSet);
			JsonHelper.WriteValue("after_effect", _afterEffect.ToString(), elasticsearchCrudJsonWriter, _afterEffectSet);
			JsonHelper.WriteValue("normalization", _normalization.ToString(), elasticsearchCrudJsonWriter, _normalizationSet);
		}
	}

	public enum DfrBasicModel
	{
		be, 
		d, 
		g, 
		@if, 
		@in, 
		ine,
		p
	}

	public enum DfrAfterEffect
	{
		no, 
		b,
		l
	}

	public enum DfrIbNormalization
	{
		no, 
		h1, 
		h2, 
		h3,
		z
	}
}
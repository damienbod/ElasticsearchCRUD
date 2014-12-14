using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.SimilarityCustom
{
	public class IbSimilarity : SimilarityBase
	{

		private DfrIbNormalization _normalization;
		private bool _normalizationSet;
		private IbLambda _lambda;
		private bool _lambdaSet;
		private IbDistribution _distribution;
		private bool _distributionSet;

		/// <summary>
		/// nformation based model
		/// http://lucene.apache.org/core/4_1_0/core/org/apache/lucene/search/similarities/IBSimilarity.html
		/// </summary>
		/// <param name="name"></param>
		public IbSimilarity(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultSimilarities.Ib;
		}

		/// <summary>
		/// distribution
		/// Possible values: ll and spl.
		/// </summary>
		public IbDistribution Distribution
		{
			get { return _distribution; }
			set
			{
				_distribution = value;
				_distributionSet = true;
			}
		}

		/// <summary>
		/// lambda
		/// Possible values: df and ttf. 
		/// </summary>
		public IbLambda Lambda
		{
			get { return _lambda; }
			set
			{
				_lambda = value;
				_lambdaSet = true;
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
			JsonHelper.WriteValue("distribution", _distribution.ToString(), elasticsearchCrudJsonWriter, _distributionSet);
			JsonHelper.WriteValue("lambda", _lambda.ToString(), elasticsearchCrudJsonWriter, _lambdaSet);
			JsonHelper.WriteValue("normalization", _normalization.ToString(), elasticsearchCrudJsonWriter, _normalizationSet);
		}
	}

	public enum IbLambda
	{
		df,
		ttf
	}

	public enum IbDistribution
	{
		ll,
		spl		
	}
}
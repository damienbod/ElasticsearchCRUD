using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.SimilarityCustom
{
	public class LmDirichletSimilarity : SimilarityBase
	{
		private int _mu;
		private bool _muSet;

		/// <summary>
		/// LM Jelinek Mercer similarity
		/// http://lucene.apache.org/core/4_7_1/core/org/apache/lucene/search/similarities/LMJelinekMercerSimilarity.html
		/// </summary>
		/// <param name="name"></param>
		public LmDirichletSimilarity(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultSimilarities.LmDirichlet;
		}

		public int Mu
		{
			get { return _mu; }
			set
			{
				_mu = value;
				_muSet = true;
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("type", Type, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("mu", _mu, elasticsearchCrudJsonWriter, _muSet);
		}
	}
}
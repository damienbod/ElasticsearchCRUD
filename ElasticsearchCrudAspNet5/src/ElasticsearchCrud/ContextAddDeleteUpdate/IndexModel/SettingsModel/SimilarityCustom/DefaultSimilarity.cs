using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.SimilarityCustom
{
	public class DefaultSimilarity : SimilarityBase
	{
		private bool _discountOverlaps;
		private bool _discountOverlapsSet;

		/// <summary>
		/// The default similarity that is based on the TF/IDF model. 
		/// </summary>
		/// <param name="name"></param>
		public DefaultSimilarity(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultSimilarities.Default;
		}

		/// <summary>
		/// discount_overlaps
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
			JsonHelper.WriteValue("discount_overlaps", _discountOverlaps, elasticsearchCrudJsonWriter, _discountOverlapsSet);
		}
	}
}
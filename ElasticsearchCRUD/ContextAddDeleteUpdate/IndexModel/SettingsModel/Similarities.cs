using System.Collections.Generic;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.SimilarityCustom;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel
{
	public class Similarities
	{
		private List<SimilarityBase> _customSimilarities;
		private bool _customSimilaritiesSet;

		public List<SimilarityBase> CustomSimilarities
		{
			get { return _customSimilarities; }
			set
			{
				_customSimilarities = value;
				_customSimilaritiesSet = true;
			}
		}

		public virtual void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			if (_customSimilaritiesSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("similarity");
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

				foreach (var item in _customSimilarities)
				{
					item.WriteJson(elasticsearchCrudJsonWriter);
				}

				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}
		}
	}
}
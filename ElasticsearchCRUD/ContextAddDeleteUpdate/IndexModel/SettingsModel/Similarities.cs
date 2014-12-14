using System.Collections.Generic;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel
{
	public class Similarities
	{
		private List<Similarity> _customSimilarities;
		private bool _customSimilaritiesSet;

		public List<Similarity> CustomSimilarities
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
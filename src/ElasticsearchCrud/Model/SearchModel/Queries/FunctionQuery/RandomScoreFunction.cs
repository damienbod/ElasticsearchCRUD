using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Queries.FunctionQuery
{
	public class RandomScoreFunction<T>: BaseScoreFunction
	{
		private readonly T _seed;

		public RandomScoreFunction(T seed)
		{
			_seed = seed;
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("random_score");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteValue("seed", _seed, elasticsearchCrudJsonWriter);		

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}

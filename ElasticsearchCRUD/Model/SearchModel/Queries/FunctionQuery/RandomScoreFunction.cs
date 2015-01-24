using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Queries.FunctionQuery
{
	public class RandomScoreFunction<T>: BaseScoreFunction
	{
		private T _seed;
		private bool _seedSet;

		public T Lang
		{
			get { return _seed; }
			set
			{
				_seed = value;
				_seedSet = true;
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("random_score");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteValue("seed", _seed, elasticsearchCrudJsonWriter, _seedSet);		

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}

using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Queries.FunctionQuery
{
	public class FunctionScoreFunction
	{
		private readonly string _function;

		public FunctionScoreFunction(string function)
		{
			_function = function;
		}

		private IFilter _filter;
		private bool _filterSet;
		private double _weight;
		private bool _weightSet;

		public IFilter Filter
		{
			get { return _filter; }
			set
			{
				_filter = value;
				_filterSet = true;
			}
		}

		public double Weight
		{
			get { return _weight; }
			set
			{
				_weight = value;
				_weightSet = true;
			}
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			if (_filterSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("filter");
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
				_filter.WriteJson(elasticsearchCrudJsonWriter);
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}
			JsonHelper.WriteValue("weight", _weight, elasticsearchCrudJsonWriter, _weightSet);

			// TODO change the function the a class
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("FUNCTION");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			elasticsearchCrudJsonWriter.JsonWriter.WriteRaw(_function);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}

	}
}
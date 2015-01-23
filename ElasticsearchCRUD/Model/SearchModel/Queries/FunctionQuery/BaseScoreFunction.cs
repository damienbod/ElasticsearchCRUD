using System;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Queries.FunctionQuery
{
	public abstract class BaseScoreFunction
	{
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

		public abstract void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter);

		protected virtual void WriteJsonBase(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, Action<ElasticsearchCrudJsonWriter> writeFunctionSpecific)
		{
			if (_filterSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("filter");
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
				_filter.WriteJson(elasticsearchCrudJsonWriter);
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}
			JsonHelper.WriteValue("weight", _weight, elasticsearchCrudJsonWriter, _weightSet);

			writeFunctionSpecific.Invoke(elasticsearchCrudJsonWriter);
		}

	}
}
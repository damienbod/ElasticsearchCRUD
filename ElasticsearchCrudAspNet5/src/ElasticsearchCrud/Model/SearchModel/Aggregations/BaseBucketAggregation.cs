using System;
using System.Collections.Generic;

namespace ElasticsearchCRUD.Model.SearchModel.Aggregations
{
	public abstract class BaseBucketAggregation : IAggs
	{
		private readonly string _type;
		private readonly string _name;
		
		private List<IAggs> _aggs;
		private bool _aggsSet;

		public BaseBucketAggregation(string type, string name)
		{
			_type = type;
			_name = name;
		}

		public List<IAggs> Aggs
		{
			get { return _aggs; }
			set
			{
				if (value.Exists(l => l.GetType() == typeof(GlobalBucketAggregation)))
				{
					throw new ElasticsearchCrudException("GlobalBucketAggregation cannot be sub aggregations");
				}
				if (value.Exists(l => l.GetType() == typeof(ReverseNestedBucketAggregation)) && GetType() !=typeof(NestedBucketAggregation))
				{
					throw new ElasticsearchCrudException("ReverseNestedBucketAggregation can only be defined in a NestedBucketAggregation");
				}
				_aggs = value;
				_aggsSet = true;
			}
		}


		public abstract void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter);

		protected virtual void WriteJsonBase(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, Action<ElasticsearchCrudJsonWriter> writeFilterSpecific)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(_name);
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(_type);
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			writeFilterSpecific.Invoke(elasticsearchCrudJsonWriter);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();

			if (_aggsSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("aggs");
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
				foreach (var item in _aggs)
				{
					item.WriteJson(elasticsearchCrudJsonWriter);
				}
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();

		}
	}
}
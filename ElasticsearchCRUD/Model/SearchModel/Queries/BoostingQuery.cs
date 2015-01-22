using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Queries
{
	/// <summary>
	/// The boosting query can be used to effectively demote results that match a given query. Unlike the "NOT" clause in bool query, 
	/// this still selects documents that contain undesirable terms, but reduces their overall score.
	/// 
	/// {
	///	 "boosting" : {
	///		"positive" : {
	///			"term" : {
	///				"field1" : "value1"
	///			}
	///		},
	///		"negative" : {
	///			"term" : {
	///				"field2" : "value2"
	///			}
	///		},
	///		"negative_boost" : 0.2
	///	 }
	/// }
	/// </summary>
	public class BoostingQuery : IQuery
	{
		private IQuery _positive;
		private bool _positiveSet;
		private IQuery _negative;
		private bool _negativeSet;
		private double _negativeBoost;
		private bool _negativeBoostSet;

		public BoostingQuery(IQuery positive, IQuery negative, double negativeBoost)
		{
			Positive = positive;
			Negative = negative;
			NegativeBoost = negativeBoost;
		}
		/// <summary>
		/// positive
		/// </summary>
		public IQuery Positive
		{
			get { return _positive; }
			set
			{
				_positive = value;
				_positiveSet = true;
			}
		}

		public IQuery Negative
		{
			get { return _negative; }
			set
			{
				_negative = value;
				_negativeSet = true;
			}
		}

		/// <summary>
		/// negative_boost
		/// </summary>
		public double NegativeBoost
		{
			get { return _negativeBoost; }
			set
			{
				_negativeBoost = value;
				_negativeBoostSet = true;
			}
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("boosting");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			if (_positiveSet)
			{

				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("positive");
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
				_positive.WriteJson(elasticsearchCrudJsonWriter);
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}

			if (_negativeSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("negative");
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
				_negative.WriteJson(elasticsearchCrudJsonWriter);
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
				
			}

			JsonHelper.WriteValue("negative_boost", _negativeBoost, elasticsearchCrudJsonWriter, _negativeBoostSet);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}

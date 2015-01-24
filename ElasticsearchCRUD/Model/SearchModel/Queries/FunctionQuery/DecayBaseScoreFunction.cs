using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Queries.FunctionQuery
{
	public abstract class DecayBaseScoreFunction<T> : BaseScoreFunction
	{
		private readonly string _field;

		/// <summary>
		/// origin
		/// The point of origin used for calculating distance. Must be given as a number for numeric field, date for date fields and geo point for geo fields. 
		/// Required for geo and numeric field. For date fields the default is now. Date math (for example now-1h) is supported for origin.
		/// </summary>
		private readonly T _origin;

		/// <summary>
		/// scale
		/// Required for all types. Defines the distance from origin at which the computed score will equal decay parameter. 
		/// For geo fields: Can be defined as number+unit (1km, 12m,…). Default unit is meters. For date fields: Can to be defined as a number+unit ("1h", "10d",…). 
		/// Default unit is milliseconds. For numeric field: Any number.
		/// </summary>
		private readonly T _scale;

		private readonly string _decayType;

		private uint _offset;
		private bool _offsetSet;
		private double _decay;
		private bool _decaySet;

		protected DecayBaseScoreFunction(string field, T origin, T scale, string decayType)
		{
			_field = field;
			_origin = origin;
			_scale = scale;
			_decayType = decayType;
		}

		/// <summary>
		/// offset
		/// If an offset is defined, the decay function will only compute the decay function for documents with a distance greater that the defined offset. The default is 0.
		/// </summary>
		public uint Offset
		{
			get { return _offset; }
			set
			{
				_offset = value;
				_offsetSet = true;
			}
		}

		/// <summary>
		/// decay
		/// The decay parameter defines how documents are scored at the distance given at scale. If no decay is defined, documents at the distance scale will be scored 0.5. 
		/// </summary>
		public double Decay
		{
			get { return _decay; }
			set
			{
				if (value <= 0) throw new ElasticsearchCrudException("_decay must be larger than 0.0");
				if (value > 1) throw new ElasticsearchCrudException("_decay must be equal or smaller than 1.0");
				_decay = value;
				_decaySet = true;
			}
		}

		protected void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(_decayType);
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(_field);
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteValue("origin", _origin, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("offset", _offset, elasticsearchCrudJsonWriter, _offsetSet);
			JsonHelper.WriteValue("scale", _scale, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("decay", _decay, elasticsearchCrudJsonWriter, _decaySet);


			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}
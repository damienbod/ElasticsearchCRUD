using System.Collections.Generic;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Aggregations
{
	public class PercentilesMetricAggregation : BaseMetricAggregation
	{
		private List<double> _percents;
		private bool _percentsSet;
		private uint _compression;
		private bool _compressionSet;
		public PercentilesMetricAggregation(string name, string field) : base("percentiles", name, field) { }

		public List<double> Percents
		{
			get { return _percents; }
			set
			{
				_percents = value;
				_percentsSet = true;
			}
		}

		public uint Compression
		{
			get { return _compression; }
			set
			{
				_compression = value;
				_compressionSet = true;
			}
		}
		
		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteListValue("percents", _percents, elasticsearchCrudJsonWriter, _percentsSet);
			JsonHelper.WriteValue("compression", _compression, elasticsearchCrudJsonWriter, _compressionSet);
		}
	}
}
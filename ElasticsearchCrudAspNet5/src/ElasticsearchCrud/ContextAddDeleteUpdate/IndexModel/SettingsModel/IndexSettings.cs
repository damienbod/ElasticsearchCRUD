using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel
{
	public class IndexSettings : IndexUpdateSettings
	{
		private int _numberOfShards;
		private bool _numberOfShardsSet;

		public int NumberOfShards
		{
			get { return _numberOfShards; }
			set
			{
				_numberOfShards = value;
				_numberOfShardsSet = true;
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			Similarities.WriteJson(elasticsearchCrudJsonWriter);
			Analysis.WriteJson(elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("number_of_shards", _numberOfShards, elasticsearchCrudJsonWriter, _numberOfShardsSet);
			base.WriteJson(elasticsearchCrudJsonWriter);
			
		}
	}
}

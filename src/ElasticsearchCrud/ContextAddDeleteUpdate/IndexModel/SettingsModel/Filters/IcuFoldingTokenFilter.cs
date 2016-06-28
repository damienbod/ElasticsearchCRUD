using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
	public class IcuFoldingTokenFilter : AnalysisFilterBase
	{
		private string _unicodeSetFilter;
		private bool _unicodeSetFilterSet;

		public IcuFoldingTokenFilter(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultTokenFilters.IcuFolding;
		}

		/// <summary>
		/// unicodeSetFilter
		/// </summary>
		public string UnicodeSetFilter
		{
			get { return _unicodeSetFilter; }
			set
			{
				_unicodeSetFilter = value;
				_unicodeSetFilterSet = true;
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("type", Type, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("unicodeSetFilter", _unicodeSetFilter, elasticsearchCrudJsonWriter, _unicodeSetFilterSet);
		}
	}
}
using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
	public class UniqueTokenFilter : AnalysisFilterBase
	{
		private bool _onlyOnSamePosition;
		private bool _onlyOnSamePositionSet;

		/// <summary>
		/// The unique token filter can be used to only index unique tokens during analysis. 
		/// By default it is applied on all the token stream. If only_on_same_position is set to true, it will only remove duplicate tokens on the same position.
		/// </summary>
		/// <param name="name">name for the custom filter</param>
		public UniqueTokenFilter(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultTokenFilters.Unique;
		}

		/// <summary>
		/// only_on_same_position
		/// Accepts articles setting which is a set of stop words articles. 
		/// </summary>
		public bool OnlyOnSamePosition
		{
			get { return _onlyOnSamePosition; }
			set
			{
				_onlyOnSamePosition = value;
				_onlyOnSamePositionSet = true;
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("type", Type, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("only_on_same_position", _onlyOnSamePosition, elasticsearchCrudJsonWriter, _onlyOnSamePositionSet);
		}
	}
}
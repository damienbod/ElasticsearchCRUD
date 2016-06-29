using System.Collections.Generic;
using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
	public class ElisionTokenFilter : AnalysisFilterBase
	{
		private List<string> _articles;
		private bool _articlesSet;

		/// <summary>
		/// A token filter which removes elisions. For example, "l’avion" (the plane) will tokenized as "avion" (plane). 
		/// Accepts articles setting which is a set of stop words articles. 
		/// </summary>
		/// <param name="name">name for the custom filter</param>
		public ElisionTokenFilter(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultTokenFilters.Elision;
		}

		/// <summary>
		/// articles
		/// Accepts articles setting which is a set of stop words articles. 
		/// </summary>
		public List<string> Articles
		{
			get { return _articles; }
			set
			{
				_articles = value;
				_articlesSet = true;
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("type", Type, elasticsearchCrudJsonWriter);
			JsonHelper.WriteListValue("articles", _articles, elasticsearchCrudJsonWriter, _articlesSet);
		}
	}
}
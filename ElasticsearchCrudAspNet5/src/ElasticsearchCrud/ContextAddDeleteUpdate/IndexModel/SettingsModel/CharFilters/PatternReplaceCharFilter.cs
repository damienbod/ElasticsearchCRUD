using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.CharFilters
{
	public class PatternReplaceCharFilter : AnalysisCharFilterBase
	{
		private string _pattern;
		private bool _patternSet;
		private string _replacement;
		private bool _replacementSet;

		/// <summary>
		/// The pattern_replace char filter allows the use of a regex to manipulate the characters in a string before analysis. 
		/// The regular expression is defined using the pattern parameter, and the replacement string can be provided using the replacement parameter 
		/// (supporting referencing the original text, as explained here).
		/// "char_filter" : {
		///   "my_pattern":{
		///	    "type":"pattern_replace",
		///	    "pattern":"sample(.*)",
		///  	"replacement":"replacedSample $1"
		///   }
		/// },
		/// </summary>
		/// <param name="name">name for the custom pattern replace char filter</param>
		public PatternReplaceCharFilter(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultCharFilters.PatternReplace;
		}

		/// <summary>
		/// pattern
		/// </summary>
		public string Pattern
		{
			get { return _pattern; }
			set
			{
				_pattern = value;
				_patternSet = true;
			}
		}

		/// <summary>
		/// replacement
		/// </summary>
		public string Replacement
		{
			get { return _replacement; }
			set
			{
				_replacement = value;
				_replacementSet = true;
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("type", Type, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("pattern", _pattern, elasticsearchCrudJsonWriter, _patternSet);
			JsonHelper.WriteValue("replacement", _replacement, elasticsearchCrudJsonWriter, _replacementSet);		
		}
	}
}
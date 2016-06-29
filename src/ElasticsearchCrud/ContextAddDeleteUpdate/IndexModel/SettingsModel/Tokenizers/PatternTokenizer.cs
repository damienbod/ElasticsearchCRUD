using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Tokenizers
{
	public class PatternTokenizer : AnalysisTokenizerBase
	{
		private string _pattern;
		private bool _patternSet;
		private string _group;
		private string _flags;
		private bool _flagsSet;
		private bool _groupSet;

		/// <summary>
		/// A tokenizer of type pattern that can flexibly separate text into terms via a regular expression. 
		/// </summary>
		/// <param name="name">name of custom tokenizer  ToLower()</param>
		public PatternTokenizer(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultTokenizers.Pattern;
		}

		/// <summary>
		/// pattern The regular expression pattern, defaults to \W+.
		/// IMPORTANT: The regular expression should match the token separators, not the tokens themselves.
		/// 
		/// Note that you may need to escape pattern string literal according to your client language rules. 
		/// For example, in many programming languages a string literal for \W+ pattern is written as "\\W+". 
		/// There is nothing special about pattern (you may have to escape other string literals as well); escaping pattern is common just 
		/// because it often contains characters that should be escaped.
		/// 
		/// group set to -1 (the default) is equivalent to "split". Using group >= 0 selects the matching group as the token. For example, if you have:
        ///
		/// pattern = '([^']+)'
		/// group   = 0
		/// input   = aaa 'bbb' 'ccc'
		///
		/// the output will be two tokens: 'bbb' and 'ccc' (including the ' marks). With the same input but using group=1, the output would be: bbb and ccc (no ' marks).
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
		/// flags The regular expression flags.
		/// </summary>
		public string Flags
		{
			get { return _flags; }
			set
			{
				_flags = value;
				_flagsSet = true;
			}
		}

		/// <summary>
		/// group Which group to extract into tokens. Defaults to -1 (split).
		/// </summary>
		public string Group
		{
			get { return _group; }
			set
			{
				_group = value;
				_groupSet = true;
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

			JsonHelper.WriteValue("flags", _flags, elasticsearchCrudJsonWriter, _flagsSet);
			JsonHelper.WriteValue("group", _group, elasticsearchCrudJsonWriter, _groupSet);
		}
	}
}

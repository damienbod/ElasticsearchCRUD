using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Analyzers
{
	public class PatternAnalyzer: BaseStopAnalyzer
	{
		private string _pattern;
		private bool _patternSet;
		private string _flags;
		private bool _flagsSet;
		private bool _lowercase;
		private bool _lowercaseSet;

		public PatternAnalyzer(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultAnalyzers.Pattern;
		}

		/// <summary>
		/// lowercase  Should terms be lowercased or not. Defaults to true.
		/// </summary>
		public bool Lowercase
		{
			get { return _lowercase; }
			set
			{
				_lowercase = value;
				_lowercaseSet = true;
			}
		}

		/// <summary>
		/// pattern The regular expression pattern, defaults to \W+.
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
		/// IMPORTANT: The regular expression should match the token separators, not the tokens themselves.
		///
		/// Flags should be pipe-separated, eg "CASE_INSENSITIVE|COMMENTS". Check Java Pattern API for more details about flags options.
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

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			WriteCommonValues(elasticsearchCrudJsonWriter);

			JsonHelper.WriteValue("lowercase", _lowercase, elasticsearchCrudJsonWriter, _lowercaseSet);
			JsonHelper.WriteValue("pattern", _pattern, elasticsearchCrudJsonWriter, _patternSet);
			JsonHelper.WriteValue("flags", _flags, elasticsearchCrudJsonWriter, _flagsSet);
		}		
	}
}

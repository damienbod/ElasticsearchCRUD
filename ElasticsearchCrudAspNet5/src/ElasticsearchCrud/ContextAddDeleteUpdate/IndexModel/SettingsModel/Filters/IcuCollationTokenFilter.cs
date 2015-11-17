using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
	public class IcuCollationTokenFilter : AnalysisFilterBase
	{
		private string _language;
		private bool _languageSet;
		private IcuCollationStrength _strength;
		private bool _strengthSet;
		private IcuCollationDecomposition _decomposition;
		private bool _decompositionSet;
		private string _alternate;
		private bool _alternateSet;
		private bool _caseLevel;
		private bool _caseLevelSet;
		private IcuCollationCaseLevel _caseFirst;
		private bool _caseFirstSet;
		private bool _numeric;
		private bool _numericSet;
		private string _variableTop;
		private bool _variableTopSet;
		private bool _hiraganaQuaternaryMode;
		private bool _hiraganaQuaternaryModeSet;

		public IcuCollationTokenFilter(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultTokenFilters.IcuCollation;
		}

		/// <summary>
		/// language
		/// </summary>
		public string Language
		{
			get { return _language; }
			set
			{
				_language = value;
				_languageSet = true;
			}
		}

		/// <summary>
		/// strength
		/// The strength property determines the minimum level of difference considered significant during comparison. 
		/// The default strength for the Collator is tertiary, unless specified otherwise by the locale used to create the Collator. 
		/// Possible values: primary, secondary, tertiary, quaternary or identical. See ICU Collation documentation for a more detailed explanation for the specific values.
		/// </summary>
		public IcuCollationStrength Strength
		{
			get { return _strength; }
			set
			{
				_strength = value;
				_strengthSet = true;
			}
		}

		/// <summary>
		/// decomposition
		/// Possible values: no or canonical. Defaults to no. Setting this decomposition property with canonical allows the Collator to handle un-normalized text properly, 
		/// producing the same results as if the text were normalized. If no is set, it is the user’s responsibility to insure that all text is already in the appropriate form 
		/// before a comparison or before getting a CollationKey. Adjusting decomposition mode allows the user to select between faster and more complete collation behavior. 
		/// Since a great many of the world’s languages do not require text normalization, most locales set no as the default decomposition mode.
		/// </summary>
		public IcuCollationDecomposition Decomposition
		{
			get { return _decomposition; }
			set
			{
				_decomposition = value;
				_decompositionSet = true;
			}
		}

		/// <summary>
		/// alternate
		/// Possible values: shifted or non-ignorable. Sets the alternate handling for strength quaternary to be either shifted or non-ignorable. 
		/// What boils down to ignoring punctuation and whitespace.
		/// </summary>
		public string Alternate
		{
			get { return _alternate; }
			set
			{
				_alternate = value;
				_alternateSet = true;
			}
		}

		/// <summary>
		/// caseLevel
		/// Possible values: true or false. Default is false. Whether case level sorting is required. When strength is set to primary this will ignore accent differences.
		/// </summary>
		public bool CaseLevel
		{
			get { return _caseLevel; }
			set
			{
				_caseLevel = value;
				_caseLevelSet = true;
			}
		}

		/// <summary>
		/// caseFirst
		/// Possible values: lower or upper. Useful to control which case is sorted first when case is not ignored for strength tertiary.
		/// </summary>
		public IcuCollationCaseLevel CaseFirst
		{
			get { return _caseFirst; }
			set
			{
				_caseFirst = value;
				_caseFirstSet = true;
			}
		}

		/// <summary>
		/// numeric
		/// Possible values: true or false. Whether digits are sorted according to numeric representation. 
		/// For example the value egg-9 is sorted before the value egg-21. Defaults to false.
		/// </summary>
		public bool Numeric
		{
			get { return _numeric; }
			set
			{
				_numeric = value;
				_numericSet = true;
			}
		}

		/// <summary>
		/// variableTop
		/// Single character or contraction. Controls what is variable for alternate.
		/// </summary>
		public string VariableTop
		{
			get { return _variableTop; }
			set
			{
				_variableTop = value;
				_variableTopSet = true;
			}
		}

		/// <summary>
		/// hiraganaQuaternaryMode
		/// Possible values: true or false. Defaults to false. Distinguishing between Katakana and Hiragana characters in quaternary strength .
		/// </summary>
		public bool HiraganaQuaternaryMode
		{
			get { return _hiraganaQuaternaryMode; }
			set
			{
				_hiraganaQuaternaryMode = value;
				_hiraganaQuaternaryModeSet = true;
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("type", Type, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("language", _language, elasticsearchCrudJsonWriter, _languageSet);
			JsonHelper.WriteValue("strength", _strength.ToString(), elasticsearchCrudJsonWriter, _strengthSet);
			JsonHelper.WriteValue("decomposition", _decomposition.ToString(), elasticsearchCrudJsonWriter, _decompositionSet);
			JsonHelper.WriteValue("alternate", _alternate, elasticsearchCrudJsonWriter, _alternateSet);
			JsonHelper.WriteValue("caseLevel", _caseLevel, elasticsearchCrudJsonWriter, _caseLevelSet);
			JsonHelper.WriteValue("caseFirst", _caseFirst.ToString(), elasticsearchCrudJsonWriter, _caseFirstSet);
			JsonHelper.WriteValue("numeric", _numeric, elasticsearchCrudJsonWriter, _numericSet);
			JsonHelper.WriteValue("variableTop", _variableTop, elasticsearchCrudJsonWriter, _variableTopSet);
			JsonHelper.WriteValue("hiraganaQuaternaryMode", _hiraganaQuaternaryMode, elasticsearchCrudJsonWriter, _hiraganaQuaternaryModeSet);
		}
	}

	public enum IcuCollationStrength
	{
		primary, secondary, tertiary, quaternary, identical
	}

	public enum IcuCollationDecomposition
	{
		no, canonical
	}

	//public enum IcuCollationAlternate
	//{
	//	shifted, non-ignorable
	//}

	public enum IcuCollationCaseLevel
	{
		upper, lower
	}
	
}
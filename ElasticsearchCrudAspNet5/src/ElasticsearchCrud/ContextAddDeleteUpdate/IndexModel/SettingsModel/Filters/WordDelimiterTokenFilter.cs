using System.Collections.Generic;
using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
	public class WordDelimiterTokenFilter: AnalysisFilterBase
	{
		private bool _generateWordParts;
		private bool _generateWordPartsSet;
		private bool _catenateNumbersSet;
		private bool _generateNumberParts;
		private bool _generateNumberPartsSet;
		private bool _catenateWords;
		private bool _catenateWordsSet;
		private bool _catenateNumbers;
		private bool _splitOnCaseChangeSet;
		private bool _catenateAll;
		private bool _catenateAllSet;
		private bool _splitOnCaseChange;
		private bool _preserveOriginal;
		private bool _preserveOriginalSet;
		private bool _stemEnglishPossessive;
		private bool _stemEnglishPossessiveSet;
		private bool _splitOnNumerics;
		private bool _splitOnNumericsSet;
		private List<string> _protectedWords;
		private bool _protectedWordsSet;
		private string _protectedWordsPath;
		private bool _protectedWordsPathSet;
		private string _typeTable;
		private bool _typeTableSet;
		private string _typeTablePath;
		private bool _typeTablePathSet;

		/// <summary>
		/// Named word_delimiter, it Splits words into subwords and performs optional transformations on subword groups. Words are split into subwords with the following rules:
		///
		/// split on intra-word delimiters (by default, all non alpha-numeric characters).
		///   "Wi-Fi" → "Wi", "Fi"
		///   split on case transitions: "PowerShot" → "Power", "Shot"
		///   split on letter-number transitions: "SD500" → "SD", "500"
		///   leading and trailing intra-word delimiters on each subword are ignored: "//hello---there, dude" → "hello", "there", "dude"
		///   trailing "'s" are removed for each subword: "O’Neil’s" → "O", "Neil" 
		/// </summary>
		/// <param name="name">name for the filter</param>
		public WordDelimiterTokenFilter(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultTokenFilters.WordDelimiter;
		}

		/// <summary>
		/// generate_word_parts
		/// If true causes parts of words to be generated: "PowerShot" ⇒ "Power" "Shot". Defaults to true. 
		/// </summary>
		public bool GenerateWordParts
		{
			get { return _generateWordParts; }
			set
			{
				_generateWordParts = value;
				_generateWordPartsSet = true;
			}
		}

		/// <summary> 
		/// generate_number_parts
		///	If true causes number subwords to be generated: "500-42" ⇒ "500" "42". Defaults to true. 
		/// </summary>
		public bool GenerateNumberParts
		{
			get { return _generateNumberParts; }
			set
			{
				_generateNumberParts = value;
				_generateNumberPartsSet = true;
			}
		}

		/// <summary>
		/// catenate_words
		/// If true causes maximum runs of word parts to be catenated: "wi-fi" ⇒ "wifi". Defaults to false. 
		/// </summary>
		public bool CatenateWords
		{
			get { return _catenateWords; }
			set
			{
				_catenateWords = value;
				_catenateWordsSet = true;
			}
		}

		/// <summary>
		/// catenate_numbers
		/// If true causes maximum runs of number parts to be catenated: "500-42" ⇒ "50042". Defaults to false. 
		/// </summary>
		public bool CatenateNumbers
		{
			get { return _catenateNumbers; }
			set
			{
				_catenateNumbers = value;
				_catenateNumbersSet = true;
			}
		}




		/// <summary>
		/// catenate_all
		/// If true causes all subword parts to be catenated: "wi-fi-4000" ⇒ "wifi4000". Defaults to false. 
		/// </summary>
		public bool CatenateAll
		{
			get { return _catenateAll; }
			set
			{
				_catenateAll = value;
				_catenateAllSet = true;
			}
		}

		/// <summary>
		/// split_on_case_change
		/// If true causes "PowerShot" to be two tokens; ("Power-Shot" remains two parts regards). Defaults to true. 
		/// </summary>
		public bool SplitOnCaseChange
		{
			get { return _splitOnCaseChange; }
			set
			{
				_splitOnCaseChange = value;
				_splitOnCaseChangeSet = true;
			}
		}

		/// <summary>
		/// preserve_original
		/// If true includes original words in subwords: "500-42" ⇒ "500-42" "500" "42". Defaults to false. 
		/// </summary>
		public bool PreserveOriginal
		{
			get { return _preserveOriginal; }
			set
			{
				_preserveOriginal = value;
				_preserveOriginalSet = true;
			}
		}

		/// <summary>
		/// split_on_numerics
		/// If true causes "j2se" to be three tokens; "j" "2" "se". Defaults to true. 
		/// </summary>
		public bool SplitOnNumerics
		{
			get { return _splitOnNumerics; }
			set
			{
				_splitOnNumerics = value;
				_splitOnNumericsSet = true;
			}
		}

		/// <summary>
		/// stem_english_possessive
		/// If true causes trailing "'s" to be removed for each subword: "O’Neil’s" ⇒ "O", "Neil". Defaults to true. 
		/// </summary>
		public bool StemEnglishPossessive
		{
			get { return _stemEnglishPossessive; }
			set
			{
				_stemEnglishPossessive = value;
				_stemEnglishPossessiveSet = true;
			}
		}

		/// <summary>
		/// protected_words
		/// A list of protected words from being delimiter. Either an array, or also can set protected_words_path which resolved to a file configured 
		/// with protected words (one on each line). Automatically resolves to config/ based location if exists. 
		/// </summary>
		public List<string> ProtectedWords
		{
			get { return _protectedWords; }
			set
			{
				_protectedWords = value;
				_protectedWordsSet = true;
			}
		}

		/// <summary>
		/// protected_words_path see protected_words
		/// </summary>
		public string ProtectedWordsPath
		{
			get { return _protectedWordsPath; }
			set
			{
				_protectedWordsPath = value;
				_protectedWordsPathSet = true;
			}
		}

		/// <summary>
		/// type_table
		/// A custom type mapping table, for example (when configured using type_table_path):
		/// </summary>
		public string TypeTable
		{
			get { return _typeTable; }
			set
			{
				_typeTable = value;
				_typeTableSet = true;
			}
		}

		public string TypeTablePath
		{
			get { return _typeTablePath; }
			set
			{
				_typeTablePath = value;
				_typeTablePathSet = true;
			}
		}


		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("type", Type, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("generate_word_parts", _generateWordParts, elasticsearchCrudJsonWriter, _generateWordPartsSet);
			JsonHelper.WriteValue("generate_number_parts", _generateNumberParts, elasticsearchCrudJsonWriter, _generateNumberPartsSet);
			JsonHelper.WriteValue("catenate_words", _catenateWords, elasticsearchCrudJsonWriter, _catenateWordsSet);
			JsonHelper.WriteValue("catenate_numbers", _catenateNumbers, elasticsearchCrudJsonWriter, _catenateNumbersSet);
			JsonHelper.WriteValue("catenate_all", _catenateAll, elasticsearchCrudJsonWriter, _catenateAllSet);
			JsonHelper.WriteValue("split_on_case_change", _splitOnCaseChange, elasticsearchCrudJsonWriter, _splitOnCaseChangeSet);
			JsonHelper.WriteValue("preserve_original", _preserveOriginal, elasticsearchCrudJsonWriter, _preserveOriginalSet);
			JsonHelper.WriteValue("split_on_numerics", _splitOnNumerics, elasticsearchCrudJsonWriter, _splitOnNumericsSet);
			JsonHelper.WriteValue("stem_english_possessive", _stemEnglishPossessive, elasticsearchCrudJsonWriter, _stemEnglishPossessiveSet);

			JsonHelper.WriteListValue("protected_words", _protectedWords, elasticsearchCrudJsonWriter, _protectedWordsSet);
			JsonHelper.WriteValue("protected_words_path", _protectedWordsPath, elasticsearchCrudJsonWriter, _protectedWordsPathSet);

			JsonHelper.WriteValue("type_table", _typeTable, elasticsearchCrudJsonWriter, _typeTableSet);
			JsonHelper.WriteValue("type_table_path", _typeTablePath, elasticsearchCrudJsonWriter, _typeTablePathSet);
			
		}
	}
}
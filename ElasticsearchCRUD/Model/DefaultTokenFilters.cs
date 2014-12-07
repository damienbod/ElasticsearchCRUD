namespace ElasticsearchCRUD.Model
{
	public static class DefaultTokenFilters
	{
		public const string Standard = "standard";
		public const string Asciifolding = "asciifolding";
		public const string Length = "length";
		public const string Lowercase = "lowercase";
		public const string Uppercase = "uppercase";
		public const string NGram = "nGram";
		public const string EdgeNGram = "edgeNGram";
		public const string PorterStem = "porter_stem";
		public const string Shingle = "shingle";
		public const string Stop = "stop";
		public const string WordDelimiter = "word_delimiter";
		public const string Stemmer = "stemmer";
		public const string StemmerOverride = "stemmer_override";
		public const string KeywordMarker = "keyword_marker";
		public const string KeywordRepeat = "keyword_repeat";
		public const string Kstem = "kstem";
		public const string Snowball = "snowball";
		public const string Phonetic = "phonetic";
		public const string Synonym = "synonym";
		
		// TODO
		// Compound Word Token Filter
		public const string Reverse = "reverse";
		// Elision Token Filter
		// Truncate Token Filter
		// Unique Token Filter
		// Pattern Capture Token Filter
		// Pattern Replace Token Filter
		public const string Trim = "trim";
		// Limit Token Count Token Filter
		// Hunspell Token Filter
		// Common Grams Token Filter
		// Normalization Token Filter
		// CJK Width Token Filter
		// CJK Bigram Token Filter
		// Delimited Payload Token Filter
		// Keep Words Token Filter
		// Keep Types Token Filter
		public const string Classic = "classic";
		public const string Apostrophe = "apostrophe";
		
	}
}
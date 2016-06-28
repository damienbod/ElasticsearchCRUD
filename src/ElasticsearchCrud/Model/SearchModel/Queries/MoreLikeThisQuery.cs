using System.Collections.Generic;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Queries
{
    /// <summary>
    /// More like this query find documents that are "like" provided text by running it against one or more fields.
    /// Additionally, More Like This can find documents that are "like" a set of chosen documents. 
    /// The syntax to specify one or more documents is similar to the Multi GET API, and supports the ids or docs array. 
    /// If only one document is specified, the query behaves the same as the More Like This API.
    /// 
    /// Under the hood, more_like_this simply creates multiple should clauses in a bool query of interesting terms extracted from some provided text. 
    /// The interesting terms are selected with respect to their tf-idf scores. These are controlled by min_term_freq, min_doc_freq, and max_doc_freq. 
    /// The number of interesting terms is controlled by max_query_terms. 
    /// While the minimum number of clauses that must be satisfied is controlled by percent_terms_to_match. 
    /// The terms are extracted from like_text which is analyzed by the analyzer associated with the field, unless specified by analyzer. 
    /// There are other parameters, such as min_word_length, max_word_length or stop_words, to control what terms should be considered as interesting. 
    /// In order to give more weight to more interesting terms, 
    /// each boolean clause associated with a term could be boosted by the term tf-idf score times some boosting factor boost_terms. 
    /// When a search for multiple docs is issued, More Like This generates a more_like_this query per document field in fields. 
    /// These fields are specified as a top level parameter or within each doc.
    /// </summary>
    public class MoreLikeThisQuery :IQuery
    {
        private List<string> _fields;
        private bool _fieldsSet;
        private string _likeText;
        private bool _likeTextSet;
        private List<Document> _documents;
        private bool _documentsSet;
        private bool _include;
        private bool _includeSet;
        private bool _exclude;
        private bool _excludeSet;
        private uint _minTermFreq;
        private bool _minTermFreqSet;
        private uint _maxQueryTerms;
        private bool _maxQueryTermsSet;
        private string _minimumShouldMatch;
        private bool _minimumShouldMatchSet;
        private string _analyzer;
        private bool _analyzerSet;
        private double _boost;
        private bool _boostSet;
        private List<string> _stopWords;
        private bool _stopWordsSet;
        private uint _minDocFreq;
        private bool _minDocFreqSet;
        private uint _maxDocFreq;
        private bool _maxDocFreqSet;
        private uint _minWordLength;
        private bool _minWordLengthSet;
        private uint _maxWordLength;
        private bool _maxWordLengthSet;
        private uint _boostTerms;
        private bool _boostTermsSet;

        public List<string> Fields
        {
            get { return _fields; }
            set
            {
                _fields = value;
                _fieldsSet = true;
            }
        }

        /// <summary>
        /// like_text
        /// The text to find documents like it, required if ids or docs are not specified.
        /// </summary>
        public string LikeText
        {
            get { return _likeText; }
            set
            {
                _likeText = value;
                _likeTextSet = true;
            }
        }

        public List<Document> Documents
        {
            get { return _documents; }
            set
            {
                _documents = value;
                _documentsSet = true;
            }
        }

        /// <summary>
        /// include
        /// When using ids or docs, specifies whether the documents should be included from the search. Defaults to false.
        /// </summary>
        public bool Include
        {
            get { return _include; }
            set
            {
                _include = value;
                _includeSet = true;
            }
        }

        /// <summary>
        /// exclude
        /// When using ids or docs, specifies whether the documents should be excluded from the search. Defaults to true.
        /// </summary>
        public bool Exclude
        {
            get { return _exclude; }
            set
            {
                _exclude = value;
                _excludeSet = true;
            }
        }

        /// <summary>
        /// minimum_should_match
        /// The minimum_should_match parameter possible values:
        /// 
        /// Integer : Indicates a fixed value regardless of the number of optional clauses.
        /// Negative integer : Indicates that the total number of optional clauses, minus this number should be mandatory.
        /// Percentage 75% : Indicates that this percent of the total number of optional clauses are necessary. The number computed from the percentage is rounded down and used as the minimum.
        /// Negative percentage -25% Indicates that this percent of the total number of optional clauses can be missing.
        ///  The number computed from the percentage is rounded down, before being subtracted from the total to determine the minimum.
        /// Combination : A positive integer, followed by the less-than symbol, followed by any of the previously mentioned specifiers is a conditional specification.
        ///  It indicates that if the number of optional clauses is equal to (or less than) the integer, they are all required, but if it’s greater than the integer,
        ///  the specification applies. In this example: if there are 1 to 3 clauses they are all required, but for 4 or more clauses only 90% are required.
        /// Multiple combinations :  Multiple conditional specifications can be separated by spaces, each one only being valid for numbers greater than the one before it.
        ///  In this example: if there are 1 or 2 clauses both are required, if there are 3-9 clauses all but 25% are required, and if there are more than 9 clauses,
        ///  all but three are required.
        ///
        /// NOTE:
        /// When dealing with percentages, negative values can be used to get different behavior in edge cases. 75% and -25% mean the same thing when dealing with 4 clauses, 
        /// but when dealing with 5 clauses 75% means 3 are required, but -25% means 4 are required.
        ///
        /// If the calculations based on the specification determine that no optional clauses are needed, 
        /// the usual rules about BooleanQueries still apply at search time (a BooleanQuery containing no required clauses must still match at least one optional clause)
        ///
        /// No matter what number the calculation arrives at, a value greater than the number of optional clauses, or a value less than 1 will never be used. 
        /// (ie: no matter how low or how high the result of the calculation result is, the minimum number of required matches will never be lower than 1 or greater than the number of clauses.
        /// </summary>
        public string MinimumShouldMatch
        {
            get { return _minimumShouldMatch; }
            set
            {
                _minimumShouldMatch = value;
                _minimumShouldMatchSet = true;
            }
        }
    
        /// <summary>
        /// min_term_freq
        /// The frequency below which terms will be ignored in the source doc. The default frequency is 2.
        /// </summary>
        public uint MinTermFreq
        {
            get { return _minTermFreq; }
            set
            {
                _minTermFreq = value;
                _minTermFreqSet = true;
            }
        }

        /// <summary>
        /// max_query_terms
        /// The maximum number of query terms that will be included in any generated query. Defaults to 25.
        /// </summary>
        public uint MaxQueryTerms
        {
            get { return _maxQueryTerms; }
            set
            {
                _maxQueryTerms = value;
                _maxQueryTermsSet = true;
            }
        }

        /// <summary>
        /// analyzer
        /// The analyzer can be set to control which analyzer will perform the analysis process on the text. 
        /// It default to the field explicit mapping definition, or the default search analyzer, for example:
        /// </summary>
        public string Analyzer
        {
            get { return _analyzer; }
            set
            {
                _analyzer = value;
                _analyzerSet = true;
            }
        }

        public double Boost
        {
            get { return _boost; }
            set
            {
                _boost = value;
                _boostSet = true;
            }
        }

        /// <summary>
        /// stop_words
        /// An array of stop words. Any word in this set is considered "uninteresting" and ignored. 
        /// Even if your Analyzer allows stopwords, you might want to tell the MoreLikeThis code to ignore them, 
        /// as for the purposes of document similarity it seems reasonable to assume that "a stop word is never interesting".
        /// </summary>
        public List<string> StopWords
        {
            get { return _stopWords; }
            set
            {
                _stopWords = value;
                _stopWordsSet = true;
            }
        }

        /// <summary>
        /// min_doc_freq
        /// The frequency at which words will be ignored which do not occur in at least this many docs. Defaults to 5.
        /// </summary>
        public uint MinDocFreq
        {
            get { return _minDocFreq; }
            set
            {
                _minDocFreq = value;
                _minDocFreqSet = true;
            }
        }

        /// <summary>
        /// max_doc_freq
        /// The maximum frequency in which words may still appear. Words that appear in more than this many docs will be ignored. Defaults to unbounded.
        /// </summary>
        public uint MaxDocFreq
        {
            get { return _maxDocFreq; }
            set
            {
                _maxDocFreq = value;
                _maxDocFreqSet = true;
            }
        }

        /// <summary>
        /// min_word_length
        /// The minimum word length below which words will be ignored. Defaults to 0.(Old name "min_word_len" is deprecated)
        /// </summary>
        public uint MinWordLength
        {
            get { return _minWordLength; }
            set
            {
                _minWordLength = value;
                _minWordLengthSet = true;
            }
        }

        /// <summary>
        /// max_word_length
        /// The maximum word length above which words will be ignored. Defaults to unbounded (0). (Old name "max_word_len" is deprecated)
        /// </summary>
        public uint MaxWordLength
        {
            get { return _maxWordLength; }
            set
            {
                _maxWordLength = value;
                _maxWordLengthSet = true;
            }
        }

        /// <summary>
        /// boost_terms
        /// Sets the boost factor to use when boosting terms. Defaults to deactivated (0). Any other value activates boosting with given boost factor.
        /// </summary>
        public uint BoostTerms
        {
            get { return _boostTerms; }
            set
            {
                _boostTerms = value;
                _boostTermsSet = true;
            }
        }

        public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
        {
            elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("more_like_this");
            elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

            JsonHelper.WriteListValue("fields", _fields, elasticsearchCrudJsonWriter, _fieldsSet);
            JsonHelper.WriteValue("like_text", _likeText, elasticsearchCrudJsonWriter, _likeTextSet);
            if (_documentsSet)
            {
                elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("docs");
                elasticsearchCrudJsonWriter.JsonWriter.WriteStartArray();

                foreach (var item in _documents)
                {
                    elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
                    item.WriteJson(elasticsearchCrudJsonWriter);
                    elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
                }

                elasticsearchCrudJsonWriter.JsonWriter.WriteEndArray();
            }

            JsonHelper.WriteValue("include", _include, elasticsearchCrudJsonWriter, _includeSet);
            JsonHelper.WriteValue("exclude", _exclude, elasticsearchCrudJsonWriter, _excludeSet);
            JsonHelper.WriteValue("minimum_should_match", _minimumShouldMatch, elasticsearchCrudJsonWriter, _minimumShouldMatchSet);
            JsonHelper.WriteValue("min_term_freq", _minTermFreq, elasticsearchCrudJsonWriter, _minTermFreqSet);
            JsonHelper.WriteValue("max_query_terms", _maxQueryTerms, elasticsearchCrudJsonWriter, _maxQueryTermsSet);
            JsonHelper.WriteValue("analyzer", _analyzer, elasticsearchCrudJsonWriter, _analyzerSet);
            JsonHelper.WriteValue("boost", _boost, elasticsearchCrudJsonWriter, _boostSet);
            JsonHelper.WriteListValue("stop_words", _stopWords, elasticsearchCrudJsonWriter, _stopWordsSet);
            JsonHelper.WriteValue("min_doc_freq", _minDocFreq, elasticsearchCrudJsonWriter, _minDocFreqSet);
            JsonHelper.WriteValue("max_doc_freq", _maxDocFreq, elasticsearchCrudJsonWriter, _maxDocFreqSet);
            JsonHelper.WriteValue("min_word_length", _minWordLength, elasticsearchCrudJsonWriter, _minWordLengthSet);
            JsonHelper.WriteValue("max_word_length", _maxWordLength, elasticsearchCrudJsonWriter, _maxWordLengthSet);
            JsonHelper.WriteValue("boost_terms", _boostTerms, elasticsearchCrudJsonWriter, _boostTermsSet);

            elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}

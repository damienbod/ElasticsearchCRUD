using System.Collections.Generic;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Queries
{
	public class TermsQuery : IQuery
	{
		private readonly string _term;
		private readonly List<object> _termValues;
		private double _boost;
		private bool _boostSet;
		private string _minimumShouldMatch;
		private bool _minimumShouldMatchSet;

		public TermsQuery(string term, List<object> termValues)
		{
			_term = term;
			_termValues = termValues;
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
		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("terms");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteListValue(_term, _termValues, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("boost", _boost, elasticsearchCrudJsonWriter, _boostSet);
			JsonHelper.WriteValue("minimum_should_match", _minimumShouldMatch, elasticsearchCrudJsonWriter, _minimumShouldMatchSet);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}
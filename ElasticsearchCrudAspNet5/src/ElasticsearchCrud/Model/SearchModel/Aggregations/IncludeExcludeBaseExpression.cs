using System.Collections.Generic;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Aggregations
{
	public abstract class IncludeExcludeBaseExpression
	{
		private readonly string _pattern;
		private readonly string _expressionProperty;
		private List<IncludeExcludeExpressionFlags> _includeExcludeExpressionFlags;
		private bool _includeExcludeExpressionFlagsSet;

		public IncludeExcludeBaseExpression(string pattern, string expressionProperty)
		{
			_pattern = pattern;
			_expressionProperty = expressionProperty;
		}

		public List<IncludeExcludeExpressionFlags> Flags
		{
			get { return _includeExcludeExpressionFlags; }
			set
			{
				_includeExcludeExpressionFlags = value;
				_includeExcludeExpressionFlagsSet = true;
			}
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(_expressionProperty);		
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteValue("pattern", _pattern, elasticsearchCrudJsonWriter);
			if (_includeExcludeExpressionFlagsSet)
			{
				var flags = string.Join("|", _includeExcludeExpressionFlags.ToArray()); ;
				JsonHelper.WriteValue("flags", flags, elasticsearchCrudJsonWriter);
			}

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}
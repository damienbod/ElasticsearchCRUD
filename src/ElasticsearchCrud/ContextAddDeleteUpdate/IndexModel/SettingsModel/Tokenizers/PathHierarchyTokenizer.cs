using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Tokenizers
{
	public class PathHierarchyTokenizer : AnalysisTokenizerBase
	{
		private string _delimiter;
		private bool _delimiterSet;
		private string _replacement;
		private bool _replacementSet;
		private int _bufferSize;
		private bool _bufferSizeSet;
		private bool _reverse;
		private bool _reverseSet;
		private int _skip;
		private bool _skipSet;

		/// <summary>
		/// The path_hierarchy tokenizer takes something like this:
		/// something/something/else
		/// And produces tokens:
		/// something
		/// something/something
		/// something/something/else
		/// </summary>
		/// <param name="name">custom name</param>
		public PathHierarchyTokenizer(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultTokenizers.Pattern;
		}

		/// <summary>
		/// delimiter 
		/// The character delimiter to use, defaults to /.
		/// </summary>
		public string Delimiter
		{
			get { return _delimiter; }
			set
			{
				_delimiter = value;
				_delimiterSet = true;
			}
		}

		/// <summary>
		/// replacement
		/// An optional replacement character to use. Defaults to the delimiter.
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
	
		/// <summary>
		/// buffer_size
		/// The buffer size to use, defaults to 1024.
		/// </summary>
		public int BufferSize
		{
			get { return _bufferSize; }
			set
			{
				_bufferSize = value;
				_bufferSizeSet = true;
			}
		}

		/// <summary>
		/// reverse
		/// Generates tokens in reverse order, defaults to false.
		/// </summary>
		public bool Reverse
		{
			get { return _reverse; }
			set
			{
				_reverse = value;
				_reverseSet = true;
			}
		}

		/// <summary>
		/// skip
		/// Controls initial tokens to skip, defaults to 0.
		/// </summary>
		public int skip
		{
			get { return _skip; }
			set
			{
				_skip = value;
				_skipSet = true;
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("type", Type, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("delimiter", _delimiter, elasticsearchCrudJsonWriter, _delimiterSet);

			JsonHelper.WriteValue("replacement", _replacement, elasticsearchCrudJsonWriter, _replacementSet);
			JsonHelper.WriteValue("buffer_size", _bufferSize, elasticsearchCrudJsonWriter, _bufferSizeSet);
			JsonHelper.WriteValue("reverse", _reverse, elasticsearchCrudJsonWriter, _reverseSet);
			JsonHelper.WriteValue("skip", _skip, elasticsearchCrudJsonWriter, _skipSet);
		}
	}
}

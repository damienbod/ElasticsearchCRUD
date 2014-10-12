using System;
using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace ElasticsearchCRUD
{
	public class ElasticsearchCrudJsonWriter : IDisposable
	{
		public ElasticsearchCrudJsonWriter()
		{
			Stringbuilder = new StringBuilder();
			JsonWriter = new JsonTextWriter(new StringWriter(Stringbuilder, CultureInfo.InvariantCulture)) { CloseOutput = true };
		}

		public ElasticsearchCrudJsonWriter(StringBuilder stringbuilder)
		{
			Stringbuilder = stringbuilder;
			JsonWriter = JsonWriter = new JsonTextWriter(new StringWriter(Stringbuilder, CultureInfo.InvariantCulture)) { CloseOutput = true };
		}

		public StringBuilder Stringbuilder { get; private set; }

		public JsonWriter JsonWriter { get; private set; }

		private bool _isDisposed;
		public void Dispose()
		{
			if (_isDisposed)
			{
				_isDisposed = true;
				JsonWriter.Close();
				JsonWriter = null;
				
			}
		}
	}
}

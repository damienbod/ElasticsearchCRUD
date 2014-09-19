using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Damienbod.BusinessLayer.DomainModel;
using Newtonsoft.Json;

namespace Damienbod.ElasticSearchProvider
{
	public class ElasticsearchSerializer : IDisposable
	{
		private JsonWriter _writer;

		internal string Serialize(IEnumerable<Animal> entries, string index)
		{
			if (Regex.IsMatch(index, "[\\\\/*?\",<>|\\sA-Z]"))
			{
				throw new ArgumentException("bad index");
			}

			if (entries == null)
			{
				return null;
			}

			var sb = new StringBuilder();
			_writer = new JsonTextWriter(new StringWriter(sb, CultureInfo.InvariantCulture)) { CloseOutput = true };

			foreach (var entry in entries)
			{
				WriteJsonEntry(entry, index);
			}

			_writer.Close();
			_writer = null;

			return sb.ToString();
		}

		private void WriteJsonEntry(Animal entry, string index)
		{
			_writer.WriteStartObject();

			_writer.WritePropertyName("index");

			// Write the batch "index" operation header
			_writer.WriteStartObject();
			WriteValue("_index", index);
			WriteValue("_type", typeof(Animal).ToString());
			_writer.WriteEndObject();
			_writer.WriteEndObject();
			_writer.WriteRaw("\n");  //ES requires this \n separator

			_writer.WriteStartObject();
			WriteValue("Id", entry.Id);
			
			WriteValue("AnimalType", entry.AnimalType);
			WriteValue("TypeSpecificForAnimalType", entry.TypeSpecificForAnimalType);
			WriteValue("Description", entry.Description);
			WriteValue("Gender", entry.Gender);
			WriteValue("LastLocation", entry.LastLocation);

			WriteValue("DateOfBirth", entry.DateOfBirth.UtcDateTime);
			WriteValue("CreatedTimestamp", entry.CreatedTimestamp.UtcDateTime);
			WriteValue("UpdatedTimestamp", entry.UpdatedTimestamp.UtcDateTime);

			_writer.WriteEndObject();
			_writer.WriteRaw("\n");
		}

		private void WriteValue(string key, object valueObj)
		{
			_writer.WritePropertyName(key);
			_writer.WriteValue(valueObj);
		}

		public void Dispose()
		{
			if (_writer != null)
			{
				_writer.Close();
				_writer = null;
			}
		}
	}
}

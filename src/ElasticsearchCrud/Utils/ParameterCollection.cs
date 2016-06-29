using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticsearchCRUD.Utils
{
	public class ParameterCollection
	{
		private readonly Dictionary<string, string> _parms = new Dictionary<string, string>();

		public void Add(string key, string val, bool isSet = true)
		{
			if (isSet)
			{
				if (_parms.ContainsKey(key))
				{
					throw new InvalidOperationException(string.Format("The key {0} already exists.", key));
				}
				_parms.Add(key, val);
			}
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("?");
			foreach (var kvp in _parms)
			{
				if (sb.Length > 0) { sb.Append("&"); }
				sb.AppendFormat("{0}={1}",
					Uri.EscapeDataString(kvp.Key),
					Uri.EscapeDataString(kvp.Value));
			}
			var result = sb.ToString();
			if (result.Equals("?")) return String.Empty;

			return result;
		}
	}
}
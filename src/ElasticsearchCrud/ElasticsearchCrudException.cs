using System;

namespace ElasticsearchCRUD
{
	public class ElasticsearchCrudException : Exception
	{
		public ElasticsearchCrudException(string message) : base(message)
		{
		}
	}
}

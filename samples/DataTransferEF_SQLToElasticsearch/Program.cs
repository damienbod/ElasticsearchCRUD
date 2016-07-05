using System;
using DataTransferSQLToEl.ExampleReindexChildDocuments;

namespace DataTransferSQLToEl
{
	class Program
	{
		static void Main(string[] args)
		{
			Repo repo = new Repo();
			//repo.SaveToElasticsearchPerson();
			
			//repo.SaveToElasticsearchStateProvince();
			var addressX = repo.GetAddressFromElasticsearch("37", "14");
			Console.WriteLine(addressX);

			//DateTime beginDateTime = DateTime.UtcNow;

			//Reindexer.ReindexStateProvince(beginDateTime);
			//Reindexer.ReindexStateProvinceAddress(beginDateTime);

			//Console.WriteLine("Created new index from version 1 index");
			Console.ReadLine();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferSQLToEl
{
	class Program
	{
		static void Main(string[] args)
		{
			Repo repo = new Repo();
			//repo.SaveToElasticsearchPerson();
			repo.SaveToElasticsearchCountryRegion();

			var dd = repo.GetCountryRegionFromElasticsearch("EN");
			Console.WriteLine(dd);
		}
	}
}

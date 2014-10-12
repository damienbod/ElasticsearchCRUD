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
			repo.SaveToElasticsearchAddress();

			var dd = repo.GetAddressFromElasticsearch("US");
			Console.WriteLine(dd);
		}
	}
}

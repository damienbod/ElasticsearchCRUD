namespace ElasticsearchCRUD.Model.SearchModel.Queries.FunctionQuery
{
	public enum FunctionScoreQueryBoostMode
	{
		/// <summary>
		/// query score and function score is multiplied (default)
		/// </summary>
		multiply,
		
		/// <summary>
		/// only function score is used, the query score is ignored
		/// </summary>
		replace,
		
		/// <summary>
		/// query score and function score are added
		/// </summary>
		sum,
		
		/// <summary>
		/// average
		/// </summary>
		avg,
		
		/// <summary>
		/// max of query score and function score
		/// </summary>
		max,
		
		/// <summary>
		/// min of query score and function score 
		/// </summary>
		min
	}
}
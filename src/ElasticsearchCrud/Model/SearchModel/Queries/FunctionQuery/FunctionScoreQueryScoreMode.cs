namespace ElasticsearchCRUD.Model.SearchModel.Queries.FunctionQuery
{
	public enum FunctionScoreQueryScoreMode
	{
		/// <summary>
		/// scores are multiplied (default)
		/// </summary>
		multiply,
		
		/// <summary>
		/// scores are summed
		/// </summary>
		sum,
		
		/// <summary>
		/// scores are averaged
		/// </summary>
		avg,
		
		/// <summary>
		/// the first function that has a matching filter is applied
		/// </summary>
		first,
		
		/// <summary>
		/// maximum score is used
		/// </summary>
		max,
		
		/// <summary>
		/// minimum score is used 
		/// </summary>
		min
	}
}
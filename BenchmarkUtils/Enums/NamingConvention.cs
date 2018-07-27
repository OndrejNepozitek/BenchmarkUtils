namespace BenchmarkUtils.Enums
{
	/// <summary>
	/// Specifies the naming convention of files with benchmark results
	/// </summary>
	public enum NamingConvention
	{
		Timestamp, // The current timestamp will be used - timestamp.txt
		FixedName, // Fixed filename will be used
		Custom, // Custom function will be used to generate a filename
	}
}
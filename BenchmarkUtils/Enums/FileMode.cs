namespace BenchmarkUtils.Enums
{
	/// <summary>
	/// Specifies whether data should be appended to existing file or if it should be overwritten.
	/// </summary>
	public enum FileMode
	{
		Append, // Append new data to an existings file
		Overwrite, // Overwrite old data
	}
}
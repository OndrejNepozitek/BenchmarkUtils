namespace BenchmarkUtils
{
	/// <summary>
	/// Represents a job for the benchmark.
	/// </summary>
	/// <typeparam name="TResult">Type of the result that will be displayed in a table.</typeparam>
	public interface IBenchmarkJob<out TResult>
	{
		/// <summary>
		/// Executes the job and returns its result.
		/// </summary>
		/// <returns></returns>
		TResult Execute();
	}
}
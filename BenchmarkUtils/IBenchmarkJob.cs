namespace BenchmarkUtils
{
	public interface IBenchmarkJob<out TResult>
	{
		TResult Execute();
	}
}
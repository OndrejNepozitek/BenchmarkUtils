namespace BenchmarkUtils
{
	using System;

	public interface IPreviewableBenchmarkJob<out TResult> : IBenchmarkJob<TResult>
	{
		event Action<TResult, int, int> OnPreview;
	}
}
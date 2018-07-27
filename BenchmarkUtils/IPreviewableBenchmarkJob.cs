namespace BenchmarkUtils
{
	using System;

	/// <inheritdoc />
	/// <summary>
	/// Represents a job that provides intermediate results of the execution.
	/// </summary>
	public interface IPreviewableBenchmarkJob<out TResult> : IBenchmarkJob<TResult>
	{
		/// <summary>
		/// Event that is called when an intermediate result is available.
		/// </summary>
		event Action<TResult> OnPreview;
	}
}
namespace BenchmarkUtils
{
	using System;
	using System.Threading.Tasks;

	/// <summary>
	/// Multithreaded version of the benchmarking class.
	/// </summary>
	/// <remarks>
	/// All given jobs must be thread-safe.
	/// </remarks>
	/// <typeparam name="TJob"></typeparam>
	/// <typeparam name="TResult"></typeparam>
	public class MultiThreadedBenchmark<TJob, TResult> : Benchmark<TJob, TResult> 
		where TJob : IBenchmarkJob<TResult>
	{
		protected TResult[] Results;

		/// <summary>
		/// Executes a benchmark of given jobs with a given name.
		/// </summary>
		/// <remarks>
		/// Jobs are executed in the threadpool and must be therefore thread-safe.
		/// </remarks>
		/// <param name="jobs">Jobs to be benchmarked.</param>
		/// <param name="name">Optional name of the benchmark.</param>
		public override void Run(TJob[] jobs, string name = null)
		{
			BenchmarkStarted(name);

			Results = new TResult[jobs.Length];
			var tasks = new Task[jobs.Length];

			if (WithConsole)
			{
				for (var i = 0; i < jobs.Length; i++)
				{
					Console.WriteLine(" Not yet executed");
				}

				Console.SetCursorPosition(0, Console.CursorTop - jobs.Length);
			}

			for (var i = 0; i < jobs.Length; i++)
			{
				var task = Run(jobs[i], i);
				tasks[i] = task;
			}

			Task.WaitAll(tasks);

			foreach (var result in Results)
			{
				BenchmarkTableOutput.PrintRow(result, TextWritersArray);
			}

			foreach (var textWriter in TextWritersArray)
			{
				textWriter.Flush();
			}

			BenchmarkEnded();
		}

		protected Task Run(TJob job, int index)
		{
			if (WithConsole && job is IPreviewableBenchmarkJob<TResult> previewableJob)
			{
				previewableJob.OnPreview += (previewResult) =>
				{
					lock (BenchmarkTableOutput)
					{
						BenchmarkTableOutput.PreviewRow(previewResult, index);
					}
				};
			}

			return Task.Run(() =>
			{
				var result = job.Execute();

				lock (BenchmarkTableOutput)
				{
					BenchmarkTableOutput.PreviewRow(result, index);
				}

				lock (Results)
				{
					Results[index] = result;
				}
			});
		}
	}
}
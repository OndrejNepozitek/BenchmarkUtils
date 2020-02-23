using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

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
        private readonly int maxDegreeOfParallelism;

        public MultiThreadedBenchmark(int maxDegreeOfParallelism = 10)
        {
            this.maxDegreeOfParallelism = maxDegreeOfParallelism;
        }

		/// <summary>
		/// Executes a benchmark of given jobs with a given name.
		/// </summary>
		/// <remarks>
		/// Jobs are executed in the threadpool and must be therefore thread-safe.
		/// </remarks>
		/// <param name="jobs">Jobs to be benchmarked.</param>
		/// <param name="name">Optional name of the benchmark.</param>
		public override List<TResult> Run(TJob[] jobs, string name = null)
		{
			BenchmarkStarted(name);

			var results = new TResult[jobs.Length];
			var tasks = new Task[jobs.Length];

			if (WithConsole && WithConsolePreview)
			{
				for (var i = 0; i < jobs.Length; i++)
				{
					Console.WriteLine(" Not yet executed");
				}

				Console.SetCursorPosition(0, Console.CursorTop - jobs.Length);
			}

            var dummyArray = new int[jobs.Length];
            var partitioner = Partitioner.Create(dummyArray, EnumerablePartitionerOptions.NoBuffering);
            Parallel.ForEach(partitioner, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, (input, state, i) =>
            {
				var result = Run(jobs[i], (int) i);

                lock (results)
                {
                    results[i] = result;
                }
            });

			foreach (var result in results)
			{
				BenchmarkTableOutput.PrintRow(result, TextWritersArray);
			}

			foreach (var textWriter in TextWritersArray)
			{
				textWriter.Flush();
			}

			BenchmarkEnded();

            return results.ToList();
        }

		protected TResult Run(TJob job, int index)
		{
			if (WithConsole && WithConsolePreview && job is IPreviewableBenchmarkJob<TResult> previewableJob)
			{
				previewableJob.OnPreview += (previewResult) =>
				{
					lock (BenchmarkTableOutput)
					{
						BenchmarkTableOutput.PreviewRow(previewResult, index);
					}
				};
			}

            var result = job.Execute();

            if (WithConsole && WithConsolePreview)
            {
                lock (BenchmarkTableOutput)
                {
                    BenchmarkTableOutput.PreviewRow(result, index);
                }
            }

            return result;
        }
	}
}
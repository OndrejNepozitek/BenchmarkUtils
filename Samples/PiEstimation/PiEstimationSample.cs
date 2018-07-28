namespace Samples.PiEstimation
{
	using System.Collections.Generic;
	using BenchmarkUtils;

	public class PiEstimationSample
	{
		public static void Run()
		{
			var benchmark = new Benchmark<BenchmarkJob, BenchmarkResult>();
			benchmark.AddFileOutput();

			const int benchmarksCount = 10;
		
			var jobs = new BenchmarkJob[]
			{
				new BenchmarkJob(benchmarksCount, 50),
				new BenchmarkJob(benchmarksCount, 500),
				new BenchmarkJob(benchmarksCount, 5000),
				new BenchmarkJob(benchmarksCount, 50000),
				new BenchmarkJob(benchmarksCount, 500000),
				new BenchmarkJob(benchmarksCount, 5000000),
				new BenchmarkJob(benchmarksCount, 50000000),
				new BenchmarkJob(benchmarksCount, 500000000),
			};

			benchmark.Run(jobs, "PI estimation");
		}
	}
}

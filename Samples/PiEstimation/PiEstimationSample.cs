namespace Samples.PiEstimation
{
	using System.Collections.Generic;
	using BenchmarkUtils;

	public class PiEstimationSample
	{
		public static void Run()
		{
			//var benchmark = new Benchmark<BenchmarkJob, BenchmarkResult>();
			var benchmark = new MultiThreadedBenchmark<BenchmarkJob, BenchmarkResult>();
			benchmark.AddFileOutput();

			const int benchmarksCount = 10;
		
			var jobs = new BenchmarkJob[]
			{
				new BenchmarkJob(benchmarksCount, 50),
				new BenchmarkJob(benchmarksCount, 500),
				new BenchmarkJob(benchmarksCount, 5000),
				new BenchmarkJob(benchmarksCount, 50000),
				new BenchmarkJob(benchmarksCount, 100000),
				new BenchmarkJob(benchmarksCount, 500000),
				new BenchmarkJob(benchmarksCount, 1000000),
				new BenchmarkJob(benchmarksCount, 5000000),
				new BenchmarkJob(benchmarksCount, 10000000),
			};

			benchmark.Run(jobs, "PI estimation");
		}
	}
}

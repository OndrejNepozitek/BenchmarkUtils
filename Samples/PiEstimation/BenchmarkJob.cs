namespace Samples.PiEstimation
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using BenchmarkUtils;
	using Utils;

	public class BenchmarkJob : IPreviewableBenchmarkJob<BenchmarkResult>
	{
		public event Action<BenchmarkResult, int, int> OnPreview;
		private readonly int benchmarksCount;
		private readonly int randomWalksCount;

		public BenchmarkJob(int benchmarksCount, int randomWalksCount)
		{
			this.benchmarksCount = benchmarksCount;
			this.randomWalksCount = randomWalksCount;
		}

		public BenchmarkResult Execute()
		{
			var random = new Random();

			var minError = double.MaxValue;
			var bestEstimation = double.MaxValue;
			var errors = new List<double>();
			var times = new List<long>();

			var timer = new Stopwatch();

			for (var i = 0; i < benchmarksCount; i++)
			{
				timer.Restart();
				var insideCount = 0;

				for (var j = 0; j < randomWalksCount; j++)
				{
					var sampleX = random.NextDouble();
					var sampleY = random.NextDouble();
					var euclideanDistance = Math.Sqrt(Math.Pow(sampleX, 2) + Math.Pow(sampleY, 2));

					if (euclideanDistance <= 1)
					{
						insideCount++;
					}
				}

				times.Add(timer.ElapsedMilliseconds);

				var estimation = 4 * (insideCount / (double) randomWalksCount);
				var error = Math.Abs(Math.PI - estimation);
				errors.Add(error);

				if (error < minError)
				{
					minError = error;
					bestEstimation = estimation;
				}

				OnPreview?.Invoke(new BenchmarkResult()
				{
					Name = $"Run {i+2}/{benchmarksCount}",
					BestEstimation = bestEstimation,
					ErrorMin = minError,
					ErrorMedian = errors.GetMedian(),
					SimulationTime = (times.Average() / 1000),
				}, i, benchmarksCount);
			}

			return new BenchmarkResult()
			{
				Name = $"{randomWalksCount} walks",
				BestEstimation = bestEstimation,
				ErrorMin = minError,
				ErrorMedian = errors.GetMedian(),
				SimulationTime = (times.Average() / 1000),
			};
		}
	}
}
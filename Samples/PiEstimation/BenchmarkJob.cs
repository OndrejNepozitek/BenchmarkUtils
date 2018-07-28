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
		public event Action<BenchmarkResult> OnPreview;
		private readonly int benchmarksCount;
		private readonly int samplesCount;

		public BenchmarkJob(int benchmarksCount, int samplesCount)
		{
			this.benchmarksCount = benchmarksCount;
			this.samplesCount = samplesCount;
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

				for (var j = 0; j < samplesCount; j++)
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

				var estimation = 4 * (insideCount / (double) samplesCount);
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
					AverageTime = (times.Average() / 1000),
				});
			}

			return new BenchmarkResult()
			{
				Name = $"{samplesCount} samples",
				BestEstimation = bestEstimation,
				ErrorMin = minError,
				ErrorMedian = errors.GetMedian(),
				AverageTime = (times.Average() / 1000),
			};
		}
	}
}
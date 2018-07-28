namespace Samples.PiEstimation
{
	using BenchmarkUtils.Attributes;
	using BenchmarkUtils.Enums;

	public class BenchmarkResult
	{
		[Order(1)]
		[Length(25)]
		[Name("Name")]
		public string Name { get; set; }

		[Order(2)]
		[Length(20)]
		[Name("Best estimation")]
		public double BestEstimation { get; set; }

		[Order(3)]
		[Length(25)]
		[Name("Error min")]
		public double ErrorMin { get; set; }

		[Order(4)]
		[Length(25)]
		[Name("Error median")]
		public double ErrorMedian { get; set; }

		[Order(5)]
		[Length(15)]
		[Name("Avg time")]
		[ValueFormat("{0:N4} s")]
		public double AverageTime { get; set; }
	}
}
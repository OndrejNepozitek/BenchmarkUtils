namespace BenchmarkUtils.Attributes
{
	using System;

	public class LengthAttribute : Attribute
	{
		public int Length { get; }

		public LengthAttribute(int length)
		{
			Length = length;
		}
	}
}
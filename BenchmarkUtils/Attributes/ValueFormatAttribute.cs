namespace BenchmarkUtils.Attributes
{
	using System;

	public class ValueFormatAttribute : Attribute
	{
		public string Format { get; }

		public ValueFormatAttribute(string format)
		{
			Format = format;
		}
	}
}
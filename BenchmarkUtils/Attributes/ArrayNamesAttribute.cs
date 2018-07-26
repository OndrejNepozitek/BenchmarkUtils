namespace BenchmarkUtils.Attributes
{
	using System;

	public class ArrayNamesAttribute : Attribute
	{
		public string[] Names { get; }

		public ArrayNamesAttribute(params string[] names)
		{
			Names = names;
		}
	}
}
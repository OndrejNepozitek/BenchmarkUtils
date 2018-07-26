namespace BenchmarkUtils.Attributes
{
	using System;

	public class NameAttribute : Attribute
	{
		public string Name { get; }

		public NameAttribute(string name)
		{
			Name = name;
		}
	}
}
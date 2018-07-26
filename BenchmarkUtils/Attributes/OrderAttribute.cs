namespace BenchmarkUtils.Attributes
{
	using System;

	[AttributeUsage(AttributeTargets.Property)]
	public class OrderAttribute : Attribute
	{
		public int Order { get; }

		public OrderAttribute(int order)
		{
			Order = order;
		}
	}
}
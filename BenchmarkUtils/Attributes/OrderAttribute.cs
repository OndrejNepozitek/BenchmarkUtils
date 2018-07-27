namespace BenchmarkUtils.Attributes
{
	using System;

	/// <summary>
	/// Specifies the order of the column as seen in the results table.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class OrderAttribute : Attribute
	{
		/// <summary>
		/// Order of the column.
		/// </summary>
		public int Order { get; }

		/// <summary>
		/// </summary>
		/// <param name="order">Order of the column.</param>
		public OrderAttribute(int order)
		{
			Order = order;
		}
	}
}
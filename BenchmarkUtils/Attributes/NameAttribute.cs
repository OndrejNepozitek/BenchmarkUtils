namespace BenchmarkUtils.Attributes
{
	using System;

	/// <summary>
	/// Specifies the column name that will be displayed in the header of the result table.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class NameAttribute : Attribute
	{
		/// <summary>
		/// Name of the column as displayed in the header.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// </summary>
		/// <param name="name">Name of the column as displayed in the header.</param>
		public NameAttribute(string name)
		{
			Name = name;
		}
	}
}
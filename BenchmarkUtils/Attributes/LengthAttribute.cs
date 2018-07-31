namespace BenchmarkUtils.Attributes
{
	using System;

	/// <summary>
	/// Sets the length of a corresponding column.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
	public class LengthAttribute : Attribute
	{
		/// <summary>
		/// Length of the column.
		/// </summary>
		public int Length { get; }

		/// <summary>
		/// </summary>
		/// <param name="length">Length of the column.</param>
		public LengthAttribute(int length)
		{
			if (length <= 0)
			{
				throw new ArgumentException("Length must be greater than 0.", nameof(length));
			}

			Length = length;
		}
	}
}
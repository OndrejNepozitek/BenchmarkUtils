namespace BenchmarkUtils.Attributes
{
	using System;

	/// <summary>
	/// Specifies the format of values contained in the column.
	/// </summary>
	/// <remarks>
	/// The format string is later used as follows: string.Format(formatString, value);
	/// See the docs - https://msdn.microsoft.com/en-us/library/system.string.format(v=vs.110).aspx
	/// </remarks>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
	public class ValueFormatAttribute : Attribute
	{
		/// <summary>
		/// Format of the value.
		/// </summary>
		public string Format { get; }

		/// <summary>
		/// </summary>
		/// <param name="format">Format of the value.</param>
		public ValueFormatAttribute(string format)
		{
			Format = format;
		}
	}
}
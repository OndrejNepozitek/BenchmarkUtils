namespace BenchmarkUtils.Attributes
{
	using System;
	using Enums;

	/// <summary>
	/// Specifies where should be the column shown.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
	public class ShowAttribute : Attribute
	{
		/// <summary>
		/// Where should be the column shown.
		/// </summary>
		public ShowIn Type { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="type">Where should be the column shown.</param>
		public ShowAttribute(ShowIn type)
		{
			Type = type;
		}
	}
}
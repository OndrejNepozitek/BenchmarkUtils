namespace BenchmarkUtils.Attributes
{
	using System;
	using Enums;

	public class ShowAttribute : Attribute
	{
		public ShowIn Type { get; }

		public ShowAttribute(ShowIn type)
		{
			Type = type;
		}
	}
}
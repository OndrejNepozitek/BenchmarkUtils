namespace Samples.Utils
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public static class Extensions
	{
		/// <summary>
		/// Gets the median of the source.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static double GetMedian(this IEnumerable<double> source)
		{
			// Create a copy of the input, and sort the copy
			var temp = source.ToArray();
			Array.Sort(temp);

			var count = temp.Length;
			if (count == 0)
			{
				throw new InvalidOperationException("Empty collection");
			}

			if (count % 2 == 0)
			{
				// Count is even, average two middle elements
				var a = temp[count / 2 - 1];
				var b = temp[count / 2];
				return (a + b) / 2d;
			}

			// Count is odd, return the middle element
			return temp[count / 2];
		}
	}
}
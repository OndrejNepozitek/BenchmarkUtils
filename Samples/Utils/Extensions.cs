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
		public static T GetMedian<T>(this IEnumerable<T> source)
			where T : IComparable<T>
		{
			// Create a copy of the input, and sort the copy
			var temp = source.ToArray();
			Array.Sort(temp);

			var count = temp.Length;
			if (count == 0)
			{
				throw new InvalidOperationException("Empty collection");
			}

			return temp[count / 2];
		}
	}
}
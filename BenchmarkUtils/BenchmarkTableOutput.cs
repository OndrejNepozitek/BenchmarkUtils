namespace BenchmarkUtils
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using Attributes;
	using Enums;

	/// <summary>
	/// Class that prints a table with benchmark results based on a given results object.
	/// </summary>
	/// <remarks>
	/// Uses attributes and reflection to get information about individual columns from the TResult type.
	/// </remarks>
	/// <typeparam name="TResult">Type of the result object.</typeparam>
	public class BenchmarkTableOutput<TResult>
	{
		private readonly List<Column> columns = new List<Column>();

		private int defaultLength;
		private string defaultFormat;
		private ShowIn defaultShow;

		public BenchmarkTableOutput()
		{
			PrepareColumns();
		}

		/// <summary>
		/// Prepares information about individuals columns of the table.
		/// </summary>
		private void PrepareColumns()
		{
			var type = typeof(TResult);
			var propertiesWithOrder = new List<Tuple<PropertyInfo, int>>();

			// Get properties together with their order
			foreach (var property in type.GetProperties())
			{
				// Skip properties that should not be shown
				if (property.GetCustomAttribute<ShowAttribute>()?.Type == ShowIn.None)
				{
					continue;
				}

				var orderAttribute = property.GetCustomAttribute<OrderAttribute>();
				var order = orderAttribute?.Order ?? int.MaxValue;

				propertiesWithOrder.Add(Tuple.Create(property, order));
			}

			// Sort properties by their order
			propertiesWithOrder.Sort((p1, p2) => p1.Item2.CompareTo(p2.Item2));

			defaultLength = typeof(TResult).GetCustomAttribute<LengthAttribute>()?.Length ?? 20;
			defaultFormat = typeof(TResult).GetCustomAttribute<ValueFormatAttribute>()?.Format;
			defaultShow = typeof(TResult).GetCustomAttribute<ShowAttribute>()?.Type ?? ShowIn.All;

			// Use reflection to call the CreateColumn generic method.
			// This is done to get into a strongly-typed context and make the code simpler.
			var createColumnMethod = GetType().GetMethod(nameof(CreateColumn), BindingFlags.NonPublic | BindingFlags.Instance);

			foreach (var propertyTuple in propertiesWithOrder)
			{
				var property = propertyTuple.Item1;
				var createColumnGeneric = createColumnMethod.MakeGenericMethod(property.PropertyType);
				var column = (Column) createColumnGeneric.Invoke(this, new object[]{property});

				columns.Add(column);
			}
		}

		/// <summary>
		/// Creates a Column from a given property info.
		/// </summary>
		/// <remarks>
		/// Helper method that gets us into a strongy-typed context which
		/// makes the code simpler when compared to a pure reflection.
		/// </remarks>
		/// <typeparam name="TReturn">Type of a given property.</typeparam>
		/// <param name="property"></param>
		/// <returns></returns>
		private Column CreateColumn<TReturn>(PropertyInfo property)
		{
			var length = property.GetCustomAttribute<LengthAttribute>()?.Length ?? defaultLength;
			var format = property.GetCustomAttribute<ValueFormatAttribute>()?.Format ?? defaultFormat;
			var hideIn = property.GetCustomAttribute<ShowAttribute>()?.Type ?? defaultShow;

			// Prepare the getter of the property
			var getterInfo = property.GetGetMethod();
			var getterFunc = (Func<TResult, TReturn>) Delegate.CreateDelegate(typeof(Func<TResult, TReturn>), getterInfo);

			var nameAttribute = property.GetCustomAttribute<NameAttribute>();

			if (nameAttribute == null)
			{
				throw new ArgumentException("The Name attribute must be specified");
			}

			var name = nameAttribute.Name;

			return new Column(name, length, format, hideIn, (result) => getterFunc(result));
		}

		/// <summary>
		/// Prints the table header to given text writers.
		/// </summary>
		/// <param name="name">Name of the benchmark. May be null.</param>
		/// <param name="writers">Where to output the header.</param>
		public void PrintHeader(string name, params TextWriter[] writers)
		{
			foreach (var writer in writers)
			{
				if (!string.IsNullOrEmpty(name))
				{
					writer.WriteLine($" << {name} >>");
				}

				var totalWidth = columns.Where(x => ShouldShow(x, writer)).Sum(x => x.Width) + 1;

				writer.WriteLine(new string('-', totalWidth));

				var first = true;
				foreach (var column in columns)
				{
					if (!ShouldShow(column, writer))
						continue;

					if (first)
					{
						first = false;

						writer.Write($" {column.Name.PadRight(column.Width - 1, ' ')}");
					}
					else
					{
						writer.Write($"| {column.Name.PadRight(column.Width - 2, ' ')}");
					}
				}

				writer.WriteLine("|");
				writer.WriteLine(new string('-', totalWidth));
			}
		}

		/// <summary>
		/// Prints a result row to given text writers.
		/// </summary>
		/// <param name="result"></param>
		/// <param name="writers"></param>
		public void PrintRow(TResult result, params TextWriter[] writers)
		{
			var first = true;
			foreach (var column in columns)
			{
				var valueObj = column.GetterFunc(result);
				var value = column.Format != null ? string.Format(column.Format, valueObj) : valueObj.ToString();

				foreach (var writer in writers)
				{
					if (!ShouldShow(column, writer))
						continue;

					if (first)
					{
						writer.Write(" ");
						writer.Write(value.PadRight(column.Width - 1, ' '));
					}
					else
					{
						writer.Write("  ");
						writer.Write(value.PadRight(column.Width - 2, ' '));
					}
				}

				if (first)
				{
					first = false;
				}
			}

			foreach (var writer in writers)
			{
				writer.WriteLine();
			}
		}

		/// <summary>
		/// Prints an intermediate result row to the console, overwriting the row at the specified offset.
		/// </summary>
		/// <param name="result">Result to be previewed.</param>
		/// <param name="rowOffset">Offset relative to the current cursor position.</param>
		public void PreviewRow(TResult result, int rowOffset = 0)
		{
			Console.SetCursorPosition(0, Console.CursorTop + rowOffset);

			PrintRow(result, Console.Out);

			Console.SetCursorPosition(0, Console.CursorTop - rowOffset - 1);
		}

		/// <summary>
		/// Decides 
		/// </summary>
		/// <param name="column"></param>
		/// <param name="writer"></param>
		/// <returns></returns>
		private bool ShouldShow(Column column, TextWriter writer)
		{
			return column.ShowIn == ShowIn.All 
			       || (column.ShowIn == ShowIn.Console && writer == Console.Out) 
			       || (column.ShowIn == ShowIn.File && writer != Console.Out);
		}

		private class Column
		{
			public string Name { get; }

			public int Width { get; }

			public string Format { get; }

			public ShowIn ShowIn { get; }

			public Func<TResult, object> GetterFunc { get; }

			public Column(string name, int width, string format, ShowIn showIn, Func<TResult, object> getterFunc)
			{
				Name = name;
				Width = width;
				Format = format;
				GetterFunc = getterFunc;
				ShowIn = showIn;
			}
		}
	}
}
namespace BenchmarkUtils
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using Attributes;
	using Enums;

	public class BenchmarkFormatter<TResult>
	{
		private readonly List<Column> columns = new List<Column>();
		private delegate object Getter(TResult result);

		public BenchmarkFormatter()
		{
			PrepareColumns();
		}

		private void PrepareColumns()
		{
			var type = typeof(TResult);
			var propertiesWithOrder = new List<Tuple<PropertyInfo, int>>();

			foreach (var property in type.GetProperties())
			{
				if (property.GetCustomAttribute<IgnoreAttribute>() != null)
				{
					continue;
				}

				var orderAttribute = property.GetCustomAttribute<OrderAttribute>();
				var order = orderAttribute?.Order ?? int.MaxValue;

				propertiesWithOrder.Add(Tuple.Create(property, order));
			}

			propertiesWithOrder.Sort((p1, p2) => p1.Item2.CompareTo(p2.Item2));

			var createColumnMethod = GetType().GetMethod(nameof(CreateColumn), BindingFlags.NonPublic | BindingFlags.Instance);

			foreach (var propertyTuple in propertiesWithOrder)
			{
				var property = propertyTuple.Item1;
				var createColumnGeneric = createColumnMethod.MakeGenericMethod(property.PropertyType);
				var column = (Column) createColumnGeneric.Invoke(this, new object[]{property});

				columns.Add(column);
			}
		}

		private Column CreateColumn<TReturn>(PropertyInfo property)
		{
			var length = property.GetCustomAttribute<LengthAttribute>()?.Length ?? 20;
			var format = property.GetCustomAttribute<ValueFormatAttribute>()?.Format;
			var hideIn = property.GetCustomAttribute<ShowAttribute>()?.Type ?? ShowIn.All;

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

		public void PrintHeader(params TextWriter[] writers)
		{
			foreach (var writer in writers)
			{
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

		public void PrintRow(TResult result, params TextWriter[] writers)
		{
			foreach (var column in columns)
			{
				var valueObj = column.GetterFunc(result);
				var value = column.Format != null ? string.Format(column.Format, valueObj) : valueObj.ToString();

				foreach (var writer in writers)
				{
					if (!ShouldShow(column, writer))
						continue;

					writer.Write("  ");
					writer.Write(value.PadRight(column.Width - 2, ' '));
				}
			}

			foreach (var writer in writers)
			{
				writer.WriteLine();
			}
		}

		public void PreviewRow(TResult result)
		{
			var writer = Console.Out;

			foreach (var column in columns)
			{
				if (!ShouldShow(column, writer))
					continue;

				var valueObj = column.GetterFunc(result);
				var value = column.Format != null ? string.Format(column.Format, valueObj) : valueObj.ToString();

				writer.Write("  ");
				writer.Write(value.PadRight(column.Width - 2, ' '));
			}

			Console.SetCursorPosition(0, Console.CursorTop);
		}

		private bool ShouldShow(Column column, TextWriter writer)
		{
			return column.ShowIn == ShowIn.All || (column.ShowIn == ShowIn.Console && writer == Console.Out) ||
			       (column.ShowIn == ShowIn.File && writer != Console.Out);
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
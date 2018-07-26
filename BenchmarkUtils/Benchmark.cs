namespace BenchmarkUtils
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using Enums;
	using FileMode = Enums.FileMode;

	public class Benchmark<TResult, TJob>
		where TJob : IBenchmarkJob<TResult>
	{
		protected BenchmarkFormatter<TResult> BenchmarkFormatter = new BenchmarkFormatter<TResult>();

		private bool withConsole = true;
		private readonly List<FileOutput> fileOutputs = new List<FileOutput>();
		private TextWriter[] textWritersArray;

		public void AddFileOutput(string folder = "Benchmarks/", FileMode fileMode = FileMode.Append, NamingConvention namingConvention = NamingConvention.Timestamp, string filename = null, Func<string> customFilenameFunc = null)
		{
			Func<string> filenameFunc;

			switch (namingConvention)
			{
				case NamingConvention.Timestamp:
					filenameFunc = () => $"{new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()}.txt";
					break;

				case NamingConvention.FixedName:
					if (string.IsNullOrEmpty(filename))
						throw new ArgumentException("Filename must not be null or empty when using FixedName naming convention.");
					filenameFunc = () => filename;
					break;

				case NamingConvention.Custom:
					if (customFilenameFunc == null)
						throw new ArgumentException("Custom filename function must not be null when using Custom naming convention.");
					filenameFunc = customFilenameFunc;
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}

			var fileOutput = new FileOutput(folder, fileMode, filenameFunc);
			fileOutputs.Add(fileOutput);
		}

		protected void BenchmarkStarted()
		{
			if (withConsole)
			{
				textWritersArray = new TextWriter[fileOutputs.Count + 1];
				textWritersArray[fileOutputs.Count] = Console.Out;
			}
			else
			{
				textWritersArray = new TextWriter[fileOutputs.Count];
			}

			for (var i = 0; i < fileOutputs.Count; i++)
			{
				var fileOutput = fileOutputs[i];
				Directory.CreateDirectory(fileOutput.Folder);
				var path = $"{fileOutput.Folder}{fileOutput.FilenameFunc()}";
				var writer = new StreamWriter(path, fileOutput.FileMode == FileMode.Append);
				textWritersArray[i] = writer;
			}

			BenchmarkFormatter.PrintHeader(textWritersArray);
		}

		protected void BenchmarkEnded()
		{
			foreach (var writer in textWritersArray)
			{
				if (writer == Console.Out)
					continue;
				
				writer.Dispose();
			}
		}

		public virtual void Run(string name, params TJob[] jobs)
		{
			BenchmarkStarted();

			foreach (var job in jobs)
			{
				Run(job);
			}

			BenchmarkEnded();
		}

		protected virtual void Run(TJob job)
		{
			if (job is IPreviewableBenchmarkJob<TResult> previewableJob)
			{
				previewableJob.OnPreview += (previewResult, count, totalCount) => BenchmarkFormatter.PreviewRow(previewResult);
			}

			var result = job.Execute();

			BenchmarkFormatter.PrintRow(result, textWritersArray);

			foreach (var textWriter in textWritersArray)
			{
				textWriter.Flush();
			}
		}

		private class FileOutput
		{
			public string Folder { get; }

			public FileMode FileMode { get; }

			public Func<string> FilenameFunc { get; }

			public FileOutput(string folder, FileMode fileMode, Func<string> filenameFunc)
			{
				Folder = folder;
				FileMode = fileMode;
				FilenameFunc = filenameFunc;
			}
		}
	}
}
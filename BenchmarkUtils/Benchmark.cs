namespace BenchmarkUtils
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using Enums;
	using FileMode = Enums.FileMode;

	/// <summary>
	/// Class for running benchmarks.
	/// </summary>
	/// <typeparam name="TJob">Type of the job to be benchmarked.</typeparam>
	/// <typeparam name="TResult">Type of the result returned by the job.</typeparam>
	public class Benchmark<TJob, TResult>
		where TJob : IBenchmarkJob<TResult>
	{
		protected BenchmarkTableOutput<TResult> BenchmarkTableOutput = new BenchmarkTableOutput<TResult>();

		protected bool WithConsole;
		protected readonly List<FileOutput> FileOutputs = new List<FileOutput>();
		protected TextWriter[] TextWritersArray;
		protected bool CursorVisible;

		/// <summary>
		/// </summary>
		/// <param name="enableConsoleOutput">Whether console output should be enabled.</param>
		public Benchmark(bool enableConsoleOutput = true)
		{
			WithConsole = enableConsoleOutput;
		}

		/// <summary>
		/// Adds file output to the benchmark.
		/// </summary>
		/// <remarks>
		/// Multiple file outputs can be added.
		/// 
		/// filename must be supplied when namingConvention == FixedName.
		/// customFilenameFunc must be supplied when namingConvention == Custom.
		/// </remarks>
		/// <param name="folder">Folder to store benchmarks.</param>
		/// <param name="fileMode">Whether data should be appended or overwritten.</param>
		/// <param name="namingConvention">Naming convention of output files.</param>
		/// <param name="filename">Name of the file. </param>
		/// <param name="customFilenameFunc">Function that returns the name for an output file.</param>
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
			FileOutputs.Add(fileOutput);
		}

		/// <summary>
		/// Enables or disables console output.
		/// </summary>
		/// <param name="enable"></param>
		public void SetConsoleOutput(bool enable)
		{
			WithConsole = enable;
		}

		/// <summary>
		/// Called before a benchmark starts. Setups the benchmark.
		/// </summary>
		protected virtual void BenchmarkStarted(string name)
		{
			if (WithConsole)
			{
				TextWritersArray = new TextWriter[FileOutputs.Count + 1];
				TextWritersArray[FileOutputs.Count] = Console.Out;
				CursorVisible = Console.CursorVisible;
				Console.CursorVisible = false;
			}
			else
			{
				TextWritersArray = new TextWriter[FileOutputs.Count];
			}

			for (var i = 0; i < FileOutputs.Count; i++)
			{
				var fileOutput = FileOutputs[i];
				Directory.CreateDirectory(fileOutput.Folder);
				var path = $"{fileOutput.Folder}{fileOutput.FilenameFunc()}";
				var writer = new StreamWriter(path, fileOutput.FileMode == FileMode.Append);
				TextWritersArray[i] = writer;
			}

			BenchmarkTableOutput.PrintHeader(name, TextWritersArray);
		}

		/// <summary>
		/// Called after a benchmark ends. Cleanup after the benchmark.
		/// </summary>
		protected virtual void BenchmarkEnded()
		{
			foreach (var writer in TextWritersArray)
			{
				if (writer == Console.Out)
					continue;
				
				writer.Dispose();
			}

			if (WithConsole)
			{
				Console.CursorVisible = CursorVisible;
			}
		}

		/// <summary>
		/// Executes a benchmark of given jobs with a given name.
		/// </summary>
		/// <param name="jobs">Jobs to be benchmarked.</param>
		/// <param name="name">Optional name of the benchmark.</param>
		public virtual List<TResult> Run(TJob[] jobs, string name = null)
		{
            var results = new List<TResult>();

			BenchmarkStarted(name);

			foreach (var job in jobs)
			{
				var result = Run(job);
                results.Add(result);
			}

			BenchmarkEnded();

            return results;
        }

		/// <summary>
		/// Benchmarks a given job.
		/// </summary>
		/// <param name="job"></param>
		protected virtual TResult Run(TJob job)
		{
			if (WithConsole && job is IPreviewableBenchmarkJob<TResult> previewableJob)
			{
				previewableJob.OnPreview += (previewResult) => BenchmarkTableOutput.PreviewRow(previewResult);
			}

			var result = job.Execute();

			BenchmarkTableOutput.PrintRow(result, TextWritersArray);

			foreach (var textWriter in TextWritersArray)
			{
				textWriter.Flush();
			}

            return result;
        }

		protected class FileOutput
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
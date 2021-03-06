# Benchmark utilities
[![TravisCI](https://api.travis-ci.org/OndrejNepozitek/BenchmarkUtils.svg?branch=master)](https://travis-ci.org/OndrejNepozitek/BenchmarkUtils)
[![NuGet](https://img.shields.io/nuget/v/BenchmarkUtils.svg)](https://www.nuget.org/packages/BenchmarkUtils)

A simple .NET utility to benchmark the performance of (often stochastic) algorithms. The typical usage is to benchmark various configurations of a stochastic algorithm and compare how they perform both result-wise and speed-wise. **It is not meant to perform microbenchmarks.** 

<p align="center">
  <img src="http://git.n0pe.eu/BenchmarkUtils_example.gif" alt="Example output" width="800" height="212">
</p>

<p align="center">
  <i>Example of how the number of samples affects the accuracy of Monte Carlo PI estimation</i>
</p>

## Table of contents
- [Features](#features)
- [How to install](#how-to-install)
- [Example setup - PI estimation](#example-setup---pi-estimation)
- [API reference](#api-reference)

## Features

- Easily specify what columns should be included in the results table
- Output results to the console and/or save to a file
- Show intermediate results as the benchmark runs
- .NET Standard 2.0 dll

#### Column style features
- Name
- Size
- Format of values
- Order
- Whether to show the column in console/file/everywhere

## How to install
- Download the latest release via [Nuget](https://www.nuget.org/packages/BenchmarkUtils) or from [Github](https://github.com/OndrejNepozitek/BenchmarkUtils/releases/latest)
- Use the `BenchmarkUtils` namespace

## Example setup - PI estimation
We will demonstrate how to use the library on a simple example of a [monte carlo algorithm that estimates the value of pi](https://www.geeksforgeeks.org/estimating-value-pi-using-monte-carlo/). The goal is to compare how the error of the estimation depends on the number of sampled points. Because the algorithm is stochastic, we will run the benchmark ten times for each configuration (i.e. number of samples) and compute the median of errors.

As we said before, the key metric of the bechmark will be the median of errors. We will also want to show the best estimation obtained,  together with the minimum error. And the last metric will be the average time needed to run the benchmark for each configuration.

**Preparing and running the benchmark consists of 3 simple steps:**
1. Prepare a class that will hold the result of a benchmark of a single configuration.
2. Prepare a class that knows how to configure and run the algorithm with a given configuration. We call this a *job*.
3. Run the benchmark.

### Result class
We have to create a class that will hold the result of a benchmark of a single configuration. Instance of this class will correspond to a single line of the table with results. It may look like this:

```csharp
public class BenchmarkResult
{
  public string Name { get; set; }

  public double BestEstimation { get; set; }

  public double ErrorMin { get; set; }

  public double ErrorMedian { get; set; }

  public double SimulationTime { get; set; }
}
```

To specifiy how should the table with results look like, we use attributes to annonate individual properties of the class. Each such property will correspond to a single column in the table. For a detailed describtion, see the [API reference](#api-reference) section. After adding attributes to the properties, our class may now look like this:

```csharp
public class BenchmarkResult
{
  [Order(1)]
  [Length(25)]
  [Name("Name")]
  public string Name { get; set; }

  [Order(2)]
  [Length(20)]
  [Name("Best estimation")]
  public double BestEstimation { get; set; }

  [Order(3)]
  [Length(25)]
  [Name("Error min")]
  public double ErrorMin { get; set; }

  [Order(4)]
  [Length(25)]
  [Name("Error median")]
  public double ErrorMedian { get; set; }

  [Order(5)]
  [Length(15)]
  [Name("Avg time")]
  [ValueFormat("{0:N4} s")]
  public double AverageTime { get; set; }
}
```

### Job class
We have to create a class that implements the `IBenchmarkJob<TResult>` interface where `TResult` is the result class that we prepared in the previous step. This interface provides one method (`TResult Execute();`) that should execute the job it represents and return the result. In our case, we want the job to run 10 iterations of our monte carlo algorithm with a predefined number of samples, gathering statistics about how the algorithm behaved. The class may look like this:

```csharp
public class BenchmarkJob : IBenchmarkJob<BenchmarkResult>
{
  private readonly int samplesCount;

  public BenchmarkJob(int samplesCount)
  {
    this.samplesCount = samplesCount;
  }
  
  public BenchmarkResult Execute()
  {
    for (var i = 0; i < 10; i++)
    {
      // use a given number of samples to estimate the value of pi
      // gather statistics about errors, best estimation and needed time
    }

    return new BenchmarkResult()
    {
      Name = $"{samplesCount} samples",
      BestEstimation = // best recorded estimation,
      ErrorMin = // minimum error,
      ErrorMedian = // median of errors,
      AverageTime = // average computation time,
    };
  }
}
```

### Running the benchmark
Finally, we will run the benchmark:

```csharp
var benchmark = new Benchmark<BenchmarkJob, BenchmarkResult>();
benchmark.AddFileOutput();

var jobs = new BenchmarkJob[]
{
  new BenchmarkJob(50),
  new BenchmarkJob(500),
  new BenchmarkJob(5000),
  new BenchmarkJob(50000),
  new BenchmarkJob(500000),
  new BenchmarkJob(5000000),
  new BenchmarkJob(50000000),
  new BenchmarkJob(500000000),
};

benchmark.Run(jobs, "PI estimation");
```

And the result will look like this:

```
 << PI estimation >>
---------------------------------------------------------------------------------------------------------------
 Name                    | Best estimation   | Error min              | Error median           | Avg time     |
---------------------------------------------------------------------------------------------------------------
 50 samples                3.12                0.021592653589793        0.138407346410207        0.0000 s     
 500 samples               3.144               0.00240734641020701      0.026407346410207        0.0000 s     
 5000 samples              3.1424              0.000807346410206744     0.025592653589793        0.0004 s     
 50000 samples             3.14128             0.000312653589793044     0.00583265358979324      0.0099 s     
 500000 samples            3.141936            0.000343346410206724     0.00175265358979315      0.0702 s     
 5000000 samples           3.1415536           3.90535897931699E-05     0.000681746410206685     0.6490 s     
 50000000 samples          3.14158032          1.23335897930232E-05     0.00025793358979298      6.1894 s     
 500000000 samples         3.14160888          1.62264102070431E-05     3.2266410206816E-05      57.9247 s    
```

## API reference

### `Benchmark` class
Documentation comments are present in the [code](https://github.com/OndrejNepozitek/BenchmarkUtils/blob/master/BenchmarkUtils/Benchmark.cs).

### Attributes

#### `LengthAttribute`
Sets the length of a corresponding column. Optional, defaults to 20.

##### Constructor parameters
`int length` - Length of the column.

##### Example
```csharp
public class DummyClass
{
 [Length(20)]
 public int DummyColumn { get; set; }
}
```

#### `NameAttribute`
Specifies the column name that will be displayed in the header of the result table. Required.

##### Constructor parameters
`string name` - Name of the column.

##### Example
```csharp
public class DummyClass
{
 [Name("Average error")]
 public int DummyColumn { get; set; }
}
```

#### `OrderAttribute`
Specifies the order of the column as seen in the results table. Optional. Columns without the attribute will be placed at the end of the table.

##### Constructor parameters
`int order` - Order of the column.

##### Example
```csharp
public class DummyClass
{
 [Order(1)]
 public int DummyColumn1 { get; set; }
 
 [Order(2)]
 public int DummyColumn2 { get; set; }
}
```

#### `ShowAttribute`
Specifies where should be the column shown. Optional, defaults to *everywhere*.

The possibilities are:
- Only in console output
- Only in file output
- Everywhere (default)
- Nowhere

##### Constructor parameters
`ShowIn type` - Where should be the column shown.

##### Example
```csharp
public class DummyClass
{
 [Show(ShowIn.Console)]
 public int DummyColumn { get; set; }
}
```

#### `ValueFormatAttribute`
Specifies the format of values contained in the column. Optional, defaults to a call to `ToString` on the value.

The format string is later used as follows: `string.Format(formatString, value);` See the [microsoft docs](https://msdn.microsoft.com/en-us/library/system.string.format(v=vs.110).aspx) for more information.
 
##### Constructor parameters
`string format` - Format of the value.

##### Example
```csharp
public class DummyClass
{
 [ValueFormat("{0:N4} s")]
 public double DummyColumn { get; set; }
}
```

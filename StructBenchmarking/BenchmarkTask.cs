using System;
using System.Diagnostics;
using System.Text;
using NUnit.Framework;

namespace StructBenchmarking
{
    public class Benchmark : IBenchmark
	{
        public double MeasureDurationInMs(ITask task, int repetitionCount)
        {
            GC.Collect();                   // Эти две строчки нужны, чтобы уменьшить вероятность того,
            GC.WaitForPendingFinalizers();  // что Garbadge Collector вызовется в середине измерений
                                            // и как-то повлияет на них.
                                            
			task.Run();
            var timer = new Stopwatch();
            timer.Start();
            for (int i = 0; i < repetitionCount; i++)
            {
                task.Run();
            }
            timer.Stop();
            return (double)timer.ElapsedMilliseconds / repetitionCount;
        }
	}

    [TestFixture]
    public class RealBenchmarkUsageSample
    {
        [Test]
        public void StringConstructorFasterThanStringBuilder()
        {
            var bench = new Benchmark();
            var constr = bench.MeasureDurationInMs(new StringConstructorTask(),  100);
            var builder = bench.MeasureDurationInMs(new StringBuilderTask(), 100);
            Assert.Less(constr, builder);
        }
    }

    public class StringBuilderTask : ITask
    {
        public void Run()
        {
            var builder = new StringBuilder();
            for (int i = 0; i < 1000000; i++)
            {
                builder.Append('a');
            }

            var str = builder.ToString();
        }
    }

    public class StringConstructorTask : ITask
    {
        public void Run()
        {
            var str = new string('a', 1000000);
        }
    }
}
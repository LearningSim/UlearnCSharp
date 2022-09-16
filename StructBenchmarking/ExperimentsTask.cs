using System;
using System.Collections.Generic;
using System.Linq;

namespace StructBenchmarking
{
    public class Experiments
    {
        public static ChartData BuildChartDataForArrayCreation(IBenchmark benchmark, int repetitionsCount)
        {
            ITask CreateClassTask(int size) => new ClassArrayCreationTask(size);
            ITask CreateStructTask(int size) => new StructArrayCreationTask(size);
            return new ChartData
            {
                Title = "Create array",
                ClassPoints = MakeExperiments(benchmark, repetitionsCount, CreateClassTask),
                StructPoints = MakeExperiments(benchmark, repetitionsCount, CreateStructTask),
            };
        }

        public static ChartData BuildChartDataForMethodCall(IBenchmark benchmark, int repetitionsCount)
        {
            ITask CreateClassTask(int size) => new MethodCallWithClassArgumentTask(size);
            ITask CreateStructTask(int size) => new MethodCallWithStructArgumentTask(size);
            return new ChartData
            {
                Title = "Call method with argument",
                ClassPoints = MakeExperiments(benchmark, repetitionsCount, CreateClassTask),
                StructPoints = MakeExperiments(benchmark, repetitionsCount, CreateStructTask),
            };
        }

        private static List<ExperimentResult> MakeExperiments(IBenchmark benchmark, int repetitionsCount, Func<int, ITask> createTask)
        {
            return Constants.FieldCounts
                .Select(size => MakeExperiment(benchmark, repetitionsCount, size, createTask))
                .ToList();
        }

        private static ExperimentResult MakeExperiment(IBenchmark benchmark, int repetitionsCount, int fieldCount, Func<int, ITask> createTask)
        {
            var time = benchmark.MeasureDurationInMs(createTask(fieldCount), repetitionsCount);
            return new ExperimentResult(fieldCount, time);
        }
    }
}
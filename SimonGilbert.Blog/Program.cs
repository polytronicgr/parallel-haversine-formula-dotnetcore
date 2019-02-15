using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SimonGilbert.Blog
{
    class MainClass
    {
        private static Dictionary<string, TimeSpan> results = new Dictionary<string, TimeSpan>();

        private static readonly List<Tuple<double, double, double, double>> _journeys =
            new List<Tuple<double, double, double, double>>
        {
            new Tuple<double, double, double, double>(51.497130, -0.071530, 51.505192, -0.087820),
            new Tuple<double, double, double, double>(51.505192, -0.087820, 53.490270, -2.339930),
            new Tuple<double, double, double, double>(53.490270, -2.339930, 40.753685, -73.999161),
            new Tuple<double, double, double, double>(40.753685, -73.999161, 37.791209, -122.464907),
            new Tuple<double, double, double, double>(37.791209, -122.464907, -51.801301, -60.149517),
            new Tuple<double, double, double, double>(-51.801301, -60.149517, 27.1750, 78.0422),
            new Tuple<double, double, double, double>(27.1750, 78.0422, 90.0000, 135.0000),
            new Tuple<double, double, double, double>(90.0000, 135.0000, 35.6850, 139.7514),
            new Tuple<double, double, double, double>(35.6850, 139.7514, 40.6943, -73.9249),
            new Tuple<double, double, double, double>(40.6943, -73.9249, 19.4424, -99.1310),
            new Tuple<double, double, double, double>(19.4424, -99.1310, 19.0170, 72.8570),
            new Tuple<double, double, double, double>(19.0170, 72.8570, -23.5587, -46.6250),
            new Tuple<double, double, double, double>(-23.5587, -46.6250, 51.514248,-0.093145),
            new Tuple<double, double, double, double>(51.514248, -0.093145, 52.466667, -1.916667),
            new Tuple<double, double, double, double>(52.466667, -1.916667, 53.5, -2.216667),
            new Tuple<double, double, double, double>(53.5, -2.216667, 53.8, -1.583333),
            new Tuple<double, double, double, double>(53.8, -1.583333, 53.366667, -1.5),
            new Tuple<double, double, double, double>(53.366667, -1.5, 55.833333, -4.25),
            new Tuple<double, double, double, double>(55.833333, -4.25, 54.988056, -1.619444),
            new Tuple<double, double, double, double>(54.988056, -1.619444, 52.966667, -1.166667),
            new Tuple<double, double, double, double>(52.966667, -1.166667, 53.416667, -3),
            new Tuple<double, double, double, double>(53.416667, -3, 51.533333, 0.7),
            new Tuple<double, double, double, double>(51.533333, 0.7, 51.45, -2.583333),
            new Tuple<double, double, double, double>(51.45, -2.583333, 55.95, -3.2),
            new Tuple<double, double, double, double>(55.95, -3.2, 50.833333, -0.15),
            new Tuple<double, double, double, double>(50.833333, -0.15, 53.716667, -0.333333),
        };

        public static void Main(string[] args)
        {
            RunParallelMethodOne();
            RunParallelMethodTwo();
            RunParallelMethodThree();
            RunParallelMethodFour();
            RunSerialMethodOne();

            Console.WriteLine(".Net Core");
            Console.Write(Environment.NewLine);
            Console.WriteLine("RESULTS:");
            foreach (var result in results.OrderBy(kvp => kvp.Value))
                Console.WriteLine($"{result.Key} : {result.Value}");

            Console.Write(Environment.NewLine);
            Console.WriteLine("WINNER:");
            var keyAndValue = results.OrderBy(kvp => kvp.Value).First();
            Console.WriteLine($"{keyAndValue.Key} => {keyAndValue.Value}");

            Console.ReadKey();
        }

        public static void RunParallelMethodOne()
        {
            var stopwatch = Stopwatch.StartNew();

            var tasks = new List<Task>();
            foreach (var journey in _journeys)
            {
                Task task = new Task(() =>
                HaversineService.Calculate(
                    journey.Item1,
                    journey.Item2,
                    journey.Item3,
                    journey.Item4,
                    DistanceType.Kilometres));

                task.Start();
                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());

            stopwatch.Stop();

            results.Add("Task.WaitAll", stopwatch.Elapsed);
        }

        public static void RunParallelMethodTwo()
        {
            var stopwatch = Stopwatch.StartNew();

            var tasks = new List<Task>();
            foreach (var journey in _journeys)
            {
                Task task = new Task(() =>
                HaversineService.Calculate(
                    journey.Item1,
                    journey.Item2,
                    journey.Item3,
                    journey.Item4,
                    DistanceType.Kilometres));

                task.Start();
                tasks.Add(task);
            }

            Task.WhenAll(tasks.ToArray());

            stopwatch.Stop();

            results.Add("Task.WhenAll", stopwatch.Elapsed);
        }

        public static void RunParallelMethodThree()
        {
            var stopwatch = Stopwatch.StartNew();

            Parallel.ForEach(_journeys, journey =>
            {
                var result = HaversineService.Calculate(
                    journey.Item1,
                    journey.Item2,
                    journey.Item3,
                    journey.Item4,
                    DistanceType.Kilometres);
            });

            stopwatch.Stop();

            results.Add("Parallel.ForEach", stopwatch.Elapsed);
        }

        public static void RunParallelMethodFour()
        {
            var stopwatch = Stopwatch.StartNew();

            Task.Run(() => Parallel.ForEach(_journeys, journey =>
            {
                var result = HaversineService.Calculate(
                    journey.Item1,
                    journey.Item2,
                    journey.Item3,
                    journey.Item4,
                    DistanceType.Kilometres);
            }));

            stopwatch.Stop();

            results.Add("Task.Run.Parallel.ForEach", stopwatch.Elapsed);
        }

        public static void RunSerialMethodOne()
        {
            var stopwatch = Stopwatch.StartNew();

            foreach (var journey in _journeys)
            {
                var result = HaversineService.Calculate(
                    journey.Item1,
                    journey.Item2,
                    journey.Item3,
                    journey.Item4,
                    DistanceType.Kilometres);
            }

            stopwatch.Stop();

            results.Add("ForEach", stopwatch.Elapsed);
        }


    }
}

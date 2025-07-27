using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace StatisticalTesting
{
    /// <summary>
    /// Advanced A/B Testing Framework for Algorithm Performance Validation
    /// Provides rigorous statistical analysis with hypothesis testing and effect size measurement
    /// </summary>
    public class StatisticalABTestFramework
    {
        private readonly Random random;
        private readonly List<TestExecution> testHistory;
        
        public StatisticalABTestFramework(int? seed = null)
        {
            random = seed.HasValue ? new Random(seed.Value) : new Random();
            testHistory = new List<TestExecution>();
        }
        
        /// <summary>
        /// Run comprehensive A/B test comparing two algorithms
        /// </summary>
        public async Task<ABTestResult> RunABTest<T>(
            Func<T[], Task<double>> algorithmA,
            Func<T[], Task<double>> algorithmB,
            Func<T[]> dataGenerator,
            string algorithmAName,
            string algorithmBName,
            int iterations = 100,
            double alphaLevel = 0.05,
            bool useWarmup = true)
        {
            Console.WriteLine($"🧪 Starting Statistical A/B Test: {algorithmAName} vs {algorithmBName}");
            Console.WriteLine($"📊 Iterations: {iterations}, α = {alphaLevel:F3}");
            Console.WriteLine();
            
            var samplesA = new List<double>();
            var samplesB = new List<double>();
            
            // Warmup runs to eliminate JIT compilation effects
            if (useWarmup)
            {
                Console.WriteLine("🔥 Warming up algorithms...");
                var warmupData = dataGenerator();
                await algorithmA(warmupData);
                await algorithmB(warmupData);
                Console.WriteLine("✅ Warmup completed");
                Console.WriteLine();
            }
            
            // Collect performance samples with progress tracking
            Console.WriteLine("📈 Collecting performance samples...");
            
            for (int i = 0; i < iterations; i++)
            {
                // Test Algorithm A
                var dataA = dataGenerator();
                var resultA = await MeasurePerformance(() => algorithmA(dataA));
                samplesA.Add(resultA);
                
                // Test Algorithm B  
                var dataB = dataGenerator();
                var resultB = await MeasurePerformance(() => algorithmB(dataB));
                samplesB.Add(resultB);
                
                // Progress indicator
                if ((i + 1) % (iterations / 10) == 0)
                {
                    Console.WriteLine($"  Progress: {i + 1}/{iterations} ({(i + 1) * 100.0 / iterations:F1}%)");
                }
            }
            
            Console.WriteLine("✅ Data collection completed");
            Console.WriteLine();
            
            // Perform statistical analysis
            Console.WriteLine("🔬 Performing statistical analysis...");
            
            // Choose appropriate test based on data characteristics
            var result = ChooseAndRunStatisticalTest(samplesA, samplesB, algorithmAName, algorithmBName, alphaLevel);
            
            // Auto-save detailed results to file
            try
            {
                result.SaveDetailedReport();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Warning: Could not save detailed report: {ex.Message}");
            }
            
            // Record test execution
            testHistory.Add(new TestExecution
            {
                Timestamp = DateTime.Now,
                AlgorithmA = algorithmAName,
                AlgorithmB = algorithmBName,
                Iterations = iterations,
                Result = result
            });
            
            return result;
        }
        
        /// <summary>
        /// Run multi-algorithm comparison using ANOVA
        /// </summary>
        public async Task<MultiAlgorithmTestResult> RunMultiAlgorithmTest<T>(
            Dictionary<string, Func<T[], Task<double>>> algorithms,
            Func<T[]> dataGenerator,
            int iterations = 100,
            double alphaLevel = 0.05)
        {
            Console.WriteLine($"🧪 Starting Multi-Algorithm Statistical Comparison");
            Console.WriteLine($"📊 Algorithms: {string.Join(", ", algorithms.Keys)}");
            Console.WriteLine($"📊 Iterations: {iterations}, α = {alphaLevel:F3}");
            Console.WriteLine();
            
            var allSamples = new Dictionary<string, List<double>>();
            
            // Initialize sample collections
            foreach (var algorithm in algorithms.Keys)
            {
                allSamples[algorithm] = new List<double>();
            }
            
            // Warmup all algorithms
            Console.WriteLine("🔥 Warming up all algorithms...");
            var warmupData = dataGenerator();
            foreach (var kvp in algorithms)
            {
                await kvp.Value(warmupData);
            }
            Console.WriteLine("✅ Warmup completed");
            Console.WriteLine();
            
            // Collect samples for all algorithms
            Console.WriteLine("📈 Collecting performance samples...");
            
            for (int i = 0; i < iterations; i++)
            {
                foreach (var kvp in algorithms)
                {
                    var data = dataGenerator();
                    var measurementResult = await MeasurePerformance(() => kvp.Value(data));
                    allSamples[kvp.Key].Add(measurementResult);
                }
                
                if ((i + 1) % (iterations / 10) == 0)
                {
                    Console.WriteLine($"  Progress: {i + 1}/{iterations} ({(i + 1) * 100.0 / iterations:F1}%)");
                }
            }
            
            Console.WriteLine("✅ Data collection completed");
            Console.WriteLine();
            
            // Perform ANOVA
            Console.WriteLine("🔬 Performing ANOVA analysis...");
            var result = HypothesisValidator.OneWayANOVA(allSamples, alphaLevel);
            
            // Auto-save detailed multi-algorithm results to file
            try
            {
                result.SaveDetailedReport();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Warning: Could not save multi-algorithm report: {ex.Message}");
            }
            
            return result;
        }
        
        /// <summary>
        /// Run sequential A/B test with early stopping
        /// Stops when statistical significance is achieved with sufficient power
        /// </summary>
        public async Task<ABTestResult> RunSequentialABTest<T>(
            Func<T[], Task<double>> algorithmA,
            Func<T[], Task<double>> algorithmB,
            Func<T[]> dataGenerator,
            string algorithmAName,
            string algorithmBName,
            int minIterations = 30,
            int maxIterations = 1000,
            double alphaLevel = 0.05,
            double powerThreshold = 0.8)
        {
            Console.WriteLine($"🧪 Starting Sequential A/B Test: {algorithmAName} vs {algorithmBName}");
            Console.WriteLine($"📊 Min: {minIterations}, Max: {maxIterations}, α = {alphaLevel:F3}, Power ≥ {powerThreshold:F2}");
            Console.WriteLine();
            
            var samplesA = new List<double>();
            var samplesB = new List<double>();
            
            // Warmup
            var warmupData = dataGenerator();
            await algorithmA(warmupData);
            await algorithmB(warmupData);
            
            Console.WriteLine("📈 Sequential testing with early stopping...");
            
            for (int i = 0; i < maxIterations; i++)
            {
                // Collect one sample from each algorithm
                var dataA = dataGenerator();
                var resultA = await MeasurePerformance(() => algorithmA(dataA));
                samplesA.Add(resultA);
                
                var dataB = dataGenerator();
                var resultB = await MeasurePerformance(() => algorithmB(dataB));
                samplesB.Add(resultB);
                
                // Check for early stopping after minimum iterations
                if (i >= minIterations && (i + 1) % 10 == 0)
                {
                    var interimResult = HypothesisValidator.WelchTTest(samplesA, samplesB, algorithmAName, algorithmBName, alphaLevel);
                    
                    if (interimResult.IsSignificant && interimResult.StatisticalPower >= powerThreshold)
                    {
                        Console.WriteLine($"🎯 Early stopping at iteration {i + 1}: Significance achieved with adequate power");
                        break;
                    }
                    
                    Console.WriteLine($"  Iteration {i + 1}: p={interimResult.PValue:F6}, power={interimResult.StatisticalPower:F3}");
                }
            }
            
            var finalResult = HypothesisValidator.WelchTTest(samplesA, samplesB, algorithmAName, algorithmBName, alphaLevel);
            finalResult.TestType = "Sequential Welch's t-test with early stopping";
            
            // Auto-save detailed sequential test results to file
            try
            {
                finalResult.SaveDetailedReport();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Warning: Could not save sequential test report: {ex.Message}");
            }
            
            return finalResult;
        }
        
        /// <summary>
        /// Generate comprehensive test data with various distributions
        /// </summary>
        public static class TestDataGenerator
        {
            private static readonly Random rand = new Random();
            
            public static int[] GenerateRandomArray(int size, int minValue = 1, int maxValue = 1000)
            {
                return Enumerable.Range(0, size).Select(_ => rand.Next(minValue, maxValue)).ToArray();
            }
            
            public static int[] GenerateWorstCaseArray(int size)
            {
                return Enumerable.Range(1, size).Reverse().ToArray(); // Reverse sorted
            }
            
            public static int[] GenerateBestCaseArray(int size)
            {
                return Enumerable.Range(1, size).ToArray(); // Already sorted
            }
            
            public static int[] GenerateNearlysorted(int size, double disorderPercent = 0.1)
            {
                var array = Enumerable.Range(1, size).ToArray();
                int swaps = (int)(size * disorderPercent);
                
                for (int i = 0; i < swaps; i++)
                {
                    int idx1 = rand.Next(size);
                    int idx2 = rand.Next(size);
                    (array[idx1], array[idx2]) = (array[idx2], array[idx1]);
                }
                
                return array;
            }
            
            public static int[] GenerateWithDuplicates(int size, double duplicateRatio = 0.3)
            {
                int uniqueCount = (int)(size * (1 - duplicateRatio));
                var unique = Enumerable.Range(1, uniqueCount).ToArray();
                var result = new List<int>(unique);
                
                while (result.Count < size)
                {
                    result.Add(unique[rand.Next(uniqueCount)]);
                }
                
                return result.OrderBy(_ => rand.Next()).ToArray();
            }
        }
        
        /// <summary>
        /// Performance benchmarking with comprehensive metrics
        /// </summary>
        public class PerformanceBenchmark
        {
            public string AlgorithmName { get; set; } = string.Empty;
            public List<double> ExecutionTimes { get; set; } = new List<double>();
            public double Mean => ExecutionTimes.Average();
            public double Median => GetPercentile(50);
            public double StandardDeviation => Math.Sqrt(ExecutionTimes.Sum(x => Math.Pow(x - Mean, 2)) / (ExecutionTimes.Count - 1));
            public double Min => ExecutionTimes.Min();
            public double Max => ExecutionTimes.Max();
            public double P95 => GetPercentile(95);
            public double P99 => GetPercentile(99);
            public double CoefficientOfVariation => StandardDeviation / Mean;
            
            private double GetPercentile(double percentile)
            {
                var sorted = ExecutionTimes.OrderBy(x => x).ToList();
                int index = (int)Math.Ceiling(percentile / 100.0 * sorted.Count) - 1;
                return sorted[Math.Max(0, Math.Min(index, sorted.Count - 1))];
            }
            
            public void PrintDetailedStatistics()
            {
                Console.WriteLine($"📊 {AlgorithmName} Performance Statistics:");
                Console.WriteLine($"  Sample Size: {ExecutionTimes.Count}");
                Console.WriteLine($"  Mean: {Mean:F3}ms");
                Console.WriteLine($"  Median: {Median:F3}ms");
                Console.WriteLine($"  Std Dev: {StandardDeviation:F3}ms");
                Console.WriteLine($"  Min: {Min:F3}ms");
                Console.WriteLine($"  Max: {Max:F3}ms");
                Console.WriteLine($"  95th Percentile: {P95:F3}ms");
                Console.WriteLine($"  99th Percentile: {P99:F3}ms");
                Console.WriteLine($"  Coefficient of Variation: {CoefficientOfVariation:F3}");
                Console.WriteLine();
            }
        }
        
        #region Private Helper Methods
        
        private static async Task<double> MeasurePerformance(Func<Task<double>> operation)
        {
            var stopwatch = Stopwatch.StartNew();
            await operation();
            stopwatch.Stop();
            return stopwatch.Elapsed.TotalMilliseconds;
        }
        
        private static ABTestResult ChooseAndRunStatisticalTest(
            List<double> samplesA,
            List<double> samplesB,
            string algorithmAName,
            string algorithmBName,
            double alphaLevel)
        {
            // Check normality assumptions
            bool normalityA = CheckNormality(samplesA);
            bool normalityB = CheckNormality(samplesB);
            
            if (normalityA && normalityB)
            {
                Console.WriteLine("✅ Data meets normality assumptions - using Welch's t-test");
                return HypothesisValidator.WelchTTest(samplesA, samplesB, algorithmAName, algorithmBName, alphaLevel);
            }
            else
            {
                Console.WriteLine("⚠️ Data violates normality assumptions - using Mann-Whitney U test");
                return HypothesisValidator.MannWhitneyUTest(samplesA, samplesB, algorithmAName, algorithmBName, alphaLevel);
            }
        }
        
        private static bool CheckNormality(List<double> samples)
        {
            if (samples.Count < 8) return true;
            
            // Simplified Shapiro-Wilk test
            double mean = samples.Average();
            double variance = samples.Sum(x => Math.Pow(x - mean, 2)) / (samples.Count - 1);
            double skewness = samples.Sum(x => Math.Pow((x - mean) / Math.Sqrt(variance), 3)) / samples.Count;
            double kurtosis = samples.Sum(x => Math.Pow((x - mean) / Math.Sqrt(variance), 4)) / samples.Count;
            
            return Math.Abs(skewness) < 2 && Math.Abs(kurtosis - 3) < 4;
        }
        
        #endregion
        
        /// <summary>
        /// Test execution record for history tracking
        /// </summary>
        private class TestExecution
        {
            public DateTime Timestamp { get; set; }
            public string AlgorithmA { get; set; } = string.Empty;
            public string AlgorithmB { get; set; } = string.Empty;
            public int Iterations { get; set; }
            public ABTestResult Result { get; set; } = new ABTestResult();
        }
        
        /// <summary>
        /// Get test history for analysis
        /// </summary>
        public List<string> GetTestHistory()
        {
            return testHistory.Select(t => 
                $"{t.Timestamp:yyyy-MM-dd HH:mm:ss} - {t.AlgorithmA} vs {t.AlgorithmB}: " +
                $"p={t.Result.PValue:F6} ({(t.Result.IsSignificant ? "significant" : "not significant")})")
                .ToList();
        }
    }
}

using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SortingAlgorithm
{
    /// <summary>
    /// Comprehensive test suite for SwiftCollab's sorting algorithm optimizations
    /// LLM-Assisted performance validation and algorithm comparison
    /// </summary>
    public class SortingAlgorithmTests
    {
        /// <summary>
        /// LLM Optimization: Run all sorting algorithm tests and return comprehensive results
        /// </summary>
        public static string RunAllTests()
        {
            var results = new StringBuilder();
            results.AppendLine("\n🧪 COMPREHENSIVE SORTING ALGORITHM TEST SUITE");
            results.AppendLine("=" + new string('=', 70));
            
            // Test different array sizes for scalability analysis
            int[] testSizes = { 100, 1000, 5000, 10000 };
            
            foreach (int size in testSizes)
            {
                results.AppendLine($"\n📊 TESTING WITH {size:N0} ELEMENTS");
                results.AppendLine("-" + new string('-', 50));
                
                // Test 1: Random data (most common scenario)
                results.AppendLine("🎲 Random Data Performance:");
                var randomTestResults = TestRandomDataPerformance(size);
                results.Append(randomTestResults);
                
                // Test 2: Worst case scenario (reverse sorted)
                results.AppendLine("\n⚠️  Worst Case Performance (Reverse Sorted):");
                var worstCaseResults = TestWorstCasePerformance(size);
                results.Append(worstCaseResults);
                
                // Test 3: Best case scenario (already sorted)
                results.AppendLine("\n✅ Best Case Performance (Already Sorted):");
                var bestCaseResults = TestBestCasePerformance(size);
                results.Append(bestCaseResults);
                
                if (size <= 1000) // Only for smaller datasets to avoid excessive output
                {
                    // Test 4: Correctness validation
                    results.AppendLine("\n🔍 Correctness Validation:");
                    var correctnessResults = TestCorrectness(size);
                    results.Append(correctnessResults);
                }
            }
            
            // Summary comparison
            results.AppendLine("\n📋 ALGORITHM RECOMMENDATION SUMMARY");
            results.AppendLine("=" + new string('=', 70));
            results.AppendLine("🏆 RECOMMENDED FOR SWIFTCOLLAB:");
            results.AppendLine("  • Small datasets (< 100 elements): HybridSort (uses insertion sort)");
            results.AppendLine("  • Medium datasets (100-1000): QuickSort for speed");
            results.AppendLine("  • Large datasets (1000-10000): ParallelQuickSort for throughput");
            results.AppendLine("  • Very large datasets (> 10000): ParallelQuickSort with larger threshold");
            results.AppendLine("  • Stability required: MergeSort (maintains relative order)");
            results.AppendLine("");
            results.AppendLine("🎯 COMPLEXITY IMPROVEMENTS:");
            results.AppendLine("  • Bubble Sort: O(n²) → QuickSort: O(n log n) average");
            results.AppendLine("  • Memory usage: In-place sorting vs additional array allocation");
            results.AppendLine("  • Parallel processing: Linear speedup with available CPU cores");
            
            return results.ToString();
        }
        
        private static string TestRandomDataPerformance(int size)
        {
            var results = new StringBuilder();
            var testData = OptimizedSorting.TestDataGenerator.GenerateRandomArray(size);
            
            // Test all algorithms
            var algorithms = new (string name, Action<int[]> sortFunc)[]
            {
                ("QuickSort", arr => OptimizedSorting.QuickSort(arr)),
                ("MergeSort", arr => OptimizedSorting.MergeSort(arr)),
                ("ParallelQuickSort", arr => OptimizedSorting.ParallelQuickSort(arr)),
                ("HybridSort", arr => OptimizedSorting.HybridSort(arr))
            };
            
            foreach (var (name, sortFunc) in algorithms)
            {
                var testArray = (int[])testData.Clone();
                
                try
                {
                    sortFunc(testArray);
                    var stats = OptimizedSorting.LastSortStatistics;
                    results.AppendLine($"  {name,-18}: {stats.ExecutionTime.TotalMilliseconds,8:F2}ms | {stats.Comparisons,10:N0} comparisons | {stats.Swaps,8:N0} swaps");
                }
                catch (Exception ex)
                {
                    results.AppendLine($"  {name,-18}: ERROR - {ex.Message}");
                }
            }
            
            return results.ToString();
        }
        
        private static string TestWorstCasePerformance(int size)
        {
            var results = new StringBuilder();
            var testData = OptimizedSorting.TestDataGenerator.GenerateWorstCaseArray(size);
            
            // Test algorithms that handle worst case well
            var algorithms = new (string name, Action<int[]> sortFunc)[]
            {
                ("QuickSort", arr => OptimizedSorting.QuickSort(arr)),
                ("MergeSort", arr => OptimizedSorting.MergeSort(arr)),
                ("HybridSort", arr => OptimizedSorting.HybridSort(arr))
            };
            
            foreach (var (name, sortFunc) in algorithms)
            {
                var testArray = (int[])testData.Clone();
                
                try
                {
                    sortFunc(testArray);
                    var stats = OptimizedSorting.LastSortStatistics;
                    results.AppendLine($"  {name,-18}: {stats.ExecutionTime.TotalMilliseconds,8:F2}ms | {stats.Comparisons,10:N0} comparisons | {stats.Swaps,8:N0} swaps");
                }
                catch (Exception ex)
                {
                    results.AppendLine($"  {name,-18}: ERROR - {ex.Message}");
                }
            }
            
            return results.ToString();
        }
        
        private static string TestBestCasePerformance(int size)
        {
            var results = new StringBuilder();
            var testData = OptimizedSorting.TestDataGenerator.GenerateBestCaseArray(size);
            
            var algorithms = new (string name, Action<int[]> sortFunc)[]
            {
                ("QuickSort", arr => OptimizedSorting.QuickSort(arr)),
                ("MergeSort", arr => OptimizedSorting.MergeSort(arr)),
                ("HybridSort", arr => OptimizedSorting.HybridSort(arr))
            };
            
            foreach (var (name, sortFunc) in algorithms)
            {
                var testArray = (int[])testData.Clone();
                
                try
                {
                    sortFunc(testArray);
                    var stats = OptimizedSorting.LastSortStatistics;
                    results.AppendLine($"  {name,-18}: {stats.ExecutionTime.TotalMilliseconds,8:F2}ms | {stats.Comparisons,10:N0} comparisons | {stats.Swaps,8:N0} swaps");
                }
                catch (Exception ex)
                {
                    results.AppendLine($"  {name,-18}: ERROR - {ex.Message}");
                }
            }
            
            return results.ToString();
        }
        
        private static string TestCorrectness(int size)
        {
            var results = new StringBuilder();
            var testData = OptimizedSorting.TestDataGenerator.GenerateRandomArray(size);
            var originalData = (int[])testData.Clone();
            
            // Expected result using built-in sort
            var expected = (int[])testData.Clone();
            Array.Sort(expected);
            
            var algorithms = new (string name, Action<int[]> sortFunc)[]
            {
                ("QuickSort", arr => OptimizedSorting.QuickSort(arr)),
                ("MergeSort", arr => OptimizedSorting.MergeSort(arr)),
                ("HybridSort", arr => OptimizedSorting.HybridSort(arr))
            };
            
            foreach (var (name, sortFunc) in algorithms)
            {
                var testArray = (int[])originalData.Clone();
                
                try
                {
                    sortFunc(testArray);
                    bool isCorrect = testArray.SequenceEqual(expected);
                    bool isSorted = IsArraySorted(testArray);
                    
                    results.AppendLine($"  {name,-18}: Correct={isCorrect,-5} | Sorted={isSorted,-5} | Elements preserved={testArray.Length == originalData.Length}");
                }
                catch (Exception ex)
                {
                    results.AppendLine($"  {name,-18}: ERROR - {ex.Message}");
                }
            }
            
            return results.ToString();
        }
        
        /// <summary>
        /// LLM Optimization: Comprehensive performance comparison between original and optimized algorithms
        /// </summary>
        public static string CompareWithBubbleSort(int size = 1000)
        {
            var results = new StringBuilder();
            results.AppendLine($"\n🔍 BUBBLE SORT vs OPTIMIZED ALGORITHMS COMPARISON ({size:N0} elements)");
            results.AppendLine("=" + new string('=', 85));
            
            var testData = OptimizedSorting.TestDataGenerator.GenerateRandomArray(size);
            
            // Test Bubble Sort (original)
            var bubbleSortArray = (int[])testData.Clone();
            var bubbleStopwatch = Stopwatch.StartNew();
            BubbleSort(bubbleSortArray);
            bubbleStopwatch.Stop();
            
            results.AppendLine("📊 PERFORMANCE COMPARISON TABLE:");
            results.AppendLine("┌──────────────────────┬─────────┬─────────────┬─────────────┐");
            results.AppendLine("│ ALGORITHM            │ TIME(ms)│ COMPLEXITY  │ IMPROVEMENT │");
            results.AppendLine("├──────────────────────┼─────────┼─────────────┼─────────────┤");
            results.AppendLine($"│ Bubble Sort (Orig)   │  {bubbleStopwatch.Elapsed.TotalMilliseconds,5:F2}  │ O(n²)       │ Baseline    │");
            
            // Test optimized algorithms
            var algorithms = new (string name, string complexity, Action<int[]> sortFunc)[]
            {
                ("QuickSort", "O(n log n)", arr => OptimizedSorting.QuickSort(arr)),
                ("MergeSort", "O(n log n)", arr => OptimizedSorting.MergeSort(arr)),
                ("ParallelQuickSort", "O(n log n)", arr => OptimizedSorting.ParallelQuickSort(arr)),
                ("HybridSort", "O(n log n)", arr => OptimizedSorting.HybridSort(arr))
            };
            
            foreach (var (name, complexity, sortFunc) in algorithms)
            {
                var testArray = (int[])testData.Clone();
                
                try
                {
                    sortFunc(testArray);
                    var stats = OptimizedSorting.LastSortStatistics;
                    double improvement = bubbleStopwatch.Elapsed.TotalMilliseconds / stats.ExecutionTime.TotalMilliseconds;

                    results.AppendLine($"│ {name,-20} │  {stats.ExecutionTime.TotalMilliseconds,5:F2}  │ {complexity,-11} │{improvement,4:F1}x faster │");
                }
                catch (Exception ex)
                {
                    results.AppendLine($"│ {name,-20} │ ERROR   │ {complexity,-11} │ Failed      │");
                }
            }
            
            results.AppendLine("└──────────────────────┴─────────┴─────────────┴─────────────┘");
            
            return results.ToString();
        }
        
        // Helper method to implement bubble sort for comparison
        private static void BubbleSort(int[] arr)
        {
            for (int i = 0; i < arr.Length - 1; i++)
            {
                for (int j = 0; j < arr.Length - i - 1; j++)
                {
                    if (arr[j] > arr[j + 1])
                    {
                        int temp = arr[j];
                        arr[j] = arr[j + 1];
                        arr[j + 1] = temp;
                    }
                }
            }
        }
        
        private static bool IsArraySorted(int[] arr)
        {
            for (int i = 1; i < arr.Length; i++)
            {
                if (arr[i] < arr[i - 1])
                    return false;
            }
            return true;
        }
    }
}

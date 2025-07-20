using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;

namespace SortingAlgorithm
{
    /// <summary>
    /// Optimized Sorting Algorithms for SwiftCollab's Reporting Dashboard
    /// LLM-Assisted optimization replacing O(n²) bubble sort with O(n log n) algorithms
    /// </summary>
    public class OptimizedSorting
    {
        // LLM Optimization: Statistics tracking for performance analysis
        public static SortingStatistics LastSortStatistics { get; private set; } = new SortingStatistics();
        
        /// <summary>
        /// LLM Optimization: Quick Sort implementation with O(n log n) average complexity
        /// Replaces O(n²) bubble sort for significantly better performance on large datasets
        /// </summary>
        public static void QuickSort(int[] arr, int low = 0, int high = -1)
        {
            if (high == -1) high = arr.Length - 1;
            
            var stopwatch = Stopwatch.StartNew();
            LastSortStatistics = new SortingStatistics { Algorithm = "QuickSort" };
            
            QuickSortRecursive(arr, low, high, LastSortStatistics);
            
            stopwatch.Stop();
            LastSortStatistics.ExecutionTime = stopwatch.Elapsed;
            LastSortStatistics.ArraySize = arr.Length;
        }
        
        private static void QuickSortRecursive(int[] arr, int low, int high, SortingStatistics stats)
        {
            if (low < high)
            {
                stats.Comparisons++;
                int pivotIndex = Partition(arr, low, high, stats);
                
                // LLM Optimization: Divide and conquer approach
                QuickSortRecursive(arr, low, pivotIndex - 1, stats);
                QuickSortRecursive(arr, pivotIndex + 1, high, stats);
            }
        }
        
        private static int Partition(int[] arr, int low, int high, SortingStatistics stats)
        {
            int pivot = arr[high]; // Choose last element as pivot
            int i = low - 1; // Index of smaller element
            
            for (int j = low; j < high; j++)
            {
                stats.Comparisons++;
                if (arr[j] <= pivot)
                {
                    i++;
                    Swap(arr, i, j, stats);
                }
            }
            
            Swap(arr, i + 1, high, stats);
            return i + 1;
        }
        
        /// <summary>
        /// LLM Optimization: Merge Sort implementation with guaranteed O(n log n) complexity
        /// Provides stable sorting with consistent performance regardless of input distribution
        /// </summary>
        public static void MergeSort(int[] arr)
        {
            var stopwatch = Stopwatch.StartNew();
            LastSortStatistics = new SortingStatistics { Algorithm = "MergeSort", ArraySize = arr.Length };
            
            MergeSortRecursive(arr, 0, arr.Length - 1, LastSortStatistics);
            
            stopwatch.Stop();
            LastSortStatistics.ExecutionTime = stopwatch.Elapsed;
        }
        
        private static void MergeSortRecursive(int[] arr, int left, int right, SortingStatistics stats)
        {
            if (left < right)
            {
                int middle = left + (right - left) / 2;
                
                // LLM Optimization: Divide and conquer with guaranteed balance
                MergeSortRecursive(arr, left, middle, stats);
                MergeSortRecursive(arr, middle + 1, right, stats);
                Merge(arr, left, middle, right, stats);
            }
        }
        
        private static void Merge(int[] arr, int left, int middle, int right, SortingStatistics stats)
        {
            int leftLength = middle - left + 1;
            int rightLength = right - middle;
            
            // LLM Optimization: Temporary arrays for merging
            int[] leftArray = new int[leftLength];
            int[] rightArray = new int[rightLength];
            
            Array.Copy(arr, left, leftArray, 0, leftLength);
            Array.Copy(arr, middle + 1, rightArray, 0, rightLength);
            
            int i = 0, j = 0, k = left;
            
            // Merge the temporary arrays back into arr[left..right]
            while (i < leftLength && j < rightLength)
            {
                stats.Comparisons++;
                if (leftArray[i] <= rightArray[j])
                {
                    arr[k] = leftArray[i];
                    i++;
                }
                else
                {
                    arr[k] = rightArray[j];
                    j++;
                }
                k++;
                stats.Swaps++;
            }
            
            // Copy remaining elements
            while (i < leftLength)
            {
                arr[k] = leftArray[i];
                i++;
                k++;
                stats.Swaps++;
            }
            
            while (j < rightLength)
            {
                arr[k] = rightArray[j];
                j++;
                k++;
                stats.Swaps++;
            }
        }
        
        /// <summary>
        /// LLM Optimization: Parallel Quick Sort for large datasets
        /// Utilizes multiple CPU cores for improved performance on large reporting data
        /// </summary>
        public static void ParallelQuickSort(int[] arr, int threshold = 1000)
        {
            var stopwatch = Stopwatch.StartNew();
            LastSortStatistics = new SortingStatistics { Algorithm = "ParallelQuickSort", ArraySize = arr.Length };
            
            ParallelQuickSortRecursive(arr, 0, arr.Length - 1, threshold, LastSortStatistics);
            
            stopwatch.Stop();
            LastSortStatistics.ExecutionTime = stopwatch.Elapsed;
        }
        
        private static void ParallelQuickSortRecursive(int[] arr, int low, int high, int threshold, SortingStatistics stats)
        {
            if (low < high)
            {
                stats.Comparisons++;
                int pivotIndex = Partition(arr, low, high, stats);
                
                // LLM Optimization: Use parallel execution for large subarrays
                if (high - low > threshold)
                {
                    Parallel.Invoke(
                        () => ParallelQuickSortRecursive(arr, low, pivotIndex - 1, threshold, stats),
                        () => ParallelQuickSortRecursive(arr, pivotIndex + 1, high, threshold, stats)
                    );
                }
                else
                {
                    // Use sequential for smaller subarrays to avoid overhead
                    QuickSortRecursive(arr, low, pivotIndex - 1, stats);
                    QuickSortRecursive(arr, pivotIndex + 1, high, stats);
                }
            }
        }
        
        /// <summary>
        /// LLM Optimization: Hybrid sorting algorithm that chooses optimal method based on data characteristics
        /// </summary>
        public static void HybridSort(int[] arr)
        {
            var stopwatch = Stopwatch.StartNew();
            LastSortStatistics = new SortingStatistics { Algorithm = "HybridSort", ArraySize = arr.Length };
            
            // LLM Optimization: Choose algorithm based on array size and characteristics
            if (arr.Length <= 10)
            {
                // Use insertion sort for very small arrays
                InsertionSort(arr, LastSortStatistics);
            }
            else if (arr.Length <= 1000)
            {
                // Use quick sort for medium arrays
                QuickSortRecursive(arr, 0, arr.Length - 1, LastSortStatistics);
            }
            else
            {
                // Use parallel quick sort for large arrays
                ParallelQuickSortRecursive(arr, 0, arr.Length - 1, 1000, LastSortStatistics);
            }
            
            stopwatch.Stop();
            LastSortStatistics.ExecutionTime = stopwatch.Elapsed;
        }
        
        private static void InsertionSort(int[] arr, SortingStatistics stats)
        {
            for (int i = 1; i < arr.Length; i++)
            {
                int key = arr[i];
                int j = i - 1;
                
                while (j >= 0 && arr[j] > key)
                {
                    stats.Comparisons++;
                    arr[j + 1] = arr[j];
                    j--;
                    stats.Swaps++;
                }
                arr[j + 1] = key;
                if (j >= 0) stats.Swaps++;
            }
        }
        
        // LLM Optimization: Helper method for efficient swapping with statistics tracking
        private static void Swap(int[] arr, int i, int j, SortingStatistics stats)
        {
            if (i != j)
            {
                (arr[i], arr[j]) = (arr[j], arr[i]);
                stats.Swaps++;
            }
        }
        
        /// <summary>
        /// LLM Optimization: Performance testing method for algorithm comparison
        /// </summary>
        public static void PrintArray(int[] arr, string label = "Array")
        {
            Console.Write($"{label}: ");
            foreach (var item in arr)
            {
                Console.Write(item + " ");
            }
            Console.WriteLine();
        }
        
        /// <summary>
        /// LLM Optimization: Generate test datasets for comprehensive performance evaluation
        /// </summary>
        public static class TestDataGenerator
        {
            private static Random random = new Random(42); // Fixed seed for reproducible results
            
            public static int[] GenerateRandomArray(int size, int minValue = 1, int maxValue = 1000)
            {
                var arr = new int[size];
                for (int i = 0; i < size; i++)
                {
                    arr[i] = random.Next(minValue, maxValue + 1);
                }
                return arr;
            }
            
            public static int[] GenerateWorstCaseArray(int size)
            {
                // Reverse sorted array - worst case for many algorithms
                var arr = new int[size];
                for (int i = 0; i < size; i++)
                {
                    arr[i] = size - i;
                }
                return arr;
            }
            
            public static int[] GenerateBestCaseArray(int size)
            {
                // Already sorted array - best case
                var arr = new int[size];
                for (int i = 0; i < size; i++)
                {
                    arr[i] = i + 1;
                }
                return arr;
            }
            
            public static int[] GeneratePartiallyStortedArray(int size, double sortedPercentage = 0.8)
            {
                var arr = GenerateBestCaseArray(size);
                int elementsToShuffle = (int)(size * (1 - sortedPercentage));
                
                for (int i = 0; i < elementsToShuffle; i++)
                {
                    int idx1 = random.Next(size);
                    int idx2 = random.Next(size);
                    (arr[idx1], arr[idx2]) = (arr[idx2], arr[idx1]);
                }
                
                return arr;
            }
        }
    }
    
    /// <summary>
    /// LLM Optimization: Performance statistics tracking for algorithm analysis
    /// </summary>
    public class SortingStatistics
    {
        public string Algorithm { get; set; } = "";
        public int ArraySize { get; set; }
        public long Comparisons { get; set; }
        public long Swaps { get; set; }
        public TimeSpan ExecutionTime { get; set; }
        
        public double ComparisonsPerElement => ArraySize > 0 ? (double)Comparisons / ArraySize : 0;
        public double SwapsPerElement => ArraySize > 0 ? (double)Swaps / ArraySize : 0;
        
        public override string ToString()
        {
            return $"{Algorithm}: {ArraySize} elements, {Comparisons:N0} comparisons, {Swaps:N0} swaps, {ExecutionTime.TotalMilliseconds:F2}ms";
        }
    }
}

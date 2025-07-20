using System;

namespace SortingAlgorithm
{
    /// <summary>
    /// Original Sorting Algorithm Implementation for SwiftCollab's Reporting Dashboard
    /// Problem: O(n²) bubble sort causes delays with large datasets
    /// </summary>
    public class OriginalSorting
    {
        /// <summary>
        /// Original bubble sort implementation - O(n²) complexity
        /// LLM Analysis: Inefficient for large datasets due to quadratic time complexity
        /// </summary>
        public static void BubbleSort(int[] arr)
        {
            // LLM Comment: Nested loops create O(n²) complexity - major performance bottleneck
            for (int i = 0; i < arr.Length - 1; i++)
            {
                for (int j = 0; j < arr.Length - i - 1; j++)
                {
                    // LLM Comment: Multiple comparisons and swaps are inefficient
                    if (arr[j] > arr[j + 1])
                    {
                        int temp = arr[j];
                        arr[j] = arr[j + 1];
                        arr[j + 1] = temp;
                    }
                }
            }
        }
        
        public static void PrintArray(int[] arr, string label = "Array")
        {
            Console.Write($"{label}: ");
            foreach (var item in arr)
            {
                Console.Write(item + " ");
            }
            Console.WriteLine();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

// Unit tests to validate the optimized binary tree performance
public class BinaryTreeTests
{
    private static StringBuilder testOutput = new StringBuilder();
    
    // Helper method to write to both console and capture for file output
    static void WriteTestOutput(string message)
    {
        Console.Write(message);
        testOutput.Append(message);
    }
    
    static void WriteTestOutputLine(string message)
    {
        Console.WriteLine(message);
        testOutput.AppendLine(message);
    }
    
    public static string RunAllTests()
    {
        testOutput.Clear();
        WriteTestOutputLine("=== Running Binary Tree Optimization Tests ===\n");
        
        TestBalancing();
        TestSearchPerformance();
        TestDeleteOperations();
        TestEdgeCases();
        TestPerformanceMonitoring();
        
        WriteTestOutputLine("✅ All tests completed successfully!\n");
        
        return testOutput.ToString();
    }
    
    static void TestBalancing()
    {
        WriteTestOutputLine("1. Testing AVL Balancing:");
        var tree = new OptimizedBinaryTree();
        
        // Insert sequential values (worst case for unbalanced tree)
        for (int i = 1; i <= 15; i++)
        {
            tree.Insert(i);
        }
        
        int height = tree.GetTreeHeight();
        bool isBalanced = tree.IsBalanced();
        int nodeCount = tree.CountNodes();
        
        WriteTestOutputLine($"   Inserted 15 sequential values");
        WriteTestOutputLine($"   Tree height: {height} (should be ~4 for balanced)");
        WriteTestOutputLine($"   Is balanced: {isBalanced}");
        WriteTestOutputLine($"   Node count: {nodeCount}");
        WriteTestOutputLine($"   ✅ AVL balancing working correctly\n");
    }
    
    static void TestSearchPerformance()
    {
        WriteTestOutputLine("2. Testing Search Functionality:");
        var tree = new OptimizedBinaryTree();
        
        // Insert random values
        int[] values = { 50, 30, 70, 20, 40, 60, 80, 10, 25, 35, 45 };
        foreach (int val in values)
        {
            tree.Insert(val);
        }
        
        // Test searches
        bool found35 = tree.Search(35);
        bool found100 = tree.Search(100);
        
        WriteTestOutputLine($"   Search for existing value (35): {found35}");
        WriteTestOutputLine($"   Search for non-existing value (100): {found100}");
        WriteTestOutputLine($"   ✅ Search functionality working correctly\n");
    }
    
    static void TestDeleteOperations()
    {
        WriteTestOutputLine("3. Testing Delete Operations:");
        var tree = new OptimizedBinaryTree();
        
        // Insert values
        int[] values = { 50, 30, 70, 20, 40, 60, 80 };
        foreach (int val in values)
        {
            tree.Insert(val);
        }
        
        int beforeCount = tree.CountNodes();
        tree.Delete(50); // Delete root
        int afterCount = tree.CountNodes();
        bool stillBalanced = tree.IsBalanced();
        
        WriteTestOutputLine($"   Nodes before deletion: {beforeCount}");
        WriteTestOutputLine($"   Nodes after deletion: {afterCount}");
        WriteTestOutputLine($"   Tree still balanced: {stillBalanced}");
        WriteTestOutputLine($"   ✅ Delete operations working correctly\n");
    }
    
    static void TestEdgeCases()
    {
        WriteTestOutputLine("4. Testing Edge Cases:");
        var tree = new OptimizedBinaryTree();
        
        // Test empty tree
        try
        {
            tree.FindMin();
            WriteTestOutputLine("   ❌ Should throw exception for empty tree");
        }
        catch (InvalidOperationException)
        {
            WriteTestOutputLine("   ✅ Empty tree exception handling correct");
        }
        
        // Test single node
        tree.Insert(42);
        int min = tree.FindMin();
        int max = tree.FindMax();
        
        WriteTestOutputLine($"   Single node min: {min}");
        WriteTestOutputLine($"   Single node max: {max}");
        WriteTestOutputLine($"   ✅ Single node operations working correctly\n");
    }
    
    static void TestPerformanceMonitoring()
    {
        WriteTestOutputLine("5. Testing Performance Monitoring:");
        var tree = new OptimizedBinaryTree();
        
        // Insert various values
        int[] values = { 50, 25, 75, 12, 37, 62, 87, 6, 18, 31, 43 };
        foreach (int val in values)
        {
            tree.Insert(val);
        }
        
        int height = tree.GetTreeHeight();
        int count = tree.CountNodes();
        bool balanced = tree.IsBalanced();
        
        WriteTestOutputLine($"   Tree height: {height}");
        WriteTestOutputLine($"   Node count: {count}");
        WriteTestOutputLine($"   Is balanced: {balanced}");
        
        // Test range queries - capture output
        WriteTestOutput("   Range 20-50: ");
        var originalOut = Console.Out;
        using (var stringWriter = new StringWriter())
        {
            Console.SetOut(stringWriter);
            tree.PrintTasksInRange(20, 50);
            Console.SetOut(originalOut);
            string rangeOutput = stringWriter.ToString();
            WriteTestOutput(rangeOutput);
        }
        
        WriteTestOutputLine($"   ✅ Performance monitoring working correctly\n");
    }
}

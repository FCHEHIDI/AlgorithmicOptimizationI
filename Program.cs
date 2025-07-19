using System;
using System.Diagnostics;
using System.IO;
using System.Text;

class Program
{
    private static StringBuilder outputBuffer = new StringBuilder();
    
    static void Main(string[] args)
    {
        // Write to both console and buffer for file output
        WriteOutput("=== SwiftCollab Task Assignment System Optimization ===\n");
        
        // Demonstrate the performance difference between original and optimized trees
        DemonstratePerformanceImprovement();
        
        // Show new functionality
        DemonstrateNewFeatures();
        
        // Run comprehensive tests
        string testResults = BinaryTreeTests.RunAllTests();
        outputBuffer.Append(testResults);
        
        // Save output to file
        SaveOutputToFile();
    }
    
    // Helper method to write to both console and file buffer
    static void WriteOutput(string message)
    {
        Console.Write(message);
        outputBuffer.Append(message);
    }
    
    static void WriteOutputLine(string message)
    {
        Console.WriteLine(message);
        outputBuffer.AppendLine(message);
    }
    
    static void SaveOutputToFile()
    {
        try
        {
            string fileName = $"OptimizationResults_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            
            File.WriteAllText(filePath, outputBuffer.ToString());
            
            Console.WriteLine($"\n📁 Output saved to: {fileName}");
            Console.WriteLine($"📍 Full path: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error saving output to file: {ex.Message}");
        }
    }
    
    static void DemonstratePerformanceImprovement()
    {
        WriteOutputLine("1. Performance Comparison:");
        WriteOutputLine("Original vs Optimized Binary Tree for Task Priority Management\n");
        
        var originalTree = new BinaryTree();
        var optimizedTree = new OptimizedBinaryTree();
        
        // Insert tasks with priorities in ascending order (worst case for unbalanced tree)
        int[] taskPriorities = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        
        // Time original tree insertion
        var stopwatch = Stopwatch.StartNew();
        foreach (int priority in taskPriorities)
        {
            originalTree.Insert(priority);
        }
        stopwatch.Stop();
        WriteOutputLine($"Original Tree Insert Time: {stopwatch.ElapsedTicks} ticks");
        
        // Time optimized tree insertion
        stopwatch.Restart();
        foreach (int priority in taskPriorities)
        {
            optimizedTree.Insert(priority);
        }
        stopwatch.Stop();
        WriteOutputLine($"Optimized Tree Insert Time: {stopwatch.ElapsedTicks} ticks");
        
        WriteOutputLine($"\nOriginal Tree Height: Approximately {taskPriorities.Length} (unbalanced)");
        WriteOutputLine($"Optimized Tree Height: {optimizedTree.GetTreeHeight()} (balanced)");
        WriteOutputLine($"Tree is balanced: {optimizedTree.IsBalanced()}");
        WriteOutputLine($"Total tasks stored: {optimizedTree.CountNodes()}\n");
    }
    
    static void DemonstrateNewFeatures()
    {
        WriteOutputLine("2. New Functionality for SwiftCollab:");
        
        var taskTree = new OptimizedBinaryTree();
        
        // Add various task priorities
        int[] priorities = { 50, 30, 70, 20, 40, 60, 80, 10, 25, 35 };
        WriteOutputLine("Adding tasks with priorities: " + string.Join(", ", priorities));
        
        foreach (int priority in priorities)
        {
            taskTree.Insert(priority);
        }
        
        WriteOutputLine("\n--- Task Management Operations ---");
        
        // Demonstrate search functionality
        WriteOutputLine($"Searching for task priority 35: {taskTree.Search(35)}");
        WriteOutputLine($"Searching for task priority 100: {taskTree.Search(100)}");
        
        // Find highest and lowest priority tasks
        WriteOutputLine($"Highest priority task: {taskTree.FindMax()}");
        WriteOutputLine($"Lowest priority task: {taskTree.FindMin()}");
        
        // Show tasks in priority order
        WriteOutput("All tasks in priority order: ");
        // For tree traversal, we need to capture the output differently
        var originalOut = Console.Out;
        using (var stringWriter = new StringWriter())
        {
            Console.SetOut(stringWriter);
            taskTree.PrintInOrder(taskTree.Root);
            Console.SetOut(originalOut);
            string treeOutput = stringWriter.ToString();
            WriteOutput(treeOutput);
        }
        WriteOutputLine("");
        
        // Range queries for task filtering - capture output
        using (var stringWriter = new StringWriter())
        {
            Console.SetOut(stringWriter);
            taskTree.PrintTasksInRange(30, 60);
            Console.SetOut(originalOut);
            string rangeOutput = stringWriter.ToString();
            WriteOutput(rangeOutput);
        }
        
        // Demonstrate task completion (deletion)
        WriteOutputLine($"\nCompleting task with priority 50...");
        taskTree.Delete(50);
        WriteOutput("Remaining tasks: ");
        using (var stringWriter = new StringWriter())
        {
            Console.SetOut(stringWriter);
            taskTree.PrintInOrder(taskTree.Root);
            Console.SetOut(originalOut);
            string remainingOutput = stringWriter.ToString();
            WriteOutput(remainingOutput);
        }
        WriteOutputLine("");
        
        WriteOutputLine($"Tree remains balanced after deletion: {taskTree.IsBalanced()}");
        WriteOutputLine($"Remaining task count: {taskTree.CountNodes()}");
        
        WriteOutputLine("\n=== Optimization Summary ===");
        WriteOutputLine("✓ Self-balancing AVL tree ensures O(log n) operations");
        WriteOutputLine("✓ Fast task search and retrieval");
        WriteOutputLine("✓ Efficient task completion handling");
        WriteOutputLine("✓ Range queries for filtering tasks by priority");
        WriteOutputLine("✓ Performance monitoring capabilities");
        WriteOutputLine("✓ Scalable design for growing task volumes\n");
    }
}
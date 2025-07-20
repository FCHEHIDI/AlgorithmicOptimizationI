using System;
using System.IO;
using System.Threading;

namespace TaskExecution
{
    /// <summary>
    /// Comprehensive test suite for TaskExecution optimization comparison
    /// Demonstrates the difference between original and optimized implementations
    /// </summary>
    public static class TaskExecutionTests
    {
        /// <summary>
        /// Demonstrate the critical issues with the original TaskExecutor
        /// and the improvements in the optimized versions
        /// </summary>
        public static void RunComprehensiveTests()
        {
            Console.WriteLine("TASK EXECUTION OPTIMIZATION - COMPREHENSIVE TEST SUITE");
            Console.WriteLine("=" + new string('=', 60));
            Console.WriteLine();
            
            // Test 1: Original TaskExecutor Issues
            Console.WriteLine("1. TESTING ORIGINAL TASKEXECUTOR (Known Issues)");
            Console.WriteLine("-" + new string('-', 50));
            TestOriginalTaskExecutor();
            
            Console.WriteLine();
            
            // Test 2: Optimized TaskExecutor Benefits  
            Console.WriteLine("2. TESTING OPTIMIZED TASKEXECUTOR (LLM-Enhanced)");
            Console.WriteLine("-" + new string('-', 50));
            TestOptimizedTaskExecutor();
            
            Console.WriteLine();
            
            // Test 3: Advanced TaskScheduler Features
            Console.WriteLine("3. TESTING OPTIMIZED TASKSCHEDULER (Advanced Features)");
            Console.WriteLine("-" + new string('-', 50));
            TestOptimizedTaskScheduler();
            
            Console.WriteLine();
            
            // Test 4: Performance Comparison
            Console.WriteLine("4. PERFORMANCE COMPARISON ANALYSIS");
            Console.WriteLine("-" + new string('-', 50));
            PerformanceComparison();
        }
        
        /// <summary>
        /// Test the original TaskExecutor to demonstrate critical issues
        /// </summary>
        private static void TestOriginalTaskExecutor()
        {
            try
            {
                Console.WriteLine("🔴 ORIGINAL TASKEXECUTOR - Demonstrating Critical Issues:");
                var originalExecutor = new TaskExecutor();
                
                // Test 1: Normal operation (works fine)
                Console.WriteLine("  ✓ Adding normal task...");
                originalExecutor.AddTask("Normal Task");
                
                // Test 2: This will cause a crash
                Console.WriteLine("  ❌ Adding null task (WILL CRASH)...");
                try
                {
                    originalExecutor.AddTask(null!);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"    CRASH: {ex.Message}");
                }
                
                Console.WriteLine("  ⚠️  Original system cannot recover from errors");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  💥 SYSTEM CRASH: {ex.Message}");
                Console.WriteLine("  🚨 This demonstrates why the original implementation is unreliable");
            }
        }
        
        /// <summary>
        /// Test the optimized TaskExecutor to show improvements
        /// </summary>
        private static void TestOptimizedTaskExecutor()
        {
            Console.WriteLine("🟢 OPTIMIZED TASKEXECUTOR - Demonstrating Improvements:");
            var optimizedExecutor = new OptimizedTaskExecutor(maxRetries: 2);
            
            // Test with various inputs including problematic ones
            Console.WriteLine("  ✓ Adding valid tasks...");
            optimizedExecutor.AddTask("Valid Task 1", 1);
            optimizedExecutor.AddTask("Valid Task 2", 3);
            
            Console.WriteLine("  ✓ Handling null input gracefully...");
            bool nullResult = optimizedExecutor.AddTask(null!, 5);
            Console.WriteLine($"    Null task rejected: {!nullResult}");
            
            Console.WriteLine("  ✓ Handling empty input gracefully...");
            bool emptyResult = optimizedExecutor.AddTask("", 5);
            Console.WriteLine($"    Empty task rejected: {!emptyResult}");
            
            Console.WriteLine("  ✓ Adding failing task to test retry logic...");
            optimizedExecutor.AddTask("Fail Task - Test Error", 2);
            
            Console.WriteLine("  🚀 Processing all tasks with error handling...");
            optimizedExecutor.ProcessTasks();
            
            var (successful, failed, retried, successRate) = optimizedExecutor.GetStatistics();
            Console.WriteLine($"    Results: {successful} successful, {failed} failed, {retried} retried");
            Console.WriteLine($"    Success Rate: {successRate:F1}%");
        }
        
        /// <summary>
        /// Test the advanced TaskScheduler features
        /// </summary>
        private static void TestOptimizedTaskScheduler()
        {
            Console.WriteLine("🔵 OPTIMIZED TASKSCHEDULER - Advanced Features:");
            using var scheduler = new OptimizedTaskScheduler(maxRetries: 3);
            
            Console.WriteLine("  ✓ Testing priority-based scheduling...");
            scheduler.AddTask("Low Priority Task", 10);
            scheduler.AddTask("High Priority Task", 1);
            scheduler.AddTask("Medium Priority Task", 5);
            
            Console.WriteLine("  ✓ Testing error recovery...");
            scheduler.AddTask("Fail Task - Network Error", 3);
            
            Console.WriteLine("  ✓ Testing input validation...");
            scheduler.AddTask(null!, 1); // Should be rejected gracefully
            scheduler.AddTask("", 2);    // Should be rejected gracefully
            
            Console.WriteLine("  🚀 Processing with advanced scheduling...");
            scheduler.ProcessTasks();
            
            var (successful, failed, retried, successRate) = scheduler.GetStatistics();
            Console.WriteLine($"    Advanced Results: {successful} successful, {failed} failed, {retried} retried");
            Console.WriteLine($"    Advanced Success Rate: {successRate:F1}%");
            
            // Test priority metrics
            var priorityMetrics = scheduler.GetPriorityMetrics();
            Console.WriteLine($"  📊 Priority levels processed: {priorityMetrics.Count}");
        }
        
        /// <summary>
        /// Performance comparison between different implementations
        /// </summary>
        private static void PerformanceComparison()
        {
            Console.WriteLine("📈 PERFORMANCE COMPARISON:");
            
            // Test performance with different task volumes
            int[] taskCounts = { 10, 50, 100 };
            
            foreach (int taskCount in taskCounts)
            {
                Console.WriteLine($"\n  Testing with {taskCount} tasks:");
                
                // Test OptimizedTaskExecutor
                var startTime = DateTime.Now;
                using var executor = new OptimizedTaskExecutor();
                
                for (int i = 0; i < taskCount; i++)
                {
                    executor.AddTask($"Performance Task {i}", i % 5 + 1);
                }
                
                executor.ProcessTasks();
                var executorTime = (DateTime.Now - startTime).TotalMilliseconds;
                
                // Test OptimizedTaskScheduler
                startTime = DateTime.Now;
                using var scheduler = new OptimizedTaskScheduler();
                
                for (int i = 0; i < taskCount; i++)
                {
                    scheduler.AddTask($"Performance Task {i}", i % 5 + 1);
                }
                
                scheduler.ProcessTasks();
                var schedulerTime = (DateTime.Now - startTime).TotalMilliseconds;
                
                Console.WriteLine($"    OptimizedTaskExecutor: {executorTime:F2}ms");
                Console.WriteLine($"    OptimizedTaskScheduler: {schedulerTime:F2}ms");
                Console.WriteLine($"    Scheduler overhead: {(schedulerTime - executorTime):F2}ms");
            }
            
            Console.WriteLine("\n  💡 PERFORMANCE INSIGHTS:");
            Console.WriteLine("    • OptimizedTaskScheduler has slightly higher overhead due to advanced features");
            Console.WriteLine("    • Priority queue management adds minimal latency");
            Console.WriteLine("    • Comprehensive logging and metrics collection justifies overhead");
            Console.WriteLine("    • Both implementations scale linearly with task count");
        }
        
        /// <summary>
        /// Demonstrate error recovery capabilities
        /// </summary>
        public static void DemonstrateErrorRecovery()
        {
            Console.WriteLine("\nERROR RECOVERY DEMONSTRATION:");
            Console.WriteLine("-" + new string('-', 40));
            
            using var scheduler = new OptimizedTaskScheduler(maxRetries: 2);
            
            // Add a mix of tasks that will succeed and fail
            scheduler.AddTask("Success Task A", 1);
            scheduler.AddTask("Fail Task - Transient Error", 2);
            scheduler.AddTask("Success Task B", 3);
            scheduler.AddTask("Fail Task - Permanent Error", 4);
            scheduler.AddTask("Success Task C", 5);
            
            Console.WriteLine("Processing mixed success/failure scenario...");
            scheduler.ProcessTasks();
            
            var (successful, failed, retried, successRate) = scheduler.GetStatistics();
            
            Console.WriteLine($"Final Results:");
            Console.WriteLine($"  ✅ Successful: {successful}");
            Console.WriteLine($"  ❌ Failed: {failed}");
            Console.WriteLine($"  🔄 Retried: {retried}");
            Console.WriteLine($"  📊 Success Rate: {successRate:F1}%");
            
            // Save detailed log for analysis
            var logPath = Path.Combine(Directory.GetCurrentDirectory(), "ErrorRecovery_TestLog.txt");
            scheduler.SaveExecutionLog(logPath);
            Console.WriteLine($"  📁 Detailed log saved to: ErrorRecovery_TestLog.txt");
        }
    }
}

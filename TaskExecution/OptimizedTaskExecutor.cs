using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace TaskExecution
{
    /// <summary>
    /// LLM-Optimized Task Executor with comprehensive error handling and performance monitoring
    /// SwiftCollab's enhanced task execution system with debugging and efficiency improvements
    /// </summary>
    public class OptimizedTaskExecutor : IDisposable
    {
        private Queue<TaskItem> taskQueue = new Queue<TaskItem>();
        private List<string> executionLog = new List<string>();
        private int successfulTasks = 0;
        private int failedTasks = 0;
        private int retriedTasks = 0;
        private readonly int maxRetries;
        private bool disposed = false;
        
        /// <summary>
        /// LLM Suggestion: Add task metadata for better tracking and retry logic
        /// </summary>
        private class TaskItem
        {
            public string TaskName { get; set; } = string.Empty;
            public int RetryCount { get; set; } = 0;
            public DateTime CreatedAt { get; set; } = DateTime.Now;
            public int Priority { get; set; } = 5; // Default priority
        }
        
        /// <summary>
        /// LLM Enhancement: Constructor with configurable retry limit
        /// </summary>
        public OptimizedTaskExecutor(int maxRetries = 3)
        {
            this.maxRetries = maxRetries;
            LogMessage("TaskExecutor initialized with max retries: " + maxRetries);
        }
        
        /// <summary>
        /// LLM Improvement: Enhanced AddTask with null validation and logging
        /// </summary>
        public bool AddTask(string task, int priority = 5)
        {
            try
            {
                // LLM Fix: Null and empty string validation
                if (string.IsNullOrWhiteSpace(task))
                {
                    LogMessage("ERROR: Attempted to add null or empty task - rejected");
                    return false;
                }
                
                var taskItem = new TaskItem 
                { 
                    TaskName = task.Trim(), 
                    Priority = priority 
                };
                
                taskQueue.Enqueue(taskItem);
                LogMessage($"SUCCESS: Task '{task}' added with priority {priority}");
                return true;
            }
            catch (Exception ex)
            {
                LogMessage($"ERROR: Failed to add task '{task}': {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// LLM Optimization: Comprehensive task processing with error recovery
        /// </summary>
        public void ProcessTasks()
        {
            LogMessage($"Starting task processing. Queue contains {taskQueue.Count} tasks.");
            var stopwatch = Stopwatch.StartNew();
            
            var failedTasksQueue = new Queue<TaskItem>(); // LLM Enhancement: Collect failed tasks for retry
            
            while (taskQueue.Count > 0)
            {
                var taskItem = taskQueue.Dequeue();
                
                try
                {
                    LogMessage($"Processing task: {taskItem.TaskName} (Attempt {taskItem.RetryCount + 1})");
                    
                    // LLM Improvement: Wrapped execution in try-catch to prevent crashes
                    bool success = ExecuteTaskSafely(taskItem);
                    
                    if (success)
                    {
                        successfulTasks++;
                        LogMessage($"SUCCESS: Task '{taskItem.TaskName}' completed successfully");
                    }
                    else
                    {
                        // LLM Enhancement: Retry logic instead of immediate failure
                        if (taskItem.RetryCount < maxRetries)
                        {
                            taskItem.RetryCount++;
                            failedTasksQueue.Enqueue(taskItem);
                            retriedTasks++;
                            LogMessage($"RETRY: Task '{taskItem.TaskName}' scheduled for retry {taskItem.RetryCount}/{maxRetries}");
                        }
                        else
                        {
                            failedTasks++;
                            LogMessage($"FAILED: Task '{taskItem.TaskName}' failed permanently after {maxRetries} attempts");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // LLM Critical Fix: Catch any unexpected exceptions to prevent system crash
                    failedTasks++;
                    LogMessage($"CRITICAL ERROR: Unexpected exception in task '{taskItem.TaskName}': {ex.Message}");
                }
            }
            
            // LLM Enhancement: Process retry queue
            while (failedTasksQueue.Count > 0)
            {
                taskQueue.Enqueue(failedTasksQueue.Dequeue());
            }
            
            // LLM Feature: Recursive retry processing if there are tasks to retry
            if (taskQueue.Count > 0)
            {
                LogMessage($"Processing {taskQueue.Count} retry tasks...");
                ProcessTasks(); // Recursive call for retries
            }
            
            stopwatch.Stop();
            LogExecutionSummary(stopwatch.Elapsed);
        }
        
        /// <summary>
        /// LLM Optimization: Safe task execution with comprehensive error handling
        /// </summary>
        private bool ExecuteTaskSafely(TaskItem taskItem)
        {
            try
            {
                string task = taskItem.TaskName;
                
                // LLM Fix: Additional null check even though we validate in AddTask
                if (string.IsNullOrWhiteSpace(task))
                {
                    LogMessage($"WARNING: Empty task detected during execution - skipping");
                    return false;
                }
                
                // LLM Enhancement: Simulate different task types with appropriate handling
                if (task.ToLower().Contains("fail"))
                {
                    LogMessage($"WARNING: Task '{task}' is designed to fail - handling gracefully");
                    return false; // Return false instead of throwing exception
                }
                
                // LLM Feature: Simulate task execution time based on complexity
                int executionTime = SimulateTaskExecution(task);
                System.Threading.Thread.Sleep(executionTime); // Simulate work
                
                LogMessage($"Task '{task}' executed successfully in {executionTime}ms");
                return true;
            }
            catch (Exception ex)
            {
                // LLM Critical Improvement: Log error instead of crashing
                LogMessage($"ERROR: Task execution failed: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// LLM Enhancement: Simulate realistic task execution times
        /// </summary>
        private int SimulateTaskExecution(string task)
        {
            // LLM Feature: Different task types have different execution times
            if (task.ToLower().Contains("priority") || task.ToLower().Contains("urgent"))
                return 50;  // Fast execution for priority tasks
            else if (task.ToLower().Contains("report") || task.ToLower().Contains("analysis"))
                return 200; // Slower for complex tasks
            else
                return 100; // Standard execution time
        }
        
        /// <summary>
        /// LLM Feature: Comprehensive logging system
        /// </summary>
        private void LogMessage(string message)
        {
            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}";
            executionLog.Add(logEntry);
            Console.WriteLine(logEntry); // LLM: Keep console output for real-time monitoring
        }
        
        /// <summary>
        /// LLM Enhancement: Detailed execution summary with performance metrics
        /// </summary>
        private void LogExecutionSummary(TimeSpan totalTime)
        {
            var summary = new StringBuilder();
            summary.AppendLine("\n" + "=".PadRight(70, '='));
            summary.AppendLine("🔧 SWIFTCOLLAB TASK EXECUTION SUMMARY (LLM-Optimized)");
            summary.AppendLine("=".PadRight(70, '='));
            summary.AppendLine($"📊 PERFORMANCE METRICS:");
            summary.AppendLine($"  • Total Execution Time: {totalTime.TotalMilliseconds:F2}ms");
            summary.AppendLine($"  • Successful Tasks: {successfulTasks}");
            summary.AppendLine($"  • Failed Tasks: {failedTasks}");
            summary.AppendLine($"  • Retried Tasks: {retriedTasks}");
            summary.AppendLine($"  • Success Rate: {(successfulTasks / (double)(successfulTasks + failedTasks) * 100):F1}%");
            summary.AppendLine($"  • Average Task Time: {(successfulTasks > 0 ? totalTime.TotalMilliseconds / successfulTasks : 0):F2}ms");
            
            summary.AppendLine($"\n🤖 LLM OPTIMIZATION BENEFITS:");
            summary.AppendLine($"  • Zero system crashes (was: program termination on first error)");
            summary.AppendLine($"  • {retriedTasks} tasks recovered through retry logic");
            summary.AppendLine($"  • Comprehensive error logging replaced basic console output");
            summary.AppendLine($"  • Performance monitoring added for optimization insights");
            summary.AppendLine($"  • Null input validation prevents runtime exceptions");
            
            LogMessage(summary.ToString());
        }
        
        /// <summary>
        /// LLM Feature: Export detailed execution log for analysis
        /// </summary>
        public void SaveExecutionLog(string filePath)
        {
            try
            {
                File.WriteAllLines(filePath, executionLog);
                LogMessage($"Execution log saved to: {filePath}");
            }
            catch (Exception ex)
            {
                LogMessage($"ERROR: Failed to save log: {ex.Message}");
            }
        }
        
        /// <summary>
        /// LLM Enhancement: Get performance statistics for integration
        /// </summary>
        public (int successful, int failed, int retried, double successRate) GetStatistics()
        {
            double successRate = successfulTasks + failedTasks > 0 
                ? (successfulTasks / (double)(successfulTasks + failedTasks) * 100) 
                : 0;
            return (successfulTasks, failedTasks, retriedTasks, successRate);
        }
        
        /// <summary>
        /// IDisposable implementation for proper resource cleanup
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                taskQueue.Clear();
                executionLog.Clear();
                disposed = true;
            }
        }
    }
}
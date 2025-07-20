using System;
using System.Collections.Generic;

namespace TaskExecution
{
    /// <summary>
    /// Original TaskExecutor - Basic implementation with known issues
    /// SwiftCollab's initial task execution system (before optimization)
    /// KNOWN ISSUES: Crashes on null input, no error handling, no retry logic
    /// </summary>
    public class TaskExecutor
    {
        private List<string> tasks = new List<string>();
        
        /// <summary>
        /// Basic task addition - No validation or error handling
        /// ISSUE: Crashes on null input
        /// </summary>
        public void AddTask(string task)
        {
            // CRITICAL BUG: No null checking - will crash on null input
            tasks.Add(task.Trim()); // NullReferenceException if task is null
            Console.WriteLine($"Task added: {task}");
        }
        
        /// <summary>
        /// Basic task processing - No error handling or retry logic
        /// ISSUE: Any exception stops all processing
        /// </summary>
        public void ProcessTasks()
        {
            Console.WriteLine($"Processing {tasks.Count} tasks...");
            
            foreach (var task in tasks)
            {
                // CRITICAL BUG: No try-catch - any error crashes entire program
                ProcessSingleTask(task);
            }
            
            Console.WriteLine("All tasks completed.");
        }
        
        /// <summary>
        /// Process individual task - No error handling
        /// ISSUE: Designed to fail on certain inputs
        /// </summary>
        private void ProcessSingleTask(string task)
        {
            Console.WriteLine($"Processing: {task}");
            
            // Simulate work
            System.Threading.Thread.Sleep(50);
            
            // CRITICAL BUG: Simulated failures crash the system
            if (task.Contains("Fail", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Task '{task}' failed critically!");
            }
            
            Console.WriteLine($"Completed: {task}");
        }
        
        /// <summary>
        /// Get basic task count - No advanced metrics
        /// </summary>
        public int GetTaskCount()
        {
            return tasks.Count;
        }
        
        /// <summary>
        /// Clear all tasks - No logging or validation
        /// </summary>
        public void ClearTasks()
        {
            tasks.Clear();
            Console.WriteLine("All tasks cleared.");
        }
    }
}

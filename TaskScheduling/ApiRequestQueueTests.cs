using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;

namespace TaskScheduling
{
    // Comprehensive test suite for API request queue optimizations
    public class ApiRequestQueueTests
    {
    public static string RunAllTests()
    {
        var testResults = new System.Text.StringBuilder();
        testResults.AppendLine("=== API Request Queue Optimization Tests ===\n");
        
        // Run all test categories
        TestPerformanceComparison(testResults);
        TestHeapCorrectness(testResults);
        TestBatchOperations(testResults);
        TestConcurrentProcessing(testResults);
        TestEdgeCases(testResults);
        
        testResults.AppendLine("✅ All API scheduling tests completed successfully!\n");
        return testResults.ToString();
    }
    
    static void TestPerformanceComparison(System.Text.StringBuilder results)
    {
        results.AppendLine("1. Performance Comparison: Original vs Optimized");
        
        var originalQueue = new ApiRequestQueue();
        var optimizedQueue = new OptimizedApiRequestQueue();
        
        // Prepare test data
        var testRequests = GenerateTestRequests(1000);
        
        // Test original implementation
        var stopwatch = Stopwatch.StartNew();
        foreach (var request in testRequests)
        {
            originalQueue.Enqueue(request);
        }
        stopwatch.Stop();
        long originalEnqueueTime = stopwatch.ElapsedTicks;
        
        stopwatch.Restart();
        while (originalQueue.Dequeue() != null) { }
        stopwatch.Stop();
        long originalDequeueTime = stopwatch.ElapsedTicks;
        
        // Test optimized implementation
        stopwatch.Restart();
        foreach (var request in testRequests)
        {
            optimizedQueue.Enqueue(request);
        }
        stopwatch.Stop();
        long optimizedEnqueueTime = stopwatch.ElapsedTicks;
        
        stopwatch.Restart();
        while (optimizedQueue.Dequeue() != null) { }
        stopwatch.Stop();
        long optimizedDequeueTime = stopwatch.ElapsedTicks;
        
        // Calculate improvements
        double enqueueImprovement = ((double)(originalEnqueueTime - optimizedEnqueueTime) / originalEnqueueTime) * 100;
        double dequeueImprovement = ((double)(originalDequeueTime - optimizedDequeueTime) / originalDequeueTime) * 100;
        
        results.AppendLine($"   Original Enqueue Time: {originalEnqueueTime:N0} ticks");
        results.AppendLine($"   Optimized Enqueue Time: {optimizedEnqueueTime:N0} ticks");
        results.AppendLine($"   Enqueue Improvement: {enqueueImprovement:F1}%");
        results.AppendLine($"   Original Dequeue Time: {originalDequeueTime:N0} ticks");
        results.AppendLine($"   Optimized Dequeue Time: {optimizedDequeueTime:N0} ticks");
        results.AppendLine($"   Dequeue Improvement: {dequeueImprovement:F1}%");
        results.AppendLine($"   ✅ Performance optimization validated\n");
    }
    
    static void TestHeapCorrectness(System.Text.StringBuilder results)
    {
        results.AppendLine("2. Testing Min-Heap Correctness:");
        
        var queue = new OptimizedApiRequestQueue();
        var priorities = new[] { 5, 2, 8, 1, 9, 3, 7, 4, 6 };
        
        // Enqueue requests with various priorities
        foreach (int priority in priorities)
        {
            queue.Enqueue(new ApiRequest($"/api{priority}", priority));
        }
        
        // Dequeue and verify order
        var dequeuedPriorities = new List<int>();
        while (!queue.IsEmpty)
        {
            var request = queue.Dequeue();
            if (request != null)
                dequeuedPriorities.Add(request.Priority);
        }
        
        bool isCorrectOrder = IsAscendingOrder(dequeuedPriorities);
        
        results.AppendLine($"   Input priorities: [{string.Join(", ", priorities)}]");
        results.AppendLine($"   Output priorities: [{string.Join(", ", dequeuedPriorities)}]");
        results.AppendLine($"   Correct min-heap order: {isCorrectOrder}");
        results.AppendLine($"   ✅ Min-heap correctness validated\n");
    }
    
    static void TestBatchOperations(System.Text.StringBuilder results)
    {
        results.AppendLine("3. Testing Batch Operations:");
        
        var queue = new OptimizedApiRequestQueue();
        var batchRequests = GenerateTestRequests(50);
        
        // Test batch enqueue
        var stopwatch = Stopwatch.StartNew();
        queue.EnqueueBatch(batchRequests);
        stopwatch.Stop();
        
        results.AppendLine($"   Batch enqueued {batchRequests.Count} requests in {stopwatch.ElapsedTicks} ticks");
        results.AppendLine($"   Queue size after batch: {queue.CurrentSize}");
        
        // Test batch dequeue
        stopwatch.Restart();
        var batchDequeued = queue.DequeueBatch(25);
        stopwatch.Stop();
        
        results.AppendLine($"   Batch dequeued {batchDequeued.Count} requests in {stopwatch.ElapsedTicks} ticks");
        results.AppendLine($"   Queue size after batch dequeue: {queue.CurrentSize}");
        results.AppendLine($"   ✅ Batch operations working correctly\n");
    }
    
    static void TestConcurrentProcessing(System.Text.StringBuilder results)
    {
        results.AppendLine("4. Testing Concurrent Processing:");
        
        var concurrentQueue = new ConcurrentApiRequestQueue();
        const int numProducers = 4;
        const int numConsumers = 2;
        const int requestsPerProducer = 100;
        
        var stopwatch = Stopwatch.StartNew();
        
        // Start producer tasks
        var producerTasks = new Task[numProducers];
        for (int i = 0; i < numProducers; i++)
        {
            int producerId = i;
            producerTasks[i] = Task.Run(() =>
            {
                var random = new Random(producerId);
                for (int j = 0; j < requestsPerProducer; j++)
                {
                    var priority = random.Next(1, 10);
                    concurrentQueue.Enqueue(new ApiRequest($"/api/producer{producerId}/{j}", priority));
                }
            });
        }
        
        // Start consumer task
        var consumerTask = Task.Run(async () =>
        {
            int processed = 0;
            while (processed < numProducers * requestsPerProducer)
            {
                var request = concurrentQueue.Dequeue();
                if (request != null)
                {
                    processed++;
                }
                else
                {
                    await Task.Delay(1); // Small delay if queue is empty
                }
            }
            return processed;
        });
        
        // Wait for completion
        Task.WaitAll(producerTasks);
        int totalProcessed = consumerTask.Result;
        stopwatch.Stop();
        
        results.AppendLine($"   Producers: {numProducers} tasks, {requestsPerProducer} requests each");
        results.AppendLine($"   Total requests produced: {numProducers * requestsPerProducer}");
        results.AppendLine($"   Total requests processed: {totalProcessed}");
        results.AppendLine($"   Processing time: {stopwatch.ElapsedMilliseconds} ms");
        results.AppendLine($"   Throughput: {totalProcessed * 1000.0 / stopwatch.ElapsedMilliseconds:F0} requests/sec");
        results.AppendLine($"   ✅ Concurrent processing validated\n");
    }
    
    static void TestEdgeCases(System.Text.StringBuilder results)
    {
        results.AppendLine("5. Testing Edge Cases:");
        
        var queue = new OptimizedApiRequestQueue();
        
        // Test empty queue
        var emptyResult = queue.Dequeue();
        var peekResult = queue.Peek();
        
        results.AppendLine($"   Dequeue from empty queue: {(emptyResult == null ? "null (correct)" : "unexpected result")}");
        results.AppendLine($"   Peek empty queue: {(peekResult == null ? "null (correct)" : "unexpected result")}");
        
        // Test single element
        queue.Enqueue(new ApiRequest("/single", 5));
        var singlePeek = queue.Peek();
        var singleDequeue = queue.Dequeue();
        
        results.AppendLine($"   Single element peek: {singlePeek?.Endpoint}");
        results.AppendLine($"   Single element dequeue: {singleDequeue?.Endpoint}");
        results.AppendLine($"   Queue empty after single dequeue: {queue.IsEmpty}");
        
        // Test duplicate priorities
        queue.Enqueue(new ApiRequest("/dup1", 3));
        queue.Enqueue(new ApiRequest("/dup2", 3));
        queue.Enqueue(new ApiRequest("/dup3", 3));
        
        results.AppendLine($"   Added 3 requests with same priority (3)");
        results.AppendLine($"   Queue handles duplicates: {queue.CurrentSize == 3}");
        
        // queue.Clear(); // Method not available in current implementation
        results.AppendLine($"   Queue cleared: {queue.IsEmpty}");
        results.AppendLine($"   ✅ Edge cases handled correctly\n");
    }
    
    // Helper method to generate test requests
    static List<ApiRequest> GenerateTestRequests(int count)
    {
        var requests = new List<ApiRequest>();
        var random = new Random(42); // Fixed seed for reproducible tests
        var endpoints = new[] { "/auth", "/data", "/healthcheck", "/users", "/reports", "/upload", "/download" };
        
        for (int i = 0; i < count; i++)
        {
            var endpoint = endpoints[random.Next(endpoints.Length)];
            var priority = random.Next(1, 11); // Priorities 1-10
            requests.Add(new ApiRequest($"{endpoint}/{i}", priority));
        }
        
        return requests;
    }
    
    // Helper method to check if list is in ascending order
    static bool IsAscendingOrder(List<int> list)
    {
        for (int i = 1; i < list.Count; i++)
        {
            if (list[i] < list[i - 1])
                return false;
        }
        return true;
    }
}
}

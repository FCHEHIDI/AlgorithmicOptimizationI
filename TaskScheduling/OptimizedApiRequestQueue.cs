using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace TaskScheduling
{
    /// <summary>
    /// Optimized API Request Queue using Min-Heap for O(log n) operations
    /// Replaces inefficient O(n log n) sorting with efficient heap operations
    /// </summary>
    public class OptimizedApiRequestQueue
    {
        private readonly List<ApiRequest> heap;
        private readonly object lockObject = new object();
        private int heapSize;
        
        // LLM Optimization: Statistics tracking for performance monitoring
        public int TotalEnqueued { get; private set; }
        public int TotalDequeued { get; private set; }
        public int CurrentSize => heapSize;
        public bool IsEmpty => heapSize == 0;
        
        public OptimizedApiRequestQueue(int initialCapacity = 16)
        {
            heap = new List<ApiRequest>(initialCapacity);
            heapSize = 0;
        }
        
        // LLM Optimization: O(log n) enqueue using min-heap structure
        public void Enqueue(ApiRequest request)
        {
            lock (lockObject) // Thread-safe operations
            {
                if (heapSize >= heap.Count)
                {
                    heap.Add(request);
                }
                else
                {
                    heap[heapSize] = request;
                }
                
                HeapifyUp(heapSize);
                heapSize++;
                TotalEnqueued++;
            }
        }
        
        // LLM Optimization: O(log n) dequeue maintaining heap property
        public ApiRequest? Dequeue()
        {
            lock (lockObject)
            {
                if (heapSize == 0)
                    return null;
                
                ApiRequest root = heap[0];
                heapSize--;
                
                if (heapSize > 0)
                {
                    heap[0] = heap[heapSize];
                    HeapifyDown(0);
                }
                
                TotalDequeued++;
                return root;
            }
        }
        
        // LLM Optimization: Batch operations for improved throughput
        public void EnqueueBatch(IEnumerable<ApiRequest> requests)
        {
            lock (lockObject)
            {
                foreach (var request in requests)
                {
                    if (heapSize >= heap.Count)
                    {
                        heap.Add(request);
                    }
                    else
                    {
                        heap[heapSize] = request;
                    }
                    
                    HeapifyUp(heapSize);
                    heapSize++;
                    TotalEnqueued++;
                }
            }
        }
        
        public List<ApiRequest> DequeueBatch(int count)
        {
            var result = new List<ApiRequest>();
            lock (lockObject)
            {
                for (int i = 0; i < count && heapSize > 0; i++)
                {
                    var request = Dequeue();
                    if (request != null)
                        result.Add(request);
                }
            }
            return result;
        }
        
        // LLM Optimization: Efficient heap operations for maintaining min-heap property
        private void HeapifyUp(int index)
        {
            while (index > 0)
            {
                int parentIndex = (index - 1) / 2;
                
                if (heap[index].Priority >= heap[parentIndex].Priority)
                    break;
                
                Swap(index, parentIndex);
                index = parentIndex;
            }
        }
        
        private void HeapifyDown(int index)
        {
            while (true)
            {
                int leftChild = 2 * index + 1;
                int rightChild = 2 * index + 2;
                int smallest = index;
                
                if (leftChild < heapSize && heap[leftChild].Priority < heap[smallest].Priority)
                    smallest = leftChild;
                
                if (rightChild < heapSize && heap[rightChild].Priority < heap[smallest].Priority)
                    smallest = rightChild;
                
                if (smallest == index)
                    break;
                
                Swap(index, smallest);
                index = smallest;
            }
        }
        
        private void Swap(int i, int j)
        {
            (heap[i], heap[j]) = (heap[j], heap[i]);
        }
        
        // LLM Optimization: Performance monitoring and debugging
        public void PrintStatistics()
        {
            lock (lockObject)
            {
                Console.WriteLine($"Queue Statistics:");
                Console.WriteLine($"  Current Size: {CurrentSize}");
                Console.WriteLine($"  Total Enqueued: {TotalEnqueued}");
                Console.WriteLine($"  Total Dequeued: {TotalDequeued}");
                Console.WriteLine($"  Pending Requests: {TotalEnqueued - TotalDequeued}");
            }
        }
        
        public ApiRequest? Peek()
        {
            lock (lockObject)
            {
                return heapSize > 0 ? heap[0] : null;
            }
        }
    }
    
    /// <summary>
    /// Thread-safe concurrent version of the optimized API request queue
    /// LLM Optimization: Uses ConcurrentQueue for high-throughput scenarios
    /// </summary>
    public class ConcurrentApiRequestQueue
    {
        private readonly ConcurrentDictionary<int, ConcurrentQueue<ApiRequest>> priorityQueues;
        private readonly object lockObject = new object();
        private int totalEnqueued = 0;
        private int totalDequeued = 0;
        
        public int TotalEnqueued => totalEnqueued;
        public int TotalDequeued => totalDequeued;
        public bool IsEmpty => priorityQueues.All(kvp => kvp.Value.IsEmpty);
        
        public ConcurrentApiRequestQueue()
        {
            priorityQueues = new ConcurrentDictionary<int, ConcurrentQueue<ApiRequest>>();
        }
        
        public void Enqueue(ApiRequest request)
        {
            var queue = priorityQueues.GetOrAdd(request.Priority, _ => new ConcurrentQueue<ApiRequest>());
            queue.Enqueue(request);
            Interlocked.Increment(ref totalEnqueued);
        }
        
        public ApiRequest? Dequeue()
        {
            // Process by priority (lowest first)
            foreach (var priority in priorityQueues.Keys.OrderBy(p => p))
            {
                if (priorityQueues.TryGetValue(priority, out var queue) && 
                    queue.TryDequeue(out var request))
                {
                    Interlocked.Increment(ref totalDequeued);
                    return request;
                }
            }
            return null;
        }
        
        public async Task ProcessRequestsAsync(Func<ApiRequest, Task> processor, CancellationToken cancellationToken = default)
        {
            var tasks = new List<Task>();
            
            while (!IsEmpty && !cancellationToken.IsCancellationRequested)
            {
                var request = Dequeue();
                if (request != null)
                {
                    tasks.Add(processor(request));
                    
                    // Limit concurrent tasks to prevent resource exhaustion
                    if (tasks.Count >= Environment.ProcessorCount)
                    {
                        await Task.WhenAny(tasks);
                        tasks.RemoveAll(t => t.IsCompleted);
                    }
                }
                else
                {
                    await Task.Delay(10, cancellationToken); // Brief pause when queue is empty
                }
            }
            
            await Task.WhenAll(tasks);
        }
    }
}

using System;
using System.Collections.Generic;

namespace TaskScheduling
{
    public class ApiRequest
    {
        public string Endpoint { get; set; }
        public int Priority { get; set; }
        
        public ApiRequest(string endpoint, int priority)
        {
            Endpoint = endpoint;
            Priority = priority;
        }
    }
    
    public class ApiRequestQueue
    {
        private List<ApiRequest> requests = new List<ApiRequest>();
        
        public void Enqueue(ApiRequest request)
        {
            requests.Add(request);
            requests.Sort((a, b) => a.Priority.CompareTo(b.Priority)); // Inefficient O(n log n) sorting
        }
        
        public ApiRequest? Dequeue()
        {
            if (requests.Count == 0)
                return null;
            ApiRequest nextRequest = requests[0];
            requests.RemoveAt(0);
            return nextRequest;
        }
        
        public bool IsEmpty => requests.Count == 0;
    }
}

using System.Collections.Generic;

namespace QueueLess.API.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }

    public class ApiErrorResponse
    {
        public int StatusCode { get; set; }
        public string Error { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public List<string> Details { get; set; } = new List<string>();
    }
}

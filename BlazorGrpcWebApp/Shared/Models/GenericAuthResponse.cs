namespace BlazorGrpcWebApp.Shared.Models
{
    public class GenericAuthResponse<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}

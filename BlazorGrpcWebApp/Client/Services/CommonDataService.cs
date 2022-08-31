namespace BlazorGrpcWebApp.Client.Services
{
    public interface ICommonDataService
    {
        public string? EmailAddress { get; set; }
        public DateTime? CodeExpiredDateToDisplay { get; set; }
    }

    public class CommonDataService : ICommonDataService
    {
        public string? EmailAddress { get; set; }
        public DateTime? CodeExpiredDateToDisplay { get; set; }
    }
}

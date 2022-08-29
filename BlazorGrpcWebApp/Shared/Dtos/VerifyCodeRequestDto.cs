namespace BlazorGrpcWebApp.Shared.Dtos
{
    public class VerifyCodeRequestDto
    {
        public string VerificationCode { get; set; }
        public string UserEmail { get; set; }
    }
}

namespace EFund_API.Dtos
{
    public class ForgotPasswordResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string TempPassword { get; set; }
    }
}

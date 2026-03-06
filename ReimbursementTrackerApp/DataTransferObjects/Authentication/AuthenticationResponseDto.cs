namespace ReimbursementTrackerApp.DataTransferObjects.Authentication
{
    public class AuthenticationResponseDto
    {

        public DateTime Expiration { get; set; }
        public string Token { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}

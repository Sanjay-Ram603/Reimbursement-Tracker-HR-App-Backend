namespace ReimbursementTrackerApp.DataTransferObjects.UserProfile
{
    public class UpdateUserProfileRequestDto
    {
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}

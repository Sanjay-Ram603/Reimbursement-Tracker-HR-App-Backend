namespace ReimbursementTrackerApp.DataTransferObjects.Report
{
    public class ReimbursementReportResponseDto
    {
        public Guid ReimbursementRequestId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Description { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeEmail { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
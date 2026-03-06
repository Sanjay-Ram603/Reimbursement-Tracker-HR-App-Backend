namespace ReimbursementTrackerApp.DataTransferObjects.Report
{
    public class ReimbursementReportResponseDto
    {
        public Guid ReimbursementRequestId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

namespace ReimbursementTrackerApp.DataTransferObjects.DashboardSummary
{

    public class DashboardSummaryDto
    {
        public int TotalRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int RejectedRequests { get; set; }
        public int PendingRequests { get; set; }
        public decimal TotalAmount { get; set; }
    }

}

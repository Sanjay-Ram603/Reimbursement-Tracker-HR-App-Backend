namespace ReimbursementTrackerApp.DataTransferObjects.PendingApproval
{

    public class PendingApprovalDto
    {
        public Guid RequestId { get; set; }

        public string EmployeeName { get; set; }

        public decimal Amount { get; set; }

        public string Status { get; set; }
    }


}

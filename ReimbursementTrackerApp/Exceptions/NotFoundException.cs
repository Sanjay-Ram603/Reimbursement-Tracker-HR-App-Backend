using System.Net;

namespace ReimbursementTrackerApp.Exceptions
{
    public class NotFoundException : CustomException
    {
        public NotFoundException(string message)
            : base(message, (int)HttpStatusCode.NotFound)
        {
        }
    }
}

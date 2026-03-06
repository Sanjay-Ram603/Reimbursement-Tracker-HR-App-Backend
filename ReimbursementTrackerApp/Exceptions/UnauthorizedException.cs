using System.Net;

namespace ReimbursementTrackerApp.Exceptions
{
    public class UnauthorizedException : CustomException
    {
        public UnauthorizedException(string message)
            : base(message, (int)HttpStatusCode.Unauthorized)
        {
        }
    }
}

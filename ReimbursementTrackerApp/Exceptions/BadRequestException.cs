using System.Net;

namespace ReimbursementTrackerApp.Exceptions
{
    public class BadRequestException : CustomException
    {
        public BadRequestException(string message)
            : base(message, (int)HttpStatusCode.BadRequest)
        {
        }
    }
}

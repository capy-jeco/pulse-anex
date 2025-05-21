using portal_agile.Dtos.Users.Requests;

namespace portal_agile.Exceptions.Users
{
    public class InvalidUserCreateDataException : Exception
    {
        public InvalidUserCreateDataException() : base("User not created.")
        {
            Request = null!;
        }

        public InvalidUserCreateDataException(string message) : base(message)
        {
            Request = null!;
        }

        public InvalidUserCreateDataException(string message, Exception innerException) : base(message, innerException)
        {
            Request = null!;
        }

        public UserCreateRequest Request { get; set; }

        public InvalidUserCreateDataException(UserCreateRequest request) : base($"User data request is invalid. {request}")
        {
            Request = request;
        }
    }
}

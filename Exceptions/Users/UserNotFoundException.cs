namespace portal_agile.Exceptions.Users
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException() : base("User not found.")
        {
        }

        public UserNotFoundException(string message) : base(message)
        {
        }

        public UserNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public Guid UserId { get; set; }

        public UserNotFoundException(Guid userId) : base($"User with ID {userId} not found.")
        {
            UserId = userId;
        }
    }
}

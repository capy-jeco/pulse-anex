namespace portal_agile.Exceptions.Users
{
    public class InvalidUserIdException : Exception
    {
        public string UserId { get; }

        public InvalidUserIdException(string userId)
            : base($"User ID '{userId}' is invalid.")
        {
            UserId = userId;
        }
    }
}

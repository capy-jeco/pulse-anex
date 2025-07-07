namespace portal_agile.Exceptions.Authentication
{
    public class UserLockedException : Exception
    {
        public UserLockedException() : base("User account is locked.")
        {
        }

        public UserLockedException(string message) : base(message)
        {
        }

        public UserLockedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        // Optional: Add lockout-specific properties
        public DateTime? LockoutEnd { get; set; }
        public int FailedAttempts { get; set; }
        public string? Reason { get; set; } // e.g., "Too many failed attempts", "Administrative lock"

        // Constructor with lockout details
        public UserLockedException(DateTime lockoutEnd, int failedAttempts, string reason = "Account locked due to security policy")
            : base($"Account is locked until {lockoutEnd:yyyy-MM-dd HH:mm:ss UTC}. Reason: {reason}")
        {
            LockoutEnd = lockoutEnd;
            FailedAttempts = failedAttempts;
            Reason = reason;
        }
    }
}

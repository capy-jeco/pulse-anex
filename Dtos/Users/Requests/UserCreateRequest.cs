﻿namespace portal_agile.Dtos.Users.Requests
{
    public class UserCreateRequest
    {
        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public required string Email { get; set; }

        public required string UserName { get; set; }

        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; } = true;
    }
}

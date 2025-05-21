namespace portal_agile.Dtos.Users.Requests
{
    public class UserUpdateViaKeyRequest
    {
        public required string Key { get; set; }

        public required object NewValue{ get; set; }
    }
}

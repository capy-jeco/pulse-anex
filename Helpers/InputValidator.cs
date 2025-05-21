namespace portal_agile.Helpers
{
    public interface IInputValidator
    {
        bool IsValidGuid(string input, out string? normalizedGuid);
    }

    public class InputValidator : IInputValidator
    {
        public bool IsValidGuid(string input, out string? normalizedGuid)
        {
            if (Guid.TryParse(input, out Guid guid))
            {
                normalizedGuid = guid.ToString();
                return true;
            }
            normalizedGuid = null!;
            return false;
        }
    }
}

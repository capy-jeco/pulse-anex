using Microsoft.AspNetCore.Mvc;

namespace portal_agile.Controllers
{
    public class TenantBaseController : Controller
    {
        protected string? TenantId => HttpContext.Items["TenantId"]?.ToString();

        protected IActionResult ValidateTenant()
        {
            if (string.IsNullOrEmpty(TenantId))
            {
                return BadRequest("Invalid tenant");
            }
            return Ok(); // Replace null with a valid IActionResult to avoid CS8603
        }
    }
}

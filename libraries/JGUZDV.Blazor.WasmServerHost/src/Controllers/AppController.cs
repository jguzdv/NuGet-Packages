using System.Text.Json;

using JGUZDV.Extensions.Json.Converters;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JGUZDV.Blazor.WasmServerHost.Controllers
{
    [Route("_app")]
    public class AppController : ControllerBase
    {
        [HttpGet("sign-in")]
        [Authorize]
        public IActionResult SignInAsync(string nextUrl = "~/")
        {
            if (!nextUrl.StartsWith('~'))
                nextUrl = '~' + nextUrl;

            return LocalRedirect(nextUrl);
        }

        [HttpGet("sign-out")]
        public async Task<IActionResult> SignOutAsync(string nextUrl = "~/")
        {
            if (!nextUrl.StartsWith('~'))
                nextUrl = '~' + nextUrl;

            await HttpContext.SignOutAsync();
            return LocalRedirect(nextUrl);
        }


        [HttpGet("principal")]
        public IActionResult GetPrincipal()
        {
            if (User.Identities.Any(x => x.IsAuthenticated))
            {
                var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
                options.Converters.Add(new ClaimsPrincipalConverter());

                var jsonPrincipal = JsonSerializer.Serialize(User, options);

                return Ok(jsonPrincipal);
            }

            return Ok();
        }
    }
}

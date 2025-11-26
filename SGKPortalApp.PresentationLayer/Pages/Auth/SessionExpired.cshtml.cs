using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SGKPortalApp.PresentationLayer.Pages.Auth
{
    public class SessionExpiredModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync()
        {
            // Kullanıcıyı logout et
            await HttpContext.SignOutAsync();
            return Page();
        }
    }
}

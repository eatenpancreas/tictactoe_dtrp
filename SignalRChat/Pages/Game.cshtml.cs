using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SignalRChat.Pages
{
    public class GameModel : PageModel
    {
        public string Slug { get; private set; }

        public void OnGet(string slug)
        {
            Slug = slug;
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Snsc.SecureShareAdmin.Domain;
using Snsc.SecureShareAdmin.Users;

namespace Snsc.SecureShareAdmin.Pages.Users;

public sealed class IndexModel : PageModel
{
    private readonly ExternalUserDirectory _users;

    public IndexModel(ExternalUserDirectory users)
    {
        _users = users;
    }

    [BindProperty]
    public string? Search { get; set; }

    public IReadOnlyList<ExternalUser> Users { get; private set; } = Array.Empty<ExternalUser>();
    public string EmptyMessage { get; private set; } = "Enter part of a user email address and click Search.";

    public void OnGet()
    {
    }

    public void OnPostSearch()
    {
        if (string.IsNullOrWhiteSpace(Search))
        {
            EmptyMessage = "Enter part of a user email address and click Search.";
            return;
        }

        Users = _users.FindByUserName("%" + Search.Trim() + "%");
        if (Users.Count == 0)
        {
            EmptyMessage = "No Users Found";
        }
    }
}

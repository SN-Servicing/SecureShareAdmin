using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Snsc.SecureShareAdmin.Users;

namespace Snsc.SecureShareAdmin.Pages.Users;

public sealed class CreateModel : PageModel
{
    private readonly ExternalUserDirectory _users;

    public CreateModel(ExternalUserDirectory users)
    {
        _users = users;
    }

    [BindProperty]
    public string UserName { get; set; } = string.Empty;

    [BindProperty]
    public string FirstName { get; set; } = string.Empty;

    [BindProperty]
    public string LastName { get; set; } = string.Empty;

    [BindProperty]
    public int UserAccess { get; set; }

    public string? ErrorMessage { get; private set; }
    public string? Message { get; private set; }

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
        {
            ErrorMessage = "User name, first name, and last name are required.";
            return Page();
        }

        (ExternalUserCreateResult result, Guid? userId) = _users.Create(
            UserName.Trim(),
            FirstName.Trim(),
            LastName.Trim(),
            UserAccess);

        if (result != ExternalUserCreateResult.Success || !userId.HasValue)
        {
            ErrorMessage = result switch
            {
                ExternalUserCreateResult.DuplicateUserName => "A user with that user name already exists.",
                ExternalUserCreateResult.DuplicateEmail => "A user with that email already exists.",
                ExternalUserCreateResult.Rejected => "The user was rejected by the provider.",
                _ => "Unable to create the user."
            };
            return Page();
        }

        return RedirectToPage("/Users/Edit", new { userName = UserName.Trim() });
    }
}

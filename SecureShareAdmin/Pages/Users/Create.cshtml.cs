using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Snsc.SecureShareAdmin.Domain;
using Snsc.SecureShareAdmin.Email;
using Snsc.SecureShareAdmin.Users;

namespace Snsc.SecureShareAdmin.Pages.Users;

public sealed class CreateModel : PageModel
{
    private readonly ExternalUserDirectory _users;
    private readonly NewAccountEmailSender _newAccountEmail;

    public CreateModel(ExternalUserDirectory users, NewAccountEmailSender newAccountEmail)
    {
        _users = users;
        _newAccountEmail = newAccountEmail;
    }

    public string? ErrorMessage { get; private set; }

    [BindProperty]
    public string FirstName { get; set; } = string.Empty;

    [BindProperty]
    public string LastName { get; set; } = string.Empty;

    [BindProperty]
    public bool LoanAccess { get; set; }

    [BindProperty]
    public bool SharedFilesAccess { get; set; }

    [BindProperty]
    public string UserName { get; set; } = string.Empty;

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

        string trimmedUserName = UserName.Trim();
        int userAccess = ExternalUserAccessMask.ToPersistedValue(LoanAccess, SharedFilesAccess);
        (ExternalUserCreateResult result, Guid? userId) = _users.Create(
            trimmedUserName,
            FirstName.Trim(),
            LastName.Trim(),
            userAccess);

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

        NewAccountEmailResult emailResult = _newAccountEmail.SendWelcomeEmail(trimmedUserName);
        if (!string.IsNullOrEmpty(emailResult.ErrorMessage))
        {
            TempData["Message"] =
                "User created, but the welcome email could not be sent: " + emailResult.ErrorMessage;
        }
        else if (emailResult.Sent)
        {
            TempData["Message"] = "User created. Welcome email sent.";
        }

        return RedirectToPage("/Users/Edit", new { externalUserId = userId.Value });
    }
}

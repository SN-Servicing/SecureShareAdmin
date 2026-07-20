using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Snsc.SecureShareAdmin.Domain;
using Snsc.SecureShareAdmin.Users;

namespace Snsc.SecureShareAdmin.Pages.Users;

public sealed class IndexModel : PageModel
{
    private const int EmailMatchPageSize = 50;
    private const int AllUsersPageSize = 10000;

    private readonly ExternalUserDirectory _users;

    public IndexModel(ExternalUserDirectory users)
    {
        _users = users;
    }

    [BindProperty]
    public string? EmailSearch { get; set; }

    [BindProperty]
    public string? NameSearch { get; set; }

    public IReadOnlyList<ExternalUser> Users { get; private set; } = Array.Empty<ExternalUser>();

    public string EmptyMessage { get; private set; } =
        "Enter an email and/or name search and click Search.";

    public void OnGet()
    {
    }

    public void OnPostSearch()
    {
        bool hasEmail = !string.IsNullOrWhiteSpace(EmailSearch);
        bool hasName = !string.IsNullOrWhiteSpace(NameSearch);
        if (!hasEmail && !hasName)
        {
            EmptyMessage = "Enter an email and/or name search and click Search.";
            return;
        }

        Users = LoadMatchingUsers(hasEmail);
        if (hasName)
        {
            Users = UserNameSearch.FilterAndRank(Users, NameSearch);
        }

        if (Users.Count == 0)
        {
            EmptyMessage = "No Users Found";
        }
    }

    private IReadOnlyList<ExternalUser> LoadMatchingUsers(bool hasEmail)
    {
        if (hasEmail)
        {
            return _users.FindByUserName("%" + EmailSearch!.Trim() + "%", EmailMatchPageSize);
        }

        return _users.FindByUserName("%", AllUsersPageSize);
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Snsc.SecureShareAdmin.Domain;
using Snsc.SecureShareAdmin.Security;
using Snsc.SecureShareAdmin.Users;
using Snsc.SecureShareAdmin.Zones;

namespace Snsc.SecureShareAdmin.Pages.Users;

public sealed class EditModel : PageModel
{
    private readonly ExternalUserDirectory _users;
    private readonly ZoneCatalog _zoneCatalog;
    private readonly AmsUserContext _amsUser;

    public EditModel(ExternalUserDirectory users, ZoneCatalog zoneCatalog, AmsUserContext amsUser)
    {
        _users = users;
        _zoneCatalog = zoneCatalog;
        _amsUser = amsUser;
    }

    [BindProperty(SupportsGet = true)]
    public string UserName { get; set; } = string.Empty;

    [BindProperty]
    public string FirstName { get; set; } = string.Empty;

    [BindProperty]
    public string LastName { get; set; } = string.Empty;

    [BindProperty]
    public int UserAccess { get; set; }

    [BindProperty]
    public string? ZoneSearchText { get; set; }

    [BindProperty]
    public List<int> SelectedZoneIds { get; set; } = new();

    public ExternalUser? ExternalUser { get; private set; }
    public IReadOnlyList<Zone> CurrentZones { get; private set; } = Array.Empty<Zone>();
    public IReadOnlyList<Zone> FoundZones { get; private set; } = Array.Empty<Zone>();
    public string? Message { get; private set; }
    public string? ErrorMessage { get; private set; }

    public IActionResult OnGet()
    {
        if (!LoadUser())
        {
            return RedirectToPage("/Users/Index");
        }

        return Page();
    }

    public IActionResult OnPostSave()
    {
        if (!LoadUser(bindForm: true))
        {
            return RedirectToPage("/Users/Index");
        }

        _users.Update(UserName, FirstName.Trim(), LastName.Trim(), UserAccess, null);

        foreach (int zoneId in SelectedZoneIds.Distinct())
        {
            if (CurrentZones.Any(z => z.ZoneId == zoneId))
            {
                continue;
            }

                _zoneCatalog.AddUserPermission(zoneId, ExternalUser!.UserId);
        }

        LoadZones();
        SelectedZoneIds.Clear();
        FoundZones = Array.Empty<Zone>();
        Message = "Update Successful.";
        return Page();
    }

    public IActionResult OnPostSearchZones()
    {
        if (!LoadUser(bindForm: true))
        {
            return RedirectToPage("/Users/Index");
        }

        int amsUserId = _amsUser.RequireAmsUserId();
        IReadOnlyList<Zone> allZones = _zoneCatalog.GetZonesForAmsUser(amsUserId);
        FoundZones = Snsc.SecureShareAdmin.Zones.ZoneSearch.FilterAndRank(allZones, null, ZoneSearchText);
        return Page();
    }

    public IActionResult OnPostRemoveZone(int zonePermissionId)
    {
        _zoneCatalog.RemoveUserPermission(zonePermissionId);
        return RedirectToPage(new { UserName });
    }

    private bool LoadUser(bool bindForm = false)
    {
        ExternalUser = _users.GetByUserName(UserName);
        if (ExternalUser == null || ExternalUser.UserId == Guid.Empty)
        {
            return false;
        }

        if (!bindForm)
        {
            string[] nameParts = (ExternalUser.Name ?? string.Empty).Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            FirstName = nameParts.Length > 0 ? nameParts[0] : string.Empty;
            LastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;
            if (int.TryParse(ExternalUser.Comment?.Split('~')[0], out int access))
            {
                UserAccess = access;
            }
        }

        LoadZones();
        return true;
    }

    private void LoadZones()
    {
        if (ExternalUser == null)
        {
            return;
        }

        CurrentZones = _zoneCatalog.GetZonesForExternalUser(ExternalUser.UserId);
    }
}

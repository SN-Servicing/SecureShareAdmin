using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Snsc.SecureShareAdmin.Domain;
using Snsc.SecureShareAdmin.Security;
using Snsc.SecureShareAdmin.Users;
using Snsc.SecureShareAdmin.Zones;

namespace Snsc.SecureShareAdmin.Pages.Zones;

public sealed class CreateModel : PageModel
{
    private readonly AmsUserContext _amsUser;
    private readonly ZoneCatalog _zoneCatalog;
    private readonly ExternalUserDirectory _users;

    public CreateModel(AmsUserContext amsUser, ZoneCatalog zoneCatalog, ExternalUserDirectory users)
    {
        _amsUser = amsUser;
        _zoneCatalog = zoneCatalog;
        _users = users;
    }

    [BindProperty]
    public int ZoneTypeId { get; set; }

    [BindProperty]
    public string Description { get; set; } = string.Empty;

    [BindProperty]
    public string PrimaryValue { get; set; } = string.Empty;

    [BindProperty]
    public string? SecondaryValue { get; set; }

    [BindProperty]
    public string? UserSearch { get; set; }

    [BindProperty]
    public List<string> SelectedUserNames { get; set; } = new();

    public List<SelectListItem> ZoneTypeOptions { get; private set; } = new();
    public IReadOnlyList<ExternalUser> FoundUsers { get; private set; } = Array.Empty<ExternalUser>();
    public ZoneType? SelectedType { get; private set; }
    public string? ErrorMessage { get; private set; }

    public void OnGet()
    {
        BindTypes();
    }

    public void OnPostSearchUsers()
    {
        BindTypes();
        RestoreFoundUsers();
    }

    public IActionResult OnPostChangeZoneType()
    {
        BindTypes();
        ClearSecondaryIfNotApplicable();
        RestoreFoundUsers();
        return Page();
    }

    public IActionResult OnPostCreate()
    {
        BindTypes();
        ClearSecondaryIfNotApplicable();
        if (ZoneTypeId <= 0 || string.IsNullOrWhiteSpace(Description) || string.IsNullOrWhiteSpace(PrimaryValue))
        {
            ErrorMessage = "Zone type, description, and primary value are required.";
            return Page();
        }

        int zoneId = _zoneCatalog.CreateZone(
            PrimaryValue.Trim(),
            SecondaryValueForPersist(),
            ZoneTypeId,
            Description.Trim());
        if (zoneId == -1)
        {
            ErrorMessage = $"{SelectedType?.PrimaryObjectIdFieldName ?? "Primary value"} {PrimaryValue} already exists.";
            return Page();
        }

        foreach (string userName in SelectedUserNames.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            ExternalUser? user = _users.GetByUserName(userName);
            if (user != null && user.UserId != Guid.Empty)
            {
                _zoneCatalog.AddUserPermission(zoneId, user.UserId);
            }
        }

        return RedirectToPage("/Zones/Edit", new { zoneId, saved = 1 });
    }

    private void BindTypes()
    {
        int amsUserId = _amsUser.RequireAmsUserId();
        IReadOnlyList<Zone> zones = _zoneCatalog.GetZonesForAmsUser(amsUserId);
        IReadOnlyList<ZoneType> types = _zoneCatalog.GetDistinctZoneTypes(zones);
        ZoneTypeOptions = types.Select(t => new SelectListItem(t.Name, t.ZoneTypeId.ToString())).ToList();
        SelectedType = types.FirstOrDefault(t => t.ZoneTypeId == ZoneTypeId) ?? types.FirstOrDefault();
        if (SelectedType != null && ZoneTypeId == 0)
        {
            ZoneTypeId = SelectedType.ZoneTypeId;
        }
    }

    private void ClearSecondaryIfNotApplicable()
    {
        if (string.IsNullOrWhiteSpace(SelectedType?.SecondaryObjectIdFieldName))
        {
            SecondaryValue = null;
        }
    }

    private string SecondaryValueForPersist()
    {
        if (string.IsNullOrWhiteSpace(SelectedType?.SecondaryObjectIdFieldName))
        {
            return string.Empty;
        }

        return SecondaryValue?.Trim() ?? string.Empty;
    }

    private void RestoreFoundUsers()
    {
        if (!string.IsNullOrWhiteSpace(UserSearch))
        {
            FoundUsers = _users.FindByUserName("%" + UserSearch.Trim() + "%");
        }
    }
}
